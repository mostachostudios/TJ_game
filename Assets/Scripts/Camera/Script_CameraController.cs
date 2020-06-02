//https://www.youtube.com/watch?v=7nxpDwnU0uU
//https://www.youtube.com/watch?v=wWyx7_cIxP8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CameraController : MonoBehaviour
{
    [SerializeField] float m_RotationSpeed = 1.7f;
    [SerializeField] float m_Distance = -2.5f;

    [SerializeField] float m_FlatDistance = 2.0f;
    private Script_PlayerController m_Script_PlayerController;

    private GameObject m_Player;

    private GameObject m_CameraTarget;
    private float m_mouseX = 0f;
    private float m_mouseY = 18f;

    [SerializeField] bool m_isInitialOrbit = true;
    [SerializeField] bool m_allowChangeCamera = true;

    private bool m_isOrbit;

    private bool m_ReadInput = true;

    private Transform Obstruction;

    void Awake()
    {
        m_Player = GameObject.FindWithTag("Player");
        m_Script_PlayerController = m_Player.GetComponent<Script_PlayerController>();
        m_CameraTarget = GameObject.FindWithTag("CameraTarget");
    }

    void Start()
    {
        Obstruction = m_CameraTarget.transform;
        if (m_isInitialOrbit)
        {
            SetOrbitCamera();
        }
        else
        {
            SetZenitCamera();
        }
    }

    void Update()
    {
        if (m_ReadInput && m_allowChangeCamera)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SetOrbitCamera();
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                SetZenitCamera();
            }
        }
    }

    void LateUpdate()
    {
        if (m_ReadInput)
        {
            if (m_isOrbit)
            {
                OrbitCameraControl();
            }
            else
            {
                ZenitCameraControl();
            }
            //ViewObstructed(); 
        }
    }

    void OrbitCameraControl()
    {
        m_mouseX += Input.GetAxis("Mouse X") * m_RotationSpeed;

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            m_mouseY -= Input.GetAxis("Mouse Y") * m_RotationSpeed;
            m_mouseY = Mathf.Clamp(m_mouseY, -5f, 55f);
        }

        transform.LookAt(m_CameraTarget.transform);

        if (Input.GetMouseButton(2))
        {
            m_CameraTarget.transform.rotation = Quaternion.Euler(m_mouseY, m_mouseX, 0f);
        }
        else
        {
            m_CameraTarget.transform.rotation = Quaternion.Euler(m_mouseY, m_mouseX, 0f);
            m_Player.transform.rotation = Quaternion.Euler(0f, m_mouseX, 0f);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            m_Distance += 0.15f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            m_Distance -= 0.15f;
        }
        m_Distance = Mathf.Clamp(m_Distance, -7f, -1.35f);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, m_Distance);
    }

    void ZenitCameraControl()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            m_FlatDistance += 0.25f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            m_FlatDistance -= 0.25f;
        }
        m_FlatDistance = Mathf.Clamp(m_FlatDistance, 1.25f, 5.75f);

        transform.position = m_Player.transform.position + new Vector3(0.0f, m_FlatDistance, -m_FlatDistance);
        transform.LookAt(m_Player.transform.position);
    }

    private void SetOrbitCamera()
    {
        m_isOrbit = true;
        m_Script_PlayerController.UsingOrbitCamera(m_isOrbit);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void SetZenitCamera()
    {
        m_isOrbit = false;
        m_Script_PlayerController.UsingOrbitCamera(m_isOrbit);
    }

    void OnEnable()
    {
        if (m_isOrbit)
        {
            SetOrbitCamera();
        }
        else
        {
            SetZenitCamera();
        }
    }

    public void ReadInput(bool readInput)
    {
        m_ReadInput = readInput;
    }

    //Experimental
    void ViewObstructed()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, m_CameraTarget.transform.position - transform.position, out hit, 4.5f))
        {

            //if (hit.collider.gameObject != m_Player)
            if (hit.collider.gameObject.tag == "Wall")
            {
                Obstruction = hit.transform;
                Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if (Vector3.Distance(Obstruction.position, transform.position) >= 3f && Vector3.Distance(transform.position, m_CameraTarget.transform.position) >= 1.5f)
                {
                    transform.Translate(Vector3.forward * 1f * Time.deltaTime);
                }
            }
            else
            {
                Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                if (Vector3.Distance(transform.position, m_CameraTarget.transform.position) < 4.5f)
                {
                    transform.Translate(Vector3.back * 1f * Time.deltaTime);
                }
            }
        }
    }

}
