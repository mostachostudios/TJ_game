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

    [Tooltip("A Render texture needed to temporary store a camera snapshot when the game is paused")] 
    [SerializeField] RenderTexture m_RenderTexture;
    [Tooltip("A post process effect applied when the menu window pops up")] 
    [SerializeField] PostProcessProfile m_PostProcessProfile;


    private GameObject m_MainCamera;

    private bool m_allow_pause = false;
    private bool m_paused = true; // switch values for showing menu (game is paused) and closing menu (game is playing) 

    void Awake()
    {
        m_MainCamera = GameObject.FindWithTag("MainCamera");

        m_Menu.layer = LayerMask.NameToLayer("PostProcessingMenu");

        PostProcessVolume m_MenuPostProcessVolume = m_Menu.AddComponent<PostProcessVolume>();
        m_MenuPostProcessVolume.isGlobal = true;
        m_MenuPostProcessVolume.profile = m_PostProcessProfile;
    }

    void Start()
    {
        SetPause();
    }

    void Update()
    {
        if (m_allow_pause) // Prevents user from pressing escape before first exec play
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
            // Render current frame to a texture linked to camera so it gets blended with Menu settings 
            m_Menu.SetActive(true);
            RenderMenuBackground();
            m_World.SetActive(false);
        }
        else
        {
            m_World.SetActive(true);
            m_Menu.SetActive(false);
        }
    }

    void RenderMenuBackground()
    {
        var camera = m_MainCamera.GetComponent<Camera>();
        camera.targetTexture = m_RenderTexture;
        
        var postProcessLayer = m_MainCamera.GetComponent<PostProcessLayer>();
        var currentLayer = postProcessLayer.volumeLayer.value;
        postProcessLayer.volumeLayer.value = LayerMask.GetMask("PostProcessingWorld","PostProcessingMenu");
        camera.Render();
        camera.targetTexture = null;
        postProcessLayer.volumeLayer.value = currentLayer;
    }

    public void PauseGame()
    {
        m_paused = true;

        m_Menu.SetActive(true);
        RenderMenuBackground();
        m_World.SetActive(false);
    }

    public void ResumeGame()
    {
        m_paused = false;
        m_World.SetActive(true);
        m_Menu.SetActive(false);
    }

    /// <summary>
    /// Allows the game to be paused and resumed each time the user presses the "pause button"
    /// </summary>
    public void AllowPauseGame(bool value)
    {
        m_allow_pause = value;
    }

}
