using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CameraSwitcher : MonoBehaviour
{
    private GameObject m_MainCamera;
    private GameObject m_FreeCamera;

    private GameObject m_ActiveCamera;

    void Awake()
    {
        m_MainCamera = GameObject.FindWithTag("MainCamera");
        m_FreeCamera = GameObject.FindWithTag("FreeCamera");

        if(m_FreeCamera == null) // If no Free Camera in Scene (Assuming there is always a MainCamera
        {
            m_FreeCamera = m_MainCamera;
        }

        SwitchToMainCamera();
    }

    //TODO Update method to be removed. Only for debug purpose
    void Update()
    {
        if (Debug.isDebugBuild && Input.GetKeyUp(KeyCode.C))
        {
            if(m_ActiveCamera == m_MainCamera)
            {
                SwitchToFreeCamera();
            }
            else
            {
                SwitchToMainCamera();
            }
        }
    }

    public GameObject GetActiveCamera()
    {
        return m_ActiveCamera;
    }

    public void SwitchToMainCamera()
    {
        m_FreeCamera.SetActive(false);
        m_MainCamera.SetActive(true);
        m_ActiveCamera = m_MainCamera;
    }

    public void SwitchToFreeCamera()
    {
        m_MainCamera.SetActive(false);
        m_FreeCamera.SetActive(true);
        m_ActiveCamera = m_FreeCamera;
    }
}
