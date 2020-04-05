using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_TestFrozen : MonoBehaviour
{
    private Script_PlayerController m_Script_PlayerController;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        m_Script_PlayerController = player.GetComponent<Script_PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            m_Script_PlayerController.SetFalling();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            m_Script_PlayerController.SetFallingDown();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            m_Script_PlayerController.SetTerrified();
        }
    }
}
