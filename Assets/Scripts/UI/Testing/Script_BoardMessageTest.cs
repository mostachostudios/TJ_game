using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BoardMessageTest : MonoBehaviour
{
    Script_CanvasMenu m_script_CanvasMenu;

    void Awake()
    {
        GameObject gameObject = GameObject.FindWithTag("CanvasMenu");
        m_script_CanvasMenu = gameObject.GetComponent<Script_CanvasMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("m"))
        {
            m_script_CanvasMenu.ShowBoardMessage("Message sent from gameplay");
        }
    }
}
