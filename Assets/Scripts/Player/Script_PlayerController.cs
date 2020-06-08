// https://www.youtube.com/watch?v=BEIaakl9vJE
// https://answers.unity.com/questions/17566/how-can-i-make-my-player-a-charactercontroller-pus.html
// https://answers.unity.com/questions/578443/jumping-with-character-controller.html
// https://docs.unity3d.com/es/530/ScriptReference/CharacterController.Move.html
// https://docs.unity3d.com/Manual/class-CharacterController.html

using System.Collections;
using UnityEngine;

public class Script_PlayerController : MonoBehaviour
{    
    [Header("Speed values")]
    [SerializeField] float m_walkSpeed = 1f;
    [SerializeField] float m_runSpeed = 2f;
    [SerializeField] float m_stealthSpeed = 0.15f;
    [SerializeField] float m_crawlSpeed = 0.25f;
    [SerializeField] float m_crouchSpeed = 0.4f;
    [SerializeField] float m_dodgeSpeed = 1.3f;
    [SerializeField] float m_backwardWalkSpeed = 0.3f;
    [SerializeField] float m_pushSpeed = 0.6f;
    [SerializeField] float m_jumpSpeed = 2.5f;

    [Header("Settings")]
    [SerializeField] float m_gravity = -9.81f;
    [SerializeField] float m_pushPower = 20f;
    [SerializeField] float m_timeNextJump = 0.8f;
    [SerializeField] float m_timeNextPickUp = 0.8f;
    [SerializeField] float m_timeNextKick = 0.8f;

    [Header("Frozen time lapse")]
    [SerializeField] float m_TimeFalling = 2f;
    [SerializeField] float m_TimeFallingDown = 5f;
    [SerializeField] float m_TimeTerrified = 8f;

    [Header("SFX")]
    [SerializeField] AudioClip m_StepFootClip;
    [SerializeField] AudioClip m_StepFootRunClip;

    [Header("Restrictions")]
    [SerializeField] public bool allowMove = true;
    [SerializeField] public bool allowRun = true;
    [SerializeField] public bool allowJump = true;
    [SerializeField] public bool allowCrouch = true;
    [SerializeField] public bool allowStealth = true;
    [SerializeField] public bool allowCrawl = true;

    private AudioSource m_AudioSource;

    private CharacterController m_characterController;
    private Animator m_animator;

    private float m_ySpeed;
    private float m_Speed;

    private bool m_isPushing = false;
    private bool m_isJumping = false;
    private bool m_canJump = true;
    private bool m_canPickUp = true;
    private bool m_canKick = true;

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

        Utils.SetAnimatorParameterByName(m_animator, "isIdle");

