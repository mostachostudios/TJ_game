using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Script_MenuController : MonoBehaviour
{
    public enum Mode { INPUT, MOUSTACHO, LANGUAGE, WIN, LOSE, FINAL_WIN, CUSTOM };

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

    [SerializeField] Text m_textPlay;
    [SerializeField] Text m_textResume;

    [SerializeField] GameObject m_ButtonSpanishFlag;
    [SerializeField] GameObject m_ButtonEnglishFlag;

    [Header("Text Board")]
    [SerializeField] Text m_Title;
    [SerializeField] Text m_TextBoardInput;
    [SerializeField] Text m_TextBoardMoustacho;
    [SerializeField] Text m_TextBoardWin;
    [SerializeField] Text m_TextBoardLose;
    [SerializeField] Text m_TextBoardFinalWin;
    [SerializeField] Text m_TextBoardCustom;

    [Header("Effects")]
    [Tooltip("A post process effect applied when the menu window pops up (Game is paused).")]
    [SerializeField] PostProcessProfile m_PostProcessProfile;

    private Mode m_currentMode = Mode.INPUT;

    private Script_GameController m_Script_GameController;
    public Script_UIController m_Script_UIController;
    
    private bool m_firstExec = true;

    public void Init(Script_GameController script_GameController)
    {
        m_Script_GameController = script_GameController;

        PostProcessVolume m_MenuPostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
        m_MenuPostProcessVolume.isGlobal = true;
        m_MenuPostProcessVolume.profile = m_PostProcessProfile;

        m_ButtonPlay.GetComponent<Button>().onClick.AddListener(Play);
        m_ButtonNextLevel.GetComponent<Button>().onClick.AddListener(RestartLevel);

        m_ButtonInfo.GetComponent<Button>().onClick.AddListener(() => SetInfoMessage(Mode.LANGUAGE));
        m_ButtonInput.GetComponent<Button>().onClick.AddListener(() => SetInfoMessage(Mode.INPUT));
        m_ButtonCredits.GetComponent<Button>().onClick.AddListener(() => SetInfoMessage(Mode.MOUSTACHO));
        m_ButtonEnglishFlag.GetComponent<Button>().onClick.AddListener(() => SelectLocale(LocalizationSettings.AvailableLocales.Locales[1]));
        m_ButtonSpanishFlag.GetComponent<Button>().onClick.AddListener(() => SelectLocale(LocalizationSettings.AvailableLocales.Locales[0]));


        m_ButtonRestartLevel.GetComponent<Button>().onClick.AddListener(RestartLevel);
        m_ButtonRestartGame.GetComponent<Button>().onClick.AddListener(RestartGame);

        m_ButtonQuit.GetComponent<Button>().onClick.AddListener(Quit);

        SetInfoMessage(Mode.INPUT);
    }

    private void SelectLocale(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
    }

    public void SetInfoMessage(Mode mode, string text = null)
    {
        if(mode == Mode.MOUSTACHO)
        {
            m_TextBoardFinalWin.gameObject.SetActive(false);
            m_TextBoardMoustacho.gameObject.SetActive(true);
            m_TextBoardInput.gameObject.SetActive(false);
            m_ButtonEnglishFlag.gameObject.SetActive(false);
            m_ButtonSpanishFlag.gameObject.SetActive(false);
            m_TextBoardWin.gameObject.SetActive(false);
            m_TextBoardLose.gameObject.SetActive(false);
            m_TextBoardCustom.gameObject.SetActive(false);
        }
        else if(mode == Mode.INPUT)
        {
            m_TextBoardFinalWin.gameObject.SetActive(false);
            m_TextBoardMoustacho.gameObject.SetActive(false);
            m_TextBoardInput.gameObject.SetActive(true);
            m_ButtonEnglishFlag.gameObject.SetActive(false);
            m_ButtonSpanishFlag.gameObject.SetActive(false);
            m_TextBoardWin.gameObject.SetActive(false);
            m_TextBoardLose.gameObject.SetActive(false);
            m_TextBoardCustom.gameObject.SetActive(false);
        }
        else if(mode == Mode.LANGUAGE)
        {
            m_TextBoardFinalWin.gameObject.SetActive(false);
            m_TextBoardMoustacho.gameObject.SetActive(false);
            m_TextBoardInput.gameObject.SetActive(false);
            m_ButtonEnglishFlag.gameObject.SetActive(true);
            m_ButtonSpanishFlag.gameObject.SetActive(true);
            m_TextBoardWin.gameObject.SetActive(false);
            m_TextBoardLose.gameObject.SetActive(false);
            m_TextBoardCustom.gameObject.SetActive(false);
        }
        else if(mode == Mode.FINAL_WIN)
        {
            m_TextBoardFinalWin.gameObject.SetActive(true);
            m_TextBoardMoustacho.gameObject.SetActive(false);
            m_TextBoardInput.gameObject.SetActive(false);
            m_ButtonEnglishFlag.gameObject.SetActive(false);
            m_ButtonSpanishFlag.gameObject.SetActive(false);
            m_TextBoardWin.gameObject.SetActive(false);
            m_TextBoardLose.gameObject.SetActive(false);
            m_TextBoardCustom.gameObject.SetActive(false);
        }
        else if (mode == Mode.WIN)
        {
            m_TextBoardFinalWin.gameObject.SetActive(false);
            m_TextBoardMoustacho.gameObject.SetActive(false);
            m_TextBoardInput.gameObject.SetActive(false);
            m_ButtonEnglishFlag.gameObject.SetActive(false);
            m_ButtonSpanishFlag.gameObject.SetActive(false);
            m_TextBoardWin.gameObject.SetActive(true);
            m_TextBoardLose.gameObject.SetActive(false);
            m_TextBoardCustom.gameObject.SetActive(false);

        }
        else if (mode == Mode.LOSE)
        {
            m_TextBoardFinalWin.gameObject.SetActive(false);
            m_TextBoardMoustacho.gameObject.SetActive(false);
            m_TextBoardInput.gameObject.SetActive(false);
            m_ButtonEnglishFlag.gameObject.SetActive(false);
            m_ButtonSpanishFlag.gameObject.SetActive(false);
            m_TextBoardWin.gameObject.SetActive(false);
            m_TextBoardLose.gameObject.SetActive(true);
            m_TextBoardCustom.gameObject.SetActive(false);
        }
        else if (mode == Mode.CUSTOM)
        {
            m_TextBoardCustom.text = text;
            m_TextBoardFinalWin.gameObject.SetActive(false);
            m_TextBoardMoustacho.gameObject.SetActive(false);
            m_TextBoardInput.gameObject.SetActive(false);
            m_ButtonEnglishFlag.gameObject.SetActive(false);
            m_ButtonSpanishFlag.gameObject.SetActive(false);
            m_TextBoardWin.gameObject.SetActive(false);
            m_TextBoardLose.gameObject.SetActive(false);
            m_TextBoardCustom.gameObject.SetActive(true);
        }

        m_currentMode = mode;
    }

    public void ResetMenu(Mode mode = Mode.INPUT)
    {
        SetInfoMessage(mode);
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
            m_ButtonPlay.GetComponentsInChildren<Text>();
            m_textPlay.gameObject.SetActive(false);
            m_textResume.gameObject.SetActive(true);
            m_ImageBackground.gameObject.SetActive(false);
            m_BlackBackground.gameObject.SetActive(false);
            m_ButtonRestartLevel.SetActive(true);
            m_ButtonRestartGame.SetActive(true);
            m_Script_GameController.AllowPauseGame(true);
        }
        m_Script_GameController.ResumeGame(true, m_firstExec);

        if(m_firstExec)
        {
            m_firstExec = false;
            m_Script_UIController.FadeOff();
        }
    }

    void RestartLevel()
    {
        m_Script_GameController.RestartLevel();
        m_Script_GameController.ResumeGame();
    }
    void RestartGame()
    {
        m_Script_GameController.RestartGame();
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
