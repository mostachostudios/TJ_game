// https://www.youtube.com/watch?v=BEIaakl9vJE
// https://answers.unity.com/questions/17566/how-can-i-make-my-player-a-charactercontroller-pus.html
// https://answers.unity.com/questions/578443/jumping-with-character-controller.html
// https://docs.unity3d.com/es/530/ScriptReference/CharacterController.Move.html

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
    [SerializeField] float m_jumpSpeed = 1.8f;

    [Header("Inactive time")]
    [SerializeField] float m_TimeFalling = 2f;
    [SerializeField] float m_TimeFallingDown = 5f;
    [SerializeField] float m_TimeTerrified = 8f;

    private CharacterController m_characterController;
    private Animator m_animator;

    private Vector3 m_movement;
    private float m_ySpeed;
    private float m_Speed;
    private List<string> m_AnimationStates;
    // TODO save current's player state somewhere (so it can be checked from outside)

    private bool m_isPushing = false;
    private bool m_allowJump = false;

    private bool m_isPlayerFrozen = false;

    private float m_initColliderRadius;
    private float m_initColliderHeight;
    private Vector3 m_initColliderCenter;

    private bool m_isOrbitCamera = true;
    void Awake()
    {
        m_characterController = gameObject.GetComponent<CharacterController>();
        m_animator = gameObject.GetComponent<Animator>();

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

        SetState("isIdle");

        m_ySpeed = 0.0f;
    }

    void Update()
    {
        if (!m_isPlayerFrozen) // If m_isPlayerFrozen is true then it is either falling or terrified (so it will ignore player's input)
        {
            SetAnimation();
            MoveCharacther();
        }
    }

    private void SetAnimation()
    {
        // Checking if isMoving this way might introduce a bit delay/lag since GetAxis returns != 0 even after a short time the user has stopping pressing the keyboard 
        //bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.0001f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.0001f;
        bool isMoving = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
        || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isStealth = Input.GetKey(KeyCode.Tab);
        bool isCrawling = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool isCrouch = Input.GetKey(KeyCode.LeftAlt);

        m_Speed = m_walkSpeed; // default speed value

        // Restore initial collider values in case they were modified
        m_characterController.radius = m_initColliderRadius;
        m_characterController.height = m_initColliderHeight;
        m_characterController.center = m_initColliderCenter;

        // TODO FIX problem with some transitions (push and run, etc) // Use ignore buffer if key was pressed before, or search for a similar way
        // Sets animantion
        if (isMoving) // Character is moving
        {
            if (m_isPushing)
            {
                SetState("isPushing");
                m_Speed = m_pushSpeed;
            }
            else if (isStealth)
            {
                SetState("isStealth");
                m_Speed = m_stealthSpeed;
            }
            else if (isCrawling)
            {
                SetState("isCrawling");
                SetCrawl();
                m_Speed = m_crawlSpeed;
            }
            else if (isCrouch)
            {
                SetState("isCrouchWalking");
                SetCrouch();
                m_Speed = m_crouchSpeed;
            }
            else
            {
                if (m_isOrbitCamera && Input.GetAxis("Horizontal") > 0f) // Right dodge
                { 
                    SetState("isDodgingRight");
                    m_Speed = m_dodgeSpeed;
                }
                else if (m_isOrbitCamera && Input.GetAxis("Horizontal") < 0f) // Left dodge
                {
                    SetState("isDodgingLeft");
                    m_Speed = m_dodgeSpeed;
                }
                else if (m_isOrbitCamera && Input.GetAxis("Vertical") < 0f) // Backward Walking
                {
                    SetState("isBackwardWalking");
                    m_Speed = m_backwardWalkSpeed;
                }
                else if (isRunning)
                {
                    SetState("isRunning");
                    CheckJumping();
                    m_Speed = m_runSpeed;
                }
                else // Walking forward
                {
                    SetState("isWalking");
                    CheckJumping();
                }
            }
        }
        else // Still
        {
            if (isCrouch || isCrawling)
            {
                SetState("isCrouchIdle");
                SetCrouch();
            }
            else
            {
                SetState("isIdle");
                CheckJumping();
            }
        }
    }

    private void SetState(string state)
    {
        foreach (string entry in m_AnimationStates)
        {
            m_animator.SetBool(entry, false);
        }
        m_animator.SetBool(state, true);
    }

    private void CheckJumping()
    {
        //TODO CHECK NO funciona si se presiona arrow up y left a la vez (pero si funciona ok con A + W y también con NumPad!!!!) 
        if (Input.GetButtonDown("Jump"))
        {
            m_animator.SetTrigger("isJumping");
            m_allowJump = true;
        }
    }

    private void SetCrouch()
    {
        //This values are calculated based on initial collider properties. If changed in editor, then these should be recalculated as well.
        m_characterController.radius = 0.25f;
        m_characterController.height = 1.0f;
        m_characterController.center = new Vector3(m_characterController.center.x, 0.5f, m_characterController.center.z);
    }

    private void SetCrawl()
    {
        //This values are calculated based on initial collider properties. If changed in editor, then these should be recalculated as well.
        m_characterController.radius = 0.2f;
        m_characterController.height = 0.2f;
        m_characterController.center = new Vector3(m_characterController.center.x, 0.20f, m_characterController.center.z);
    }

    private void MoveCharacther()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (m_isOrbitCamera)
        {
            m_movement = Vector3.forward * vertical + Vector3.right * horizontal;

            m_movement = transform.TransformDirection(m_movement);
            m_movement *= m_Speed;

            if (m_characterController.isGrounded)
            {
                m_ySpeed = 0.0f;
                if (m_allowJump) // Jumping is only active depending animation state (forward walk, run and idle)
                {
                    m_ySpeed = m_jumpSpeed;
                    m_allowJump = false;
                }
            }

            m_ySpeed += m_gravity * Time.deltaTime;
            m_movement.y = m_ySpeed;

            m_characterController.Move(m_movement * Time.deltaTime);
        }
        else
        {
            m_movement = new Vector3(horizontal, 0.0f, vertical);
            m_movement *= m_Speed;

            if (m_characterController.isGrounded)
            {
                m_ySpeed = 0.0f;
                if (m_allowJump) // Jumping is only active depending animation state (forward walk, run and idle)
                {
                    m_ySpeed = m_jumpSpeed;
                    m_allowJump = false;
                }
            }

            transform.LookAt(transform.position + m_movement);

            m_ySpeed += m_gravity * Time.deltaTime;
            m_movement.y = m_ySpeed;

            m_characterController.Move(m_movement * Time.deltaTime);
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

    public void SetFalling()
    {
        m_isPlayerFrozen = true;
        SetState("isFalling");
        StartCoroutine(WaitUntilNotFrozen(m_TimeFalling));
    }

    public void SetFallingDown()
    {
        m_isPlayerFrozen = true;
        SetState("isFallingDown");
        StartCoroutine(WaitUntilNotFrozen(m_TimeFallingDown));
    }

    public void SetTerrified()
    {
        m_isPlayerFrozen = true;
        SetState("isTerrified");
        StartCoroutine(WaitUntilNotFrozen(m_TimeTerrified));
    }

    IEnumerator WaitUntilNotFrozen(float timeFrozen)
    {
        yield return new WaitForSeconds(timeFrozen);
        m_isPlayerFrozen = false;
    }

    public bool IsPlayerFrozen()
    {
        return m_isPlayerFrozen;
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
}
