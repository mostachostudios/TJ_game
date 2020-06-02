using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Script_UIController : MonoBehaviour
{
    [SerializeField] Text m_TextMessage;
    [SerializeField] Text m_TextCountdown;
    [SerializeField] Image m_ImageCountdown;

    [SerializeField] Image m_Avatar1;
    [SerializeField] Text m_TextName1;
    [SerializeField] Text m_TextDialog1;

    [SerializeField] Image m_Avatar2;
    [SerializeField] Text m_TextName2;
    [SerializeField] Text m_TextDialog2;

    [SerializeField] Image m_Avatar3;
    [SerializeField] Text m_TextName3;
    [SerializeField] Text m_TextDialog3;

    [SerializeField] AudioClip m_AudioDisplayDialog;
    [SerializeField] AudioClip m_AudioDisplayTip;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.volume = 0.2f;
        m_AudioSource.playOnAwake = false;
        m_AudioSource.loop = false;
        m_AudioSource.time = 0.0f;
    }

    public void SetTextCountdown(string text)
    {
        m_TextCountdown.text = text;
    }

    public void ShowCountdown(bool show)
    {
        //FIX_ME (adrian):
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
                m_Avatar1.sprite = dialog.m_Avatar;
                m_Avatar1.color = new Color(m_Avatar1.color.r, m_Avatar1.color.g, m_Avatar1.color.b, 1f);
                m_TextDialog1.text = dialog.m_Text;
                m_TextName1.text = dialog.m_Avatar.name.ToUpper();
                break;
            case Dialog.DialogPosition.DOWN_LEFT:
                m_Avatar2.sprite = dialog.m_Avatar;
                m_Avatar2.color = new Color(m_Avatar2.color.r, m_Avatar2.color.g, m_Avatar2.color.b, 1f);
                m_TextDialog2.text = dialog.m_Text;
                m_TextName2.text = dialog.m_Avatar.name.ToUpper();
                break;
            case Dialog.DialogPosition.DOWN_RIGHT:
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
                m_Avatar1.color = new Color(m_Avatar1.color.r, m_Avatar1.color.g, m_Avatar1.color.b, 0f);
                m_TextDialog1.text = "";
                m_TextName1.text = "";
                break;
            case Dialog.DialogPosition.DOWN_LEFT:
                m_Avatar2.color = new Color(m_Avatar2.color.r, m_Avatar2.color.g, m_Avatar2.color.b, 0f);
                m_TextDialog2.text = "";
                m_TextName2.text = "";
                break;
            case Dialog.DialogPosition.DOWN_RIGHT:
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
        m_TextMessage.text = text;

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
    public void EraseTextMessage(float time = 0.0f)
    {
        StartCoroutine(EraseText(time));
    }

    IEnumerator EraseText(float time)
    {
        yield return new WaitForSeconds(time);
        m_TextMessage.text = "";
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
    }
}
