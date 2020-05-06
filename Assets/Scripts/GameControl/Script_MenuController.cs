﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Script_MenuController : MonoBehaviour
{
    [Header("Background Image Texture")]
    [Tooltip("Reference to Raw Image component")] [SerializeField] RawImage m_ImageBackground;
    [Tooltip("Reference to Raw Image component")] [SerializeField] RawImage m_BlackBackground;

    [Header("Menu Buttons")]
    [SerializeField] GameObject m_ButtonPlay;
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

    private string m_INPUT = "\n"+
                                "ASWD or ARROW Buttons   ------------------   Movement\n" +
                                "SPACE BAR  --------------------------------   Jump\n" +
                                "SHIFT  -------------------------------------   Run\n" +
                                "ALT + Move  --------------------------------   Crouch\n" +
                                "CTRL + Move  ------------------------------   Crawl\n" +
                                "TAB  ---------------------------------------   Stealth\n" +
                                "INTRO or LEFT Mouse Button + Move  -------   Push\n" +
                                "RIGHT Mouse Button  -----------------------   Change Camera Angle\n" +
                                "M (ESC in Build)  ----------------------------   Pause and Resume Game";

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
        m_ButtonInfo.SetActive(true);
        m_ButtonInput.SetActive(true);
        m_ButtonCredits.SetActive(true);
        m_ButtonRestartLevel.SetActive(true);
        m_ButtonRestartGame.SetActive(true);
    }

    public void EndingGameWindow(string text, bool isRestartable)
    {
        m_TextBoard.text = text;

        m_ButtonPlay.SetActive(false);
        m_ButtonInfo.SetActive(false);
        m_ButtonInput.SetActive(false);
        m_ButtonCredits.SetActive(false);
        if (!isRestartable)
        {
            m_ButtonRestartLevel.SetActive(false);
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
        m_Script_GameController.ResumeGame(); //TODO Check if switch order
        m_Script_GameController.RestartLevel();
    }
    void RestartGame()
    {
        //SetInfoMessage() // TODO reset initial info message
        m_Script_GameController.ResumeGame(); //TODO Check if switch order
        m_Script_GameController.RestartGame();
    }
    void Quit()
    {
        Application.Quit(); // Warning: this only works when the project is built. Won't work when playing in the editor
    }
}