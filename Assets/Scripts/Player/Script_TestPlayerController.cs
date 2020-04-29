using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_TestPlayerController : MonoBehaviour
{
    private Script_PlayerController m_Script_PlayerController;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        m_Script_PlayerController = player.GetComponent<Script_PlayerController>();
    }
    
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKey(KeyCode.F1))
            {
                m_Script_PlayerController.MoveWalk();
            }
            else if (Input.GetKey(KeyCode.F2))
            {
                m_Script_PlayerController.MoveRun();
            }
            else if (Input.GetKey(KeyCode.F3))
            {
                m_Script_PlayerController.MoveStealth();
            }
            else if (Input.GetKey(KeyCode.F4))
            {
                m_Script_PlayerController.MovePush();
            }
            else if (Input.GetKey(KeyCode.F5))
            {
                m_Script_PlayerController.MoveCrawl();
            }
            else if (Input.GetKey(KeyCode.F6))
            {
                m_Script_PlayerController.MoveCrouch();
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                m_Script_PlayerController.Jump();
            }
            else if (Input.GetKey(KeyCode.F8))
            {
                m_Script_PlayerController.SetCrouch();
            }
            else if (Input.GetKey(KeyCode.F9))
            {
                m_Script_PlayerController.SetIdle();
            }
            else if (Input.GetKeyDown(KeyCode.F12))
            {
                m_Script_PlayerController.SetTerrified();
            }
        }
    }
}
