using System.Collections;
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

    [Tooltip("A render texture needed to temporary store a camera snapshot when the game is paused")]
    [SerializeField] RenderTexture m_RenderTexture;
    [Tooltip("A post process effect applied when the menu window pops up")]
    [SerializeField] PostProcessProfile m_PostProcessProfile;


    private GameObject m_MainCamera;

    private bool m_allow_pause = false;
    private bool m_paused = true; // switch values for showing menu (game is paused) and closing menu (game is playing) 

    private MonoBehaviour[] m_WorldMonoBehaviours;

    void Awake()
    {
        m_WorldMonoBehaviours = m_World.GetComponentsInChildren<MonoBehaviour>();

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

    void RenderMenuBackground()
    {
        var camera = m_MainCamera.GetComponent<Camera>();
        camera.targetTexture = m_RenderTexture;

        var postProcessLayer = m_MainCamera.GetComponent<PostProcessLayer>();
        var currentLayer = postProcessLayer.volumeLayer.value;
        postProcessLayer.volumeLayer.value = LayerMask.GetMask("PostProcessingWorld", "PostProcessingMenu");
        camera.Render();
        camera.targetTexture = null;
        postProcessLayer.volumeLayer.value = currentLayer;
    }

    void SetActiveScripts(bool active)
    {
        foreach (var monoBehaviour in m_WorldMonoBehaviours)
        {
            monoBehaviour.enabled = active;
        }
    }

    public void PauseGame()
    {
        m_paused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        m_Menu.SetActive(true);
        RenderMenuBackground();
        SetActiveScripts(false);
        m_MainCamera.SetActive(false);
        m_UI.SetActive(false);
    }

    public void ResumeGame()
    {
        m_paused = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetActiveScripts(true);
        m_MainCamera.SetActive(true);
        m_UI.SetActive(true);
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
