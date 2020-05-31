using System.Collections.Generic;
using UnityEngine;

public class Script_PauseController : ScriptableObject
{
    private GameObject m_World;
    private GameObject m_Menu;
    private GameObject m_UI;    

    //private List<MonoBehaviour> m_WorldMonoBehaviours;
    private List<AudioSource> m_AudioSources;

    private Script_PlayerController m_Script_PlayerController;
    private Script_CameraController m_Script_CameraController;
    private Script_Countdown m_Script_Countdown;
    public void Init(GameObject UI, GameObject Menu, GameObject World)
    {
        m_UI = UI;
        m_Menu = Menu;
        m_World = World;

        //m_WorldMonoBehaviours = new List<MonoBehaviour>();
        //
        //var WorldMonoBehaviours = m_World.GetComponentsInChildren<MonoBehaviour>();
        //foreach (var monoBehaviour in WorldMonoBehaviours)
        //{
        //    //Ignore PostProcessLayers so they won't be deactivated
        //    if (!monoBehaviour.GetType().FullName.Equals("UnityEngine.Rendering.PostProcessing.PostProcessLayer"))
        //    {
        //        m_WorldMonoBehaviours.Add(monoBehaviour);
        //    }
        //}

        m_AudioSources = new List<AudioSource>(m_World.GetComponentsInChildren<AudioSource>());
        m_Script_PlayerController = FindObjectOfType<Script_PlayerController>();
        m_Script_CameraController = FindObjectOfType<Script_CameraController>();
        m_Script_Countdown = FindObjectOfType<Script_Countdown>();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetActiveScripts(false);
        m_UI.SetActive(false);
        m_Menu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetActiveScripts(true);
        m_UI.SetActive(true);
        m_Menu.SetActive(false);
    }

    void SetActiveScripts(bool active)
    {
        //foreach (var monoBehaviour in m_WorldMonoBehaviours)
        //{
        //    Debug.Log("name: " + monoBehaviour.name);
        //    monoBehaviour.enabled = active;
        //}

        m_Script_PlayerController.ReadInput(active);
        m_Script_CameraController.ReadInput(active);
        m_Script_Countdown.Freeze(!active);

        foreach (var audioSource in m_AudioSources)
        {
            if (active)
            {
                audioSource.UnPause();
            }
            else
            {
                audioSource.Pause();
            }
        }

    }
}
