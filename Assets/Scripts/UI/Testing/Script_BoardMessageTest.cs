using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BoardMessageTest : MonoBehaviour
{
    Script_MenuController m_Script_MenuController;

    void Awake()
    {
        m_Script_MenuController = GameObject.FindWithTag("Menu").GetComponent<Script_MenuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            m_Script_MenuController.ShowBoardMessage("Message sent from gameplay");
        }
    }
}
