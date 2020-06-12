using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Script_UIController : MonoBehaviour
{
    [Header("UI objects references")]
    [SerializeField] Text m_TextMessage;
    [SerializeField] Text m_TextCountdown;
    [SerializeField] Image m_ImageCountdown;
    [SerializeField] RawImage m_backgroundCenter;
    [SerializeField] RawImage m_intermediateBackgroundCenter;

    [SerializeField] Image m_Avatar1;
    [SerializeField] Text m_TextName1;
    [SerializeField] Text m_TextDialog1;
    [SerializeField] RawImage m_backgroundName1;
    [SerializeField] RawImage m_backgroundDialog1;

    [SerializeField] Image m_Avatar2;
    [SerializeField] Text m_TextName2;
    [SerializeField] Text m_TextDialog2;
    [SerializeField] RawImage m_backgroundName2;
    [SerializeField] RawImage m_backgroundDialog2;

    [SerializeField] Image m_Avatar3;
    [SerializeField] Text m_TextName3;
    [SerializeField] Text m_TextDialog3;
    [SerializeField] RawImage m_backgroundName3;
    [SerializeField] RawImage m_backgroundDialog3;

    [SerializeField] Image m_FadeImage;

    [SerializeField] RawImage m_narrativeBackground;
    [SerializeField] Text m_narrativeText;

    [SerializeField] RawImage m_inputNameBackground;
    [SerializeField] RawImage m_inputNameCenterBackground;
    [SerializeField] InputField m_inputNameInputField;
    [SerializeField] Text m_inputNameLabel;
    [SerializeField] Text m_inputNamePlaceholder;
    [SerializeField] Text m_inputNameText;
    [SerializeField] Button m_btnSetName;
    [SerializeField] Text m_txtBtnSetName;

    [Header("SFX")]
    [SerializeField] AudioClip m_AudioDisplayDialog;
    [SerializeField] AudioClip m_AudioDisplayTip;

    [Header("VFX")]
    [Tooltip("A post process effect applied if desired when the game pauses but having the Menu not being shown.")]
    [SerializeField] PostProcessProfile m_PostProcessProfile;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.volume = 0.6f;
        m_AudioSource.playOnAwake = false;
        m_AudioSource.loop = false;
        m_AudioSource.time = 0.0f;

        
        PostProcessVolume m_PostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
        m_PostProcessVolume.isGlobal = true;
        m_PostProcessVolume.profile = m_PostProcessProfile;
        m_PostProcessVolume.weight = 0.8f;
        m_PostProcessVolume.enabled = false;

        m_btnSetName.onClick.AddListener(NameOk);
    }

    public void SetTextCountdown(string text)
    {
        m_TextCountdown.text = text;
    }

    public void ShowCountdown(bool show)
    {
        // null check to avoid runtime error when shutdown
        if (m_TextCountdown != null)
        {
            m_TextCountdown.enabled = show;
        }
        if (m_ImageCountdown != null)
        {
            m_ImageCountdown.enabled = show;
        }
    }

    /// <summary>
    /// If time = 0, then dialog won't be erased
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="time"></param>
    public void SetDialog(Dialog dialog, float time = 0f)
    {
        switch (dialog.m_DialogPosition)
        {
            case Dialog.DialogPosition.UP_LEFT:
                m_backgroundDialog1.gameObject.SetActive(true);
                m_backgroundName1.gameObject.SetActive(true);
                m_Avatar1.sprite = dialog.m_Avatar;
                m_Avatar1.color = new Color(m_Avatar1.color.r, m_Avatar1.color.g, m_Avatar1.color.b, 1f);
                m_TextDialog1.text = dialog.m_Text;
                m_TextName1.text = dialog.m_Avatar.name.ToUpper();
                break;
            case Dialog.DialogPosition.DOWN_LEFT:
                m_backgroundDialog2.gameObject.SetActive(true);
                m_backgroundName2.gameObject.SetActive(true);
                m_Avatar2.sprite = dialog.m_Avatar;
                m_Avatar2.color = new Color(m_Avatar2.color.r, m_Avatar2.color.g, m_Avatar2.color.b, 1f);
                m_TextDialog2.text = dialog.m_Text;
                m_TextName2.text = dialog.m_Avatar.name.ToUpper();
                break;
            case Dialog.DialogPosition.DOWN_RIGHT:
                m_backgroundDialog3.gameObject.SetActive(true);
                m_backgroundName3.gameObject.SetActive(true);
                m_Avatar3.sprite = dialog.m_Avatar;
                m_Avatar3.color = new Color(m_Avatar3.color.r, m_Avatar3.color.g, m_Avatar3.color.b, 1f);
                m_TextDialog3.text = dialog.m_Text;
                m_TextName3.text = dialog.m_Avatar.name.ToUpper();
                break;
        }

        if (!m_AudioSource.isPlaying)
        {
            m_AudioSource.clip = m_AudioDisplayDialog;
            m_AudioSource.Play();
        }

        if (time > 0f)
        {
            StartCoroutine(EraseDialogRoutine(dialog, time));
        }
    }

    /// <summary>
    /// If time = 0, then dialog will be immediately erased
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="time"></param>
    public void EraseDialog(Dialog dialog, float time = 0f)
    {
        StartCoroutine(EraseDialogRoutine(dialog, time));
    }

    IEnumerator EraseDialogRoutine(Dialog dialog, float time)
    {
        yield return new WaitForSeconds(time);

        switch (dialog.m_DialogPosition)
        {
            case Dialog.DialogPosition.UP_LEFT:
                m_backgroundDialog1.gameObject.SetActive(false);
                m_backgroundName1.gameObject.SetActive(false);
                m_Avatar1.color = new Color(m_Avatar1.color.r, m_Avatar1.color.g, m_Avatar1.color.b, 0f);
                m_TextDialog1.text = "";
                m_TextName1.text = "";
                break;
            case Dialog.DialogPosition.DOWN_LEFT:
                m_backgroundDialog2.gameObject.SetActive(false);
                m_backgroundName2.gameObject.SetActive(false);
                m_Avatar2.color = new Color(m_Avatar2.color.r, m_Avatar2.color.g, m_Avatar2.color.b, 0f);
                m_TextDialog2.text = "";
                m_TextName2.text = "";
                break;
            case Dialog.DialogPosition.DOWN_RIGHT:
                m_backgroundDialog3.gameObject.SetActive(false);
                m_backgroundName3.gameObject.SetActive(false);
                m_Avatar3.color = new Color(m_Avatar3.color.r, m_Avatar3.color.g, m_Avatar3.color.b, 0f);
                m_TextDialog3.text = "";
                m_TextName3.text = "";
                break;
        }

        yield return null;
    }

    /// <summary>
    /// Use this to display tips or notifications to the player
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void SetTextMessage(string text, bool isTip = false)
    {
        m_backgroundCenter.gameObject.SetActive(true);
        m_TextMessage.text = text;

        m_backgroundCenter.CrossFadeAlpha(1.0f, .0f, false);
        m_TextMessage.CrossFadeAlpha(1.0f, .0f, false);
        m_intermediateBackgroundCenter.CrossFadeAlpha(1.0f, .0f, false);

        if (isTip)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.clip = m_AudioDisplayTip;
                m_AudioSource.Play();
            }
        }
    }

    /// <summary>
    /// Erase text linked to tips and notifications after 'time' seconds
    /// </summary>
    /// <param name="time"></param>
    public void EraseTextMessage(float time = 0.0f, float fadeSeconds = .0f)
    {
        StartCoroutine(EraseText(time, fadeSeconds));
    }

    IEnumerator EraseText(float time, float fadeSeconds = .0f)
    {
        yield return new WaitForSeconds(time);

        m_backgroundCenter.CrossFadeAlpha(0.0f, fadeSeconds, false);
        m_TextMessage.CrossFadeAlpha(0.0f, fadeSeconds, false);
        m_intermediateBackgroundCenter.CrossFadeAlpha(0.0f, fadeSeconds, false);

        yield return null;
    }

    private void OnDisable()
    {
        m_FadeImage.gameObject.SetActive(false);
    }

    public void FadeOff()
    {
        m_FadeImage.CrossFadeAlpha(1.0f, 0.0f, false);
        m_FadeImage.gameObject.SetActive(true);
        m_FadeImage.CrossFadeAlpha(0.0f, 2.0f, false);
        StartCoroutine(FadeCamera(2.0f));
    }

    IEnumerator FadeCamera(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_FadeImage.gameObject.SetActive(false);

        yield return null;
    }

    public void SetNarrative(string text)
    {
        m_narrativeBackground.gameObject.SetActive(true);
        m_narrativeText.gameObject.SetActive(true);
        m_narrativeText.text = text;
    }

    public void HideNarrative(float fadeOffSeconds)
    {
        m_narrativeText.CrossFadeAlpha(0.0f, fadeOffSeconds, false);
        m_narrativeBackground.CrossFadeAlpha(0.0f, fadeOffSeconds, false);
        StartCoroutine(FadeNarrative(fadeOffSeconds));
    }
    IEnumerator FadeNarrative(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_narrativeText.gameObject.SetActive(false);
        m_narrativeBackground.gameObject.SetActive(false);

        yield return null;
    }

    public void ShowInputName(float fadeSeconds)
    {
        m_inputNameBackground.CrossFadeAlpha(0.0f, 0.0f, false);
        m_inputNameCenterBackground.CrossFadeAlpha(0.0f, 0.0f, false);
        m_inputNameInputField.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, false);
        m_inputNameLabel.CrossFadeAlpha(0.0f, 0.0f, false);
        m_inputNamePlaceholder.CrossFadeAlpha(0.0f, 0.0f, false);
        m_inputNameText.CrossFadeAlpha(0.0f, 0.0f, false);
        m_btnSetName.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, false);
        m_txtBtnSetName.CrossFadeAlpha(0.0f, 0.0f, false);

        StartCoroutine(ProcessInputNameFade(1.0f, fadeSeconds));
    }

    public void HideInputName(float fadeSeconds)
    {
        StartCoroutine(ProcessInputNameFade(0.0f, fadeSeconds));
    }

    public bool IsInputNameShowing()
    {
        return m_inputNameBackground.IsActive();
    }

    void NameOk()
    {
        Script_PlayerController player = FindObjectOfType<Script_PlayerController>();
        if(m_inputNameInputField.text.Length > 3)
        {
            player.m_playerName = m_inputNameInputField.text;
            StartCoroutine(ProcessInputNameFade(0.0f, 0.5f));
            Tutorial tutorial1 = FindObjectOfType<Tutorial>();
            tutorial1.cutsceneSeen = true;
        }
    }

    private IEnumerator ProcessInputNameFade(float alpha, float fadeSeconds)
    {
        if (alpha > .0f)
        {
            Script_CameraController cameraController = FindObjectOfType<Script_CameraController>();
            cameraController.ReadInput(false);
            m_inputNameBackground.gameObject.SetActive(true);
            m_inputNameCenterBackground.gameObject.SetActive(true);
            m_inputNameInputField.gameObject.SetActive(true);
            m_inputNameLabel.gameObject.SetActive(true);
            m_inputNamePlaceholder.gameObject.SetActive(true);
            m_inputNameText.gameObject.SetActive(true);
            m_btnSetName.gameObject.SetActive(true);
            m_txtBtnSetName.gameObject.SetActive(true);

            m_inputNameInputField.Select();
            m_inputNameInputField.ActivateInputField();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            m_inputNameBackground.gameObject.SetActive(false);
            m_inputNameCenterBackground.gameObject.SetActive(false);
            m_inputNameInputField.gameObject.SetActive(false);
            m_inputNameLabel.gameObject.SetActive(false);
            m_inputNamePlaceholder.gameObject.SetActive(false);
            m_inputNameText.gameObject.SetActive(false);
            m_btnSetName.gameObject.SetActive(false);
            m_txtBtnSetName.gameObject.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Script_CameraController cameraController = FindObjectOfType<Script_CameraController>();
            cameraController.ReadInput(true);
        }

        m_inputNameBackground.CrossFadeAlpha(alpha, fadeSeconds, false);
        m_inputNameCenterBackground.CrossFadeAlpha(alpha, fadeSeconds, false);
        m_inputNameInputField.GetComponent<Image>().CrossFadeAlpha(alpha, fadeSeconds, false);
        m_inputNameLabel.CrossFadeAlpha(alpha, fadeSeconds, false);
        m_inputNamePlaceholder.CrossFadeAlpha(alpha, fadeSeconds, false);
        m_inputNameText.CrossFadeAlpha(alpha, fadeSeconds, false);
        m_btnSetName.GetComponent<Image>().CrossFadeAlpha(alpha, fadeSeconds, false);
        m_txtBtnSetName.CrossFadeAlpha(alpha, fadeSeconds, false);

        yield return null;
    }

    public void ClearUI()
    {
        m_TextMessage.text = "";
        m_TextDialog1.text = "";
        m_TextDialog2.text = "";
        m_TextDialog3.text = "";
        m_TextName1.text = "";
        m_TextName2.text = "";
        m_TextName3.text = "";
        m_Avatar1.color = new Color(m_Avatar1.color.r, m_Avatar1.color.g, m_Avatar1.color.b, 0f);
        m_Avatar2.color = new Color(m_Avatar2.color.r, m_Avatar2.color.g, m_Avatar2.color.b, 0f);
        m_Avatar3.color = new Color(m_Avatar3.color.r, m_Avatar3.color.g, m_Avatar3.color.b, 0f);

        m_backgroundCenter.gameObject.SetActive(false);
        m_backgroundDialog1.gameObject.SetActive(false);
        m_backgroundDialog2.gameObject.SetActive(false);
        m_backgroundDialog3.gameObject.SetActive(false);
        m_backgroundName1.gameObject.SetActive(false);
        m_backgroundName2.gameObject.SetActive(false);
        m_backgroundName3.gameObject.SetActive(false);

        m_narrativeBackground.gameObject.SetActive(false);
        m_narrativeText.gameObject.SetActive(false);
    }
}