        m_ySpeed = 0.0f;
        m_Speed = 0.0f;
    }

    void Update() // TODO check if should update movement in LateUpdate
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
        bool isMoving = allowMove && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
        || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
        bool isRunning = allowRun && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isStealth = allowStealth && Input.GetKey(KeyCode.Mouse1);
        bool isCrawling = allowCrawl && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.Space);
        bool isCrouch = allowCrouch && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && !Input.GetKey(KeyCode.Space);

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
                if (m_isOrbitCamera && horizontal > 0f) // Not available in Zenith Camera
                {
                    MoveDodgeRight(direction);
                }
                else if (m_isOrbitCamera && horizontal < 0f) // Not available in Zenith Camera
                {
                    MoveDodgeLeft(direction);
                }
                else if (m_isOrbitCamera && vertical < 0f) // Not available in Zenith Camera
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

            movement.Normalize();

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
        Utils.SetAnimatorParameterByName(m_animator, "isPushing");
        m_Speed = m_pushSpeed;
        MoveCharacther(direction);
    }

    public void MoveStealth(Vector3 direction = new Vector3())
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isStealth");
        m_Speed = m_stealthSpeed;
        MoveCharacther(direction);
    }

    public void MoveCrawl(Vector3 direction = new Vector3())
    {
        SetCrawlCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isCrawling");
        m_Speed = m_crawlSpeed;
        MoveCharacther(direction);
    }

    public void MoveCrouch(Vector3 direction = new Vector3())
    {
        SetCrouchCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isCrouchWalking");
        m_Speed = m_crouchSpeed;
        MoveCharacther(direction);
    }

    private void MoveDodgeLeft(Vector3 direction)
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isDodgingLeft");
        m_Speed = m_dodgeSpeed;
        MoveCharacther(direction);
    }

    private void MoveDodgeRight(Vector3 direction)
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isDodgingRight");
        m_Speed = m_dodgeSpeed;
        MoveCharacther(direction);
    }

    private void MoveBackwards(Vector3 direction)
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isBackwardWalking");
        m_Speed = m_backwardWalkSpeed;
        MoveCharacther(direction);
    }

    public void MoveRun(Vector3 direction = new Vector3())
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isRunning");
        CheckJumping();
        m_Speed = m_runSpeed;
        MoveCharacther(direction);
    }

    public void MoveWalk(Vector3 direction = new Vector3())
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isWalking");
        CheckJumping();
        m_Speed = m_walkSpeed;
        MoveCharacther(direction);
    }

    public void SetCrouch()
    {
        SetCrouchCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isCrouchIdle");
        m_Speed = 0f;
        MoveCharacther(Vector3.zero);
    }

    public void SetIdle()
    {
        ResetCollider();
        Utils.SetAnimatorParameterByName(m_animator, "isIdle");
        CheckJumping();
        m_Speed = 0f;
        MoveCharacther(Vector3.zero);
    }

    public void Jump()
    {
        if (m_canJump && m_characterController.isGrounded)
        {
            Utils.SetAnimatorParameterByName(m_animator, "isJumping");
            m_isJumping = true;
            m_canJump = false;
            StartCoroutine(WaitNextJump());
        }
    }

    private void CheckJumping()
    {
        //NOTA: No funciona si se presiona arrow up y left a la vez (pero si funciona ok con A + W y también con NumPad!!!!) 
        // Si en lugar de Space, se usa otra tecla, como F7, entonces sí que va bien. (Parece ser un Bug de Unity o C#)
        // Es decir, al precionar LeftArrow + UpArrow + Space a la vez, la expresion devuelta es false!
        if (allowJump && Input.GetKey(KeyCode.Space)) 
        {
            Jump();
        }
    }

    public void PickUp()
    {
        if (m_canPickUp && m_characterController.isGrounded)
        {
            Utils.SetAnimatorParameterByName(m_animator, "isPicking");
            m_canPickUp = false;
            StartCoroutine(WaitNextPickUp());
        }
    }

    public void Kick()
    {
        if (m_canKick && m_characterController.isGrounded)
        {
            Utils.SetAnimatorParameterByName(m_animator, "isKicking");
            m_canKick = false;
            StartCoroutine(WaitNextKick());
        }
    }

    public float SetFalling()
    {
        m_isPlayerFrozen = true;
        Utils.SetAnimatorParameterByName(m_animator, "isFalling");
        m_Speed = 0f;
        StartCoroutine(WaitUntilNotFrozen(m_TimeFalling));
        return m_TimeFalling;
    }

    public float SetFallingDown()
    {
        m_isPlayerFrozen = true;
        Utils.SetAnimatorParameterByName(m_animator, "isFallingDown");
        m_Speed = 0f;
        StartCoroutine(WaitUntilNotFrozen(m_TimeFallingDown));
        return m_TimeFallingDown;
    }

    public float SetTerrified()
    {
        m_isPlayerFrozen = true;
        Utils.SetAnimatorParameterByName(m_animator, "isTerrified");
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

    IEnumerator WaitNextJump()
    {
        yield return new WaitForSeconds(m_timeNextJump);
        m_canJump = true;
        yield return null;
    }

    IEnumerator WaitNextPickUp()
    {
        yield return new WaitForSeconds(m_timeNextPickUp);
        m_canPickUp = true;
        yield return null;
    }

    IEnumerator WaitNextKick()
    {
        yield return new WaitForSeconds(m_timeNextKick);
        m_canKick = true;
        yield return null;
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

    public bool IsStealth()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

}
