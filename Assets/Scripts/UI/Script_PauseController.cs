﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Script_PauseController : MonoBehaviour
{
    [Tooltip("A refence to the World gameObject in the scene")]
    [SerializeField] GameObject m_World;
    [Tooltip("A refence to the Menu gameObject in the scene")]
    [SerializeField] GameObject m_Menu;
    [Tooltip("A refence to the UI gameObject in the scene")]
    [SerializeField] GameObject m_UI;
    
    private bool m_allow_pause = false;
    private bool m_paused = true; // switch values for showing menu (game is paused) and closing menu (game is playing) 

    private List<MonoBehaviour> m_WorldMonoBehaviours;
    private AudioSource[] m_AudioSources;

    void Awake()
    {
        m_WorldMonoBehaviours = new List<MonoBehaviour>();
        var WorldMonoBehaviours = m_World.GetComponentsInChildren<MonoBehaviour>();

        foreach (var monoBehaviour in WorldMonoBehaviours)
        {
            //Ignore PostProcessLayers so they won't be deactivated
            if (!monoBehaviour.GetType().FullName.Equals("UnityEngine.Rendering.PostProcessing.PostProcessLayer"))
            {
                m_WorldMonoBehaviours.Add(monoBehaviour);
            }
        }

        m_AudioSources = m_World.GetComponentsInChildren<AudioSource>();
    }

    void Start()
    {
        SetPause();
    }

    void Update()
    {
        if (m_allow_pause)
        {
            //TODO change this by using conditional compilation such as #if UNITY_EDITOR #endif or another one for better performance 
            if ((Debug.isDebugBuild && Input.GetKeyUp(KeyCode.M)) || (!Debug.isDebugBuild && Input.GetKeyUp(KeyCode.Escape)))
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
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // No longer used
    /*
    void RenderMenuBackground()
    {       
        var camera = m_ActiveCamera.GetComponent<Camera>();

        camera.targetTexture = m_RenderTexture;

        //var postProcessLayer = m_ActiveCamera.GetComponent<PostProcessLayer>();
        var postProcessLayer = camera.GetComponent<PostProcessLayer>();
        var currentLayer = postProcessLayer.volumeLayer.value;
        postProcessLayer.volumeLayer.value = LayerMask.GetMask("PostProcessingWorld", "PostProcessingMenu");
        camera.Render();
        camera.targetTexture = null;
        postProcessLayer.volumeLayer.value = currentLayer;

    }
    */

    void SetActiveScripts(bool active)
    {
        foreach (var monoBehaviour in m_WorldMonoBehaviours)
        {
            monoBehaviour.enabled = active;          
        }
        if (!active)
        {
            foreach (var audioSource in m_AudioSources)
            {
                audioSource.Pause();
            }
        }
    }

    public void PauseGame()
    {
        m_paused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetActiveScripts(false);
        m_UI.SetActive(false);

        m_Menu.SetActive(true);
    }

    public void ResumeGame()
    {
        m_paused = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        m_Menu.SetActive(false);

        SetActiveScripts(true);
        m_UI.SetActive(true);
    }

    /// <summary>
    /// Allows the game to be paused and resumed each time the user presses the "pause button"
    /// </summary>
    public void AllowPauseGame(bool value)
    {
        m_allow_pause = value;
    }

}
