//https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html
//https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
//https://forum.unity.com/threads/detect-when-scene-has-fully-loaded.532558/

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class Script_GameController : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] GameObject m_EventSystem;
    [SerializeField] GameObject m_UI;
    [SerializeField] GameObject m_Menu;
    [SerializeField] public SoundManager m_soundManager;

    [Header("Audio")]
    [Tooltip("Audio clip to be played when player wins the game")]
    [SerializeField] AudioClip m_AudioWin;
    [Tooltip("Audio clip to be played when player passes to a next level")]
    [SerializeField] AudioClip m_AudioNextLevel;
    [Tooltip("Audio clip to be played when player loses the game")]
    [SerializeField] AudioClip m_AudioLose;
    private AudioSource m_AudioSource;
    private AudioSource m_BackgroundMusicAudioSource;

    private bool m_allow_pause = false;
    private bool m_paused = true; // switch values for showing menu (game is paused) and closing menu (game is playing) 

    private GameObject m_World;

    private Script_MenuController m_Script_MenuController;
    private Script_UIController m_Script_UIController;
    private Script_PauseController m_Script_PauseController;

    private int numLevels;
    private int currentLevel;

    public enum EndOption
    {
        Win, // Last level is completed
        NextLevel, // Current level is completed (excluding last level)
        Lose // Player lose in current level
    }

    void Awake()
    {
        m_EventSystem.SetActive(true);
        m_UI.SetActive(true);
        m_Menu.SetActive(true);

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(m_EventSystem);
        DontDestroyOnLoad(m_UI);
        DontDestroyOnLoad(m_Menu);

        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.playOnAwake = false;
        m_AudioSource.volume = 0.8f;

        m_BackgroundMusicAudioSource = gameObject.AddComponent<AudioSource>();
        m_BackgroundMusicAudioSource.playOnAwake = false;
        m_soundManager.m_musicAudioSource = m_BackgroundMusicAudioSource;
        m_soundManager.enabled = true;

        m_Script_MenuController = m_Menu.GetComponent<Script_MenuController>();
        m_Script_MenuController.Init(this);

        m_Script_UIController = m_UI.GetComponent<Script_UIController>();

        m_Script_PauseController = ScriptableObject.CreateInstance<Script_PauseController>();

        numLevels = SceneManager.sceneCountInBuildSettings;

        // currentLevel = FindObjectOfType<Script_Starter>().StartSceneIndex(); // It won't work this way
        currentLevel = GameObject.FindWithTag("Starter").GetComponent<Script_Starter>().StartSceneIndex();

        Debug.Log("Starting from level: " + currentLevel);
        if(currentLevel <= 0)
        {
            currentLevel = 1; // Scene 0 is initial start-up Scene with no world in it
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        RestartLevel(true);
    }

    void Start()
    {
        PauseGame();
    }

    void Update()
    {
        if (m_allow_pause)
        {
            if(m_Script_UIController.IsInputNameShowing())
            {
                return;
            }

            KeyCode pauseMenuKey;
#if UNITY_EDITOR
            pauseMenuKey = KeyCode.M;
#else
            pauseMenuKey = KeyCode.Escape;
#endif

            if (Input.GetKeyUp(pauseMenuKey))
            {
                m_paused = !m_paused;
                SetPauseResume();
            }
            // TODO TO BE REMOVED ONCE DEVELOPMENT IS FINISHED OR KEEP IF ENABLING A CHEATING MODE
            else if (Debug.isDebugBuild && (Input.GetKeyUp(KeyCode.KeypadPlus) || Input.GetKeyUp(KeyCode.Plus))) 
            {
                currentLevel++;
                if(currentLevel == numLevels)
                {
                    currentLevel = 1;
                }
                RestartLevel();
            }
            else if (Debug.isDebugBuild && (Input.GetKeyUp(KeyCode.KeypadMinus) || Input.GetKeyUp(KeyCode.Minus)))
            {
                currentLevel--;
                if (currentLevel == 0)
                {
                    currentLevel = numLevels - 1;
                }
                RestartLevel();
            }
        }
    }

    void SetPauseResume()
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

    public void PauseGame(bool showMenu = true)
    {
        m_paused = true;
        m_Script_PauseController.PauseGame(showMenu);
    }

    public void ResumeGame(bool eraseText = true, bool firstExecution = false)
    {
        m_paused = false;
        m_Script_PauseController.ResumeGame();
        if (eraseText)
        {
            m_Script_UIController.EraseTextMessage();
        }

        if(firstExecution)
        {
            m_soundManager.ChangeMode(currentLevel == 1 ? SoundManager.Mode.MANUAL : SoundManager.Mode.GAMEPLAY, true, 0.5f);
        }
    }

    /// <summary>
    /// Allows the game to be paused and resumed each time the user presses the "pause button"
    /// </summary>
    public void AllowPauseGame(bool value)
    {
        m_allow_pause = value;
    }

    public void DisplayCountdown(string text)
    {
        m_Script_UIController.SetTextCountdown(text);
    }

    public void ShowCountdown(bool show)
    {
        m_Script_UIController.ShowCountdown(show);
    }

    public void DisplayMessage(string text, float time, bool isTip = false)
    {
        m_Script_UIController.SetTextMessage(text, isTip);
        m_Script_UIController.EraseTextMessage(time);
    }

    public void RestartLevel(bool firstExec = false)
    {
        m_allow_pause = true;

        m_AudioSource.Stop();

        m_UI.SetActive(true);
        m_Menu.SetActive(true);
       
        m_Script_UIController.ClearUI();

        if (!firstExec)
        {
            m_Script_MenuController.ResetMenu();
            m_soundManager.ChangeMode(SoundManager.Mode.GAMEPLAY, true, 1.0f);
        }

        SceneManager.LoadScene(currentLevel);    
    }

    public void RestartGame()
    {
        currentLevel = 1;
        RestartLevel();
    }

    public void GoNextLevel()
    {
        currentLevel++;
        if (currentLevel >= numLevels)
        {
            EndLevel(EndOption.Win);
        }
        else
        {
            EndLevel(EndOption.NextLevel);
        }
    }

    public void EndLevel(EndOption endOption, string localizedText = null)
    {
        m_allow_pause = false;

        switch (endOption)
        {
            case EndOption.Win:
                m_Script_MenuController.SetInfoMessage(localizedText == null ? Script_MenuController.Mode.FINAL_WIN : Script_MenuController.Mode.CUSTOM, localizedText);
                m_AudioSource.clip = m_AudioWin;
                break;
            case EndOption.NextLevel:
                m_Script_MenuController.SetInfoMessage(localizedText == null ? Script_MenuController.Mode.WIN : Script_MenuController.Mode.CUSTOM, localizedText);
                m_AudioSource.clip = m_AudioNextLevel;
                break;
            case EndOption.Lose:
                m_Script_MenuController.SetInfoMessage(localizedText == null ? Script_MenuController.Mode.LOSE : Script_MenuController.Mode.CUSTOM, localizedText);
                m_AudioSource.clip = m_AudioLose;
                break;
        }

        
        m_Script_MenuController.SetEndingLevelWindow(endOption);
        PauseGame();

        m_AudioSource.Play();
    }

    void ReloadWorld()
    {
        m_World = GameObject.FindWithTag("World");
        Debug.Log("current scene: " + SceneManager.GetActiveScene().name);
        if(m_World == null)
        {
            Debug.Log("Problem: World null");
        }
        else
        {
          //  Debug.Log("World OK");
        }
        m_Script_PauseController.Init(m_UI, m_Menu, m_World);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);

        if (scene.buildIndex == 0)
        {
            m_soundManager.ChangeMode(SoundManager.Mode.MENU, true, 0);
        }

        if (scene.buildIndex != 0) // Avoid loading world in start-up scene
        {
            ReloadWorld();
            SetPauseResume();
        }
        

    }
}
