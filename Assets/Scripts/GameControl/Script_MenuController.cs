using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Script_MenuController : MonoBehaviour
{
    [Header("Background Image Texture")]
    [Tooltip("Reference to Raw Image component")] [SerializeField] RawImage m_ImageBackground;
    [Tooltip("Reference to Raw Image component")] [SerializeField] RawImage m_BlackBackground;

    [Header("Menu Buttons")]
    [SerializeField] GameObject m_ButtonPlay;
    [SerializeField] GameObject m_ButtonNextLevel;
    [SerializeField] GameObject m_ButtonInfo;
    [SerializeField] GameObject m_ButtonInput;
    [SerializeField] GameObject m_ButtonCredits;
    [SerializeField] GameObject m_ButtonRestartLevel;
    [SerializeField] GameObject m_ButtonRestartGame;
    [SerializeField] GameObject m_ButtonQuit;

    [Header("Text Board")]
    [SerializeField] Text m_TextBoard;

    [Header("Effects")]
    [Tooltip("A post process effect applied when the menu window pops up (Game is paused).")]
    [SerializeField] PostProcessProfile m_PostProcessProfile;

    private Script_GameController m_Script_GameController;

    //TODO Handle the following board messages in a proper way 
    private string m_INFO = "Messages info";

    private string m_INPUT =    "ASWD or ARROW Buttons   -------------------   Movement\n" +
                                "SPACE BAR  --------------------------------------   Jump\n" +
                                "SHIFT  ---------------------------------------------   Run\n" +
                                "CTRL + Move  ------------------------------------  Crouch\n" +
                                "CTRL + SPACE + Move  -------------------------  Crawl\n" +
                                "RIGHT Mouse Button  ----------------------------  Stealth\n" +
                                "INTRO or LEFT Mouse Button + Move  --------  Push\n" +
                                "ESC   ------------------------------------------------  Pause and Resume Game";

    private string m_CREDITS = "                                      Mostacho Studios\n" +
                                "\n" +
                                "Twitter: @StudiosMostacho\n" +
                                "Itchio: mostachostudios\n" +
                                "Github: mostachostudios\n" +
                                "\n" +
                                "Made with Unity\n" +
                                "Thanks to all the 3D community for  providing some free wonderful assets. Resources are: (to be completed)";
    
    private bool m_firstExec = true;

    public void Init(Script_GameController script_GameController)
    {
        m_Script_GameController = script_GameController;

        PostProcessVolume m_MenuPostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
        m_MenuPostProcessVolume.isGlobal = true;
        m_MenuPostProcessVolume.profile = m_PostProcessProfile;

        m_ButtonPlay.GetComponent<Button>().onClick.AddListener(Play);
        m_ButtonNextLevel.GetComponent<Button>().onClick.AddListener(RestartLevel);

        m_ButtonInfo.GetComponent<Button>().onClick.AddListener(() => WriteBoardMessage(m_INFO));
        m_ButtonInput.GetComponent<Button>().onClick.AddListener(() => WriteBoardMessage(m_INPUT));
        m_ButtonCredits.GetComponent<Button>().onClick.AddListener(() => WriteBoardMessage(m_CREDITS));

        m_ButtonRestartLevel.GetComponent<Button>().onClick.AddListener(RestartLevel);
        m_ButtonRestartGame.GetComponent<Button>().onClick.AddListener(RestartGame);

        m_ButtonQuit.GetComponent<Button>().onClick.AddListener(Quit);
    }

    public void SetInfoMessage(string message)
    {
        m_TextBoard.text = message;
    }

    public void ResetMenu()
    {
        SetInfoMessage(m_INPUT);
        m_ButtonPlay.SetActive(true);
        m_ButtonNextLevel.SetActive(false);
        m_ButtonInfo.SetActive(true);
        m_ButtonInput.SetActive(true);
        m_ButtonCredits.SetActive(true);
        m_ButtonRestartLevel.SetActive(true);
        m_ButtonRestartGame.SetActive(true);
    }

    public void SetEndingLevelWindow(Script_GameController.EndOption endOption)
    {
        m_ButtonInfo.SetActive(false);
        m_ButtonInput.SetActive(false);
        m_ButtonCredits.SetActive(false);
        m_ButtonPlay.SetActive(false);

        switch (endOption)
        {
            case Script_GameController.EndOption.Win:
                m_ButtonRestartLevel.SetActive(false);
                m_ButtonRestartGame.SetActive(false);
                m_ButtonNextLevel.SetActive(false);
                break;
            case Script_GameController.EndOption.NextLevel:
                m_ButtonRestartLevel.SetActive(false);
                m_ButtonRestartGame.SetActive(false);
                m_ButtonNextLevel.SetActive(true);
                break;
            case Script_GameController.EndOption.Lose:
                m_ButtonNextLevel.SetActive(false);
                break;
        }
    }

    void Play()
    {
        if (m_firstExec)
        {
            var text = m_ButtonPlay.GetComponentInChildren<Text>();
            text.text = "RESUME!";
            m_firstExec = false;
            m_ImageBackground.gameObject.SetActive(false);
            m_BlackBackground.gameObject.SetActive(false);
            m_ButtonRestartLevel.SetActive(true);
            m_ButtonRestartGame.SetActive(true);
            m_Script_GameController.AllowPauseGame(true);
        }
        m_Script_GameController.ResumeGame();
    }
    void WriteBoardMessage(string message)
    {
        m_TextBoard.text = message;
    }
    void RestartLevel()
    {
        //SetInfoMessage() // TODO reset initial info message
        m_Script_GameController.RestartLevel(); //TODO Check if switch order with next line (ResumeGame must be executed after next scene is fully loaded)
        m_Script_GameController.ResumeGame(); 
    }
    void RestartGame()
    {
        //SetInfoMessage() // TODO reset initial info message
        m_Script_GameController.RestartGame(); //TODO Check if switch order
        m_Script_GameController.ResumeGame(); 
    }
    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
