using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Pause : MonoBehaviour
{
    [SerializeField] GameObject m_World;
    [SerializeField] GameObject m_CanvasMenu;

    private bool m_executed = false;
    private bool m_paused = true; // switch values for showing menu (game is paused) and closing menu (game is playing) 

    void Awake()
    {
        SetPause();
    }

    void Update()
    {
        if (m_executed) // Prevents user from pressing escape before first exec play
        {
            if (Input.GetKeyUp("escape")) 
            {
                m_paused = !m_paused;
                SetPause();
            }
        }
    }

    void SetPause()
    {
        if (m_paused)
        {
            m_CanvasMenu.SetActive(true);
            m_World.SetActive(false);
        }
        else
        {
            m_CanvasMenu.SetActive(false);
            m_World.SetActive(true);
        }
    }

    public void PauseGame()
    {
        m_paused = true;
        m_CanvasMenu.SetActive(true);
        m_World.SetActive(false);
    }

    public void ResumeGame()
    {
        m_paused = false;
        m_CanvasMenu.SetActive(false);
        m_World.SetActive(true);
    }

    /// <summary>
    /// Only takes effect the first time the game is executed/started
    /// </summary>
    public void Execute()
    {
        if (!m_executed)
        {
            m_executed = true;
        }
    }
}
