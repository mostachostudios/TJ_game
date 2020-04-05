//https://www.youtube.com/watch?v=urNrY7FgMao
//https://www.youtube.com/watch?v=xcn7hz7J7sI

// Currently not in use. Only kept for suggested reviews

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_OrbitCameraAlternative : MonoBehaviour
{
    private Transform m_playerTransform;
    private Vector3 m_cameraOffset;
    
    [Range(0.01f, 1.0f)]
    [SerializeField] float m_SmoothFactor = 0.5f;
    [SerializeField] bool m_LookAtPlayer = true;

    [SerializeField] bool m_RotateAroundPlayer = true;
    [SerializeField] float m_RotationSpeed = 5.0f;


    void Awake()
    {
        m_playerTransform = GameObject.FindWithTag("Player").transform;
        m_cameraOffset = transform.position - m_playerTransform.position;
    }

    void LateUpdate()
    {
        if (m_RotateAroundPlayer)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * m_RotationSpeed, Vector3.up);
            m_cameraOffset = camTurnAngle * m_cameraOffset;
        }

        Vector3 newPos = m_playerTransform.position + m_cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, m_SmoothFactor);

        if (m_LookAtPlayer || m_RotateAroundPlayer)
        {
            transform.LookAt(m_playerTransform);
        }
    }
}
