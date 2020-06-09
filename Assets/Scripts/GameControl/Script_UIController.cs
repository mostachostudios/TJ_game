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
        m_AudioSource.volume = 0.2f;
        m_AudioSource.playOnAwake = false;
        m_AudioSource.loop = false;
        m_AudioSource.time = 0.0f;

        
        PostProcessVolume m_PostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
        m_PostProcessVolume.isGlobal = true;
        m_PostProcessVolume.profile = m_PostProcessProfile;
        m_PostProcessVolume.weight = 0.8f;
        m_PostProcessVolume.enabled = false;
    }

    public void SetTextCountdown(string text)
    {
        m_TextCountdown.text = text;
    }

    public void ShowCountdown(bool show)
    {
        m_TextCountdown.enabled = show;
        m_ImageCountdown.enabled = show;
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
    }
}
