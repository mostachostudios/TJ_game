using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_MenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] GameObject m_ButtonPlay;
    [SerializeField] GameObject m_ButtonInfo;
    [SerializeField] GameObject m_ButtonInput;
    [SerializeField] GameObject m_ButtonCredits;
    [SerializeField] GameObject m_ButtonQuit;

    [Header("Text Board")]
    [SerializeField] Text m_TextBoard;

    [Header("Audio")]
    [Tooltip("Audio clip to be played when player wins the game")]
    [SerializeField] AudioClip m_AudioWin;

    [Tooltip("Audio clip to be played when player loses the game")]
    [SerializeField] AudioClip m_AudioLose;

    private AudioSource m_AudioSource;

    private Script_PauseController m_Script_PauseController;

    //TODO Handle the following board messages in a proper way 
    private string m_INFO = "Messages info";
    private string m_INPUT = "Input info";
    private string m_CREDITS = "Made with Unity by...";

    private bool m_firstExec = true;

    public enum EndOption
    {
        Win,
        Lose
    }

    void Awake()
    {
        m_ButtonPlay.GetComponent<Button>().onClick.AddListener(Play);

        m_ButtonInfo.GetComponent<Button>().onClick.AddListener(() => WriteBoardMessage(m_INFO));
        m_ButtonInput.GetComponent<Button>().onClick.AddListener(() => WriteBoardMessage(m_INPUT));
        m_ButtonCredits.GetComponent<Button>().onClick.AddListener(() => WriteBoardMessage(m_CREDITS));

        m_ButtonQuit.GetComponent<Button>().onClick.AddListener(Quit);

        gameObject.AddComponent<AudioListener>();

        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.playOnAwake = false;

        m_Script_PauseController = GameObject.FindWithTag("RootGame").GetComponent<Script_PauseController>();
    }

    void Play()
    {
        if (m_firstExec)
        {
            var text = m_ButtonPlay.GetComponentInChildren<Text>();
            text.text = "RESUME!";
            m_firstExec = false;
            m_Script_PauseController.AllowPauseGame(true);
        }
        m_Script_PauseController.ResumeGame();
    }

    void WriteBoardMessage(string message)
    {
        m_TextBoard.text = message;
    }

    void Quit()
    {
        Application.Quit(); // Warning: this only works when the project is built. Won't work when playing in the editor
    }

    /// <summary>
    /// Allows sending generic messages (info, quest, etc) during game play. Game will get paused
    /// </summary>
    /// <param name="message"></param>
    public void ShowBoardMessage(string message)
    {
        m_TextBoard.text = message;
        m_INFO = message;
        m_Script_PauseController.PauseGame();
    }

    public void EndGame(EndOption option)
    {
        m_ButtonPlay.SetActive(false);
        m_ButtonInfo.SetActive(false);
        m_ButtonInput.SetActive(false);
        m_ButtonCredits.SetActive(false);

        switch (option)
        {
            case EndOption.Win:
                m_TextBoard.text = "Congrats! You managed to escape on time.";
                m_AudioSource.clip = m_AudioWin;
                break;
            case EndOption.Lose:
                m_TextBoard.text = "GAME OVER\n\nSorry. You could not make it.";
                m_AudioSource.clip = m_AudioLose;
                break;
        }

        m_Script_PauseController.AllowPauseGame(false);
        m_Script_PauseController.PauseGame();
        m_AudioSource.Play();
    }
}
