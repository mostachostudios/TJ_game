// https://www.youtube.com/watch?v=BEIaakl9vJE
// https://answers.unity.com/questions/17566/how-can-i-make-my-player-a-charactercontroller-pus.html
// https://answers.unity.com/questions/578443/jumping-with-character-controller.html
// https://docs.unity3d.com/es/530/ScriptReference/CharacterController.Move.html

//https://docs.unity3d.com/Manual/class-CharacterController.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_gravity = -9.81f;
    [SerializeField] float m_pushPower = 20f;

    [Header("Speed values")]
    [SerializeField] float m_walkSpeed = 1f;
    [SerializeField] float m_runSpeed = 2f;
    [SerializeField] float m_stealthSpeed = 0.1f;
    [SerializeField] float m_crawlSpeed = 0.25f;
    [SerializeField] float m_crouchSpeed = 0.4f;
    [SerializeField] float m_dodgeSpeed = 1.3f;
    [SerializeField] float m_backwardWalkSpeed = 0.3f;
    [SerializeField] float m_pushSpeed = 0.6f;
    [SerializeField] float m_jumpSpeed = 2.5f;

    [Header("Frozen time lapse")]
    [SerializeField] float m_TimeFalling = 2f;
    [SerializeField] float m_TimeFallingDown = 5f;
    [SerializeField] float m_TimeTerrified = 8f;

    [Header("SFX")]
    [SerializeField] AudioClip m_StepFootClip;
    [SerializeField] AudioClip m_StepFootRunClip;

    private AudioSource m_AudioSource;

    private CharacterController m_characterController;
    private Animator m_animator;

    //private Vector3 m_movement;
    private float m_ySpeed;
    private float m_Speed;
    private List<string> m_AnimationStates;
    // TODO save current's player state somewhere (so it can be checked from outside)

    private bool m_isPushing = false;
    private bool m_isJumping = false;

    private bool m_isPlayerFrozen = false;

    private float m_initColliderRadius;
    private float m_initColliderHeight;
    private Vector3 m_initColliderCenter;

    private bool m_isOrbitCamera = true;

    private bool m_ReadInput = true;

    void Awake()
    {
        m_characterController = gameObject.GetComponent<CharacterController>();
        m_animator = gameObject.GetComponent<Animator>();

        m_AudioSource = gameObject.GetComponent<AudioSource>();
        m_AudioSource.clip = m_StepFootClip;

        m_initColliderRadius = m_characterController.radius;
        m_initColliderHeight = m_characterController.height;
        m_initColliderCenter = m_characterController.center;
        
        m_AnimationStates = new List<string>();
        m_AnimationStates.Add("isIdle");
        m_AnimationStates.Add("isPushing");
        m_AnimationStates.Add("isWalking");
        m_AnimationStates.Add("isRunning");
        m_AnimationStates.Add("isStealth");
        m_AnimationStates.Add("isCrouchIdle");
        m_AnimationStates.Add("isCrouchWalking");
        m_AnimationStates.Add("isCrawling");
        m_AnimationStates.Add("isDodgingLeft"); // Not available in Zenith Camera
        m_AnimationStates.Add("isDodgingRight"); // Not available in Zenith Camera
        m_AnimationStates.Add("isBackwardWalking"); // Not available in Zenith Camera
        m_AnimationStates.Add("isFalling");
        m_AnimationStates.Add("isFallingDown");
        m_AnimationStates.Add("isTerrified");
        //m_AnimationStates.Add("isSeatIdle");
        //m_AnimationStates.Add("isSitToStand");

        SetAnimatorState("isIdle");

        m_ySpeed = 0.0f;
        m_Speed = 0.0f;
    }

    void Update()
    {
        if (m_ReadInput && !m_isPlayerFrozen) // If m_isPlayerFrozen is true then it is either falling or terrified (so it will ignore player's input)
        {
            ApplyAnimationAndMovement();
        }
    }

    private void ApplyAnimationAndMovement()
    {
        // Checking if isMoving this way might introduce a bit delay/lag since GetAxis returns != 0 even after a short time the user has stopping pressing the keyboard 
        //bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.0001f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.0001f;
        bool isMoving = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
        || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isStealth = Input.GetKey(KeyCode.Tab);
        bool isCrawling = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool isCrouch = Input.GetKey(KeyCode.LeftAlt);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = Vector3.forward * vertical + Vector3.right * horizontal;

        // TODO FIX problem with some transitions (push and run, etc) // Use ignore buffer if key was pressed before, or search for a similar way
        if (isMoving) // Character is moving
        {
            if (m_isPushing)
            {
                MovePush(direction);
            }
            else if (isStealth)
            {
                MoveStealth(direction);
            }
            else if (isCrawling)
            {
                MoveCrawl(direction);
            }
            else if (isCrouch)
            {
                MoveCrouch(direction);
            }
            else
            {
                if (m_isOrbitCamera && horizontal > 0f)
                {
                    MoveDodgeRight(direction);
                }
                else if (m_isOrbitCamera && horizontal < 0f)
                {
                    MoveDodgeLeft(direction);
                }
                else if (m_isOrbitCamera && vertical < 0f)
                {
                    MoveBackwards(direction);
                }
                else if (isRunning)
                {
                    MoveRun(direction);
                }
                else
                {
                    MoveWalk(direction);
                }
            }
        }
        else // Still
        {
            if (isCrouch || isCrawling)
            {
                SetCrouch();
            }
            else
            {
                SetIdle();
            }
        }
    }

    private void MoveCharacther(Vector3 nextMovement)
    {
        if (m_characterController.enabled)
        {
            Vector3 movement;
            if (nextMovement != Vector3.zero)
            {
                movement = m_isOrbitCamera ? transform.TransformDirection(nextMovement) : nextMovement;
            }
            else
            {
                movement = transform.forward;
            }

            if (!m_isOrbitCamera)
            {
                transform.LookAt(transform.position + movement);
            }

            if (m_characterController.isGrounded)
            {
                m_ySpeed = 0.0f;
                if (m_isJumping) // Jumping is only active depending animation state (forward walk, run and idle)
                {
                    m_ySpeed = m_jumpSpeed;
                    m_isJumping = false;
                }
            }

            movement *= m_Speed;
            m_ySpeed += m_gravity * Time.deltaTime;
            movement.y = m_ySpeed;

            m_characterController.Move(movement * Time.deltaTime);

            if (!m_AudioSource.isPlaying && m_Speed > 0f)
            {
                if (m_Speed >= (m_runSpeed - 0.01f))
                {
                    m_AudioSource.clip = m_StepFootRunClip;
                    m_AudioSource.PlayDelayed(0.05f);
                }
                else if (m_Speed >= (m_walkSpeed - 0.01f))
                {
                    m_AudioSource.clip = m_StepFootClip;
                    m_AudioSource.PlayDelayed(0.1f);
                }
                else if (m_Speed >= (m_crouchSpeed - 0.01f))
                {
                    m_AudioSource.clip = m_StepFootClip;
                    m_AudioSource.PlayDelayed(0.3f);
                }
                else
                {
                    m_AudioSource.clip = m_StepFootClip;
                    m_AudioSource.PlayDelayed(1.7f);
                }
            }
        }
    }

    //TODO FIX si el usuario continua haciendo push y se sale del alcance del objeto, continuará el pushing aunque no esté tocando nada
    // y no cambiará de estado!
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetMouseButton(0))
        {
            var body = hit.collider.attachedRigidbody;
            if (body != null && !body.isKinematic)
            {
                m_isPushing = true;
                body.AddForceAtPosition(hit.controller.gameObject.transform.forward * m_pushPower, hit.point);
            }
            else
            {
            //   m_isPushing = false; // Solución no válida
            }
        }
        else
        {
            m_isPushing = false;
        }
    }

    public void MovePush(Vector3 direction = new Vector3())
    {
        ResetCollider();
        SetAnimatorState("isPushing");
        m_Speed = m_pushSpeed;
        MoveCharacther(direction);
    }

    public void MoveStealth(Vector3 direction = new Vector3())
    {
        ResetCollider();
        SetAnimatorState("isStealth");
        m_Speed = m_stealthSpeed;
        MoveCharacther(direction);
    }

    public void MoveCrawl(Vector3 direction = new Vector3())
    {
        SetCrawlCollider();
        SetAnimatorState("isCrawling");
        m_Speed = m_crawlSpeed;
        MoveCharacther(direction);
    }

    public void MoveCrouch(Vector3 direction = new Vector3())
    {
        SetCrouchCollider();
        SetAnimatorState("isCrouchWalking");
        m_Speed = m_crouchSpeed;
        MoveCharacther(direction);
    }

    private void MoveDodgeLeft(Vector3 direction)
    {
        ResetCollider();
        SetAnimatorState("isDodgingLeft");
        m_Speed = m_dodgeSpeed;
        MoveCharacther(direction);
    }

    private void MoveDodgeRight(Vector3 direction)
    {
        ResetCollider();
        SetAnimatorState("isDodgingRight");
        m_Speed = m_dodgeSpeed;
        MoveCharacther(direction);
    }

    private void MoveBackwards(Vector3 direction)
    {
        ResetCollider();
        SetAnimatorState("isBackwardWalking");
        m_Speed = m_backwardWalkSpeed;
        MoveCharacther(direction);
    }

    public void MoveRun(Vector3 direction = new Vector3())
    {
        ResetCollider();
        CheckJumping();
        SetAnimatorState("isRunning");
        m_Speed = m_runSpeed;
        MoveCharacther(direction);
    }

    public void MoveWalk(Vector3 direction = new Vector3())
    {
        ResetCollider();
        CheckJumping();
        SetAnimatorState("isWalking");
        m_Speed = m_walkSpeed;
        MoveCharacther(direction);
    }

    public void SetCrouch()
    {
        SetCrouchCollider();
        SetAnimatorState("isCrouchIdle");
        m_Speed = 0f;
        MoveCharacther(Vector3.zero);
    }

    public void SetIdle()
    {
        ResetCollider();
        CheckJumping();
        SetAnimatorState("isIdle");
        m_Speed = 0f;
        MoveCharacther(Vector3.zero);
    }

    public void Jump()
    {
        if (m_characterController.isGrounded)
        {
            m_animator.SetTrigger("isJumping");
            m_isJumping = true;
        }
    }

    private void CheckJumping()
    {
        //TODO CHECK NO funciona si se presiona arrow up y left a la vez (pero si funciona ok con A + W y también con NumPad!!!!) 
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    public float SetFalling()
    {
        m_isPlayerFrozen = true;
        SetAnimatorState("isFalling");
        m_Speed = 0f;
        StartCoroutine(WaitUntilNotFrozen(m_TimeFalling));
        return m_TimeFalling;
    }

    public float SetFallingDown()
    {
        m_isPlayerFrozen = true;
        SetAnimatorState("isFallingDown");
        m_Speed = 0f;
        StartCoroutine(WaitUntilNotFrozen(m_TimeFallingDown));
        return m_TimeFallingDown;
    }

    public float SetTerrified()
    {
        m_isPlayerFrozen = true;
        SetAnimatorState("isTerrified");
        m_Speed = 0f;
        StartCoroutine(WaitUntilNotFrozen(m_TimeTerrified));
        return m_TimeTerrified;
    }

    IEnumerator WaitUntilNotFrozen(float timeFrozen)
    {
        yield return new WaitForSeconds(timeFrozen);
        m_isPlayerFrozen = false;
        yield return null;
    }

    public void SetAnimatorState(string state)
    {
        foreach (string entry in m_AnimationStates)
        {
            m_animator.SetBool(entry, false);
        }
        m_animator.SetBool(state, true);
    }

    private void SetCrouchCollider()
    {
        //This values are calculated based on initial collider properties. If changed in editor, then these should be recalculated as well.
        m_characterController.radius = 0.25f;
        m_characterController.height = 1.0f;
        m_characterController.center = new Vector3(m_characterController.center.x, 0.5f, m_characterController.center.z);
    }

    private void SetCrawlCollider()
    {
        //This values are calculated based on initial collider properties. If changed in editor, then these should be recalculated as well.
        m_characterController.radius = 0.2f;
        m_characterController.height = 0.2f;
        m_characterController.center = new Vector3(m_characterController.center.x, 0.20f, m_characterController.center.z);
    }

    private void ResetCollider()
    {
        m_characterController.radius = m_initColliderRadius;
        m_characterController.height = m_initColliderHeight;
        m_characterController.center = m_initColliderCenter;
    }

    public bool IsPlayerFrozen()
    {
        return m_isPlayerFrozen;
    }

    public void FreezePlayer(bool freeze)
    {
        m_isPlayerFrozen = freeze;
    }

    /// <summary>
    /// Current player speed. Might be used to approximately guess current player state
    /// </summary>
    /// <returns></returns>
    public float CurrentSpeed()
    {
        return m_Speed;
    }

    /// <summary>
    /// This procedure allows to externally change the game play mode at runtime, by switching between Orbit and Zenit camera. 
    /// Used mostly for developed mode. 
    /// It is to meant to be called only by the camera script
    /// TODO: consider if keeping both modes when deploying application, or finally remove one of them
    /// </summary>
    /// <param name="activateOrbitCamera"></param>
    public void UsingOrbitCamera(bool activateOrbitCamera)
    {
        m_isOrbitCamera = activateOrbitCamera;
    }

    public void ReadInput(bool readInput)
    {
        m_ReadInput = readInput;
    }
}
