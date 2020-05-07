using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Script_UIController : MonoBehaviour
{
    [SerializeField] Text m_TextMessage;
    [SerializeField] Text m_TextCountdown;

    [SerializeField] Image Avatar1;
    [SerializeField] Text m_TextName1;
    [SerializeField] Text m_TextDialog1;

    [SerializeField] Image Avatar2;
    [SerializeField] Text m_TextName2;
    [SerializeField] Text m_TextDialog2;

    [SerializeField] Image Avatar3;
    [SerializeField] Text m_TextName3;
    [SerializeField] Text m_TextDialog3;

    public void SetTextCountdown(string text)
    {
        m_TextCountdown.text = text;
    }

    public void SetDialog(Dialog dialog)
    {
        switch (dialog.m_DialogPosition)
        {
            case Dialog.DialogPosition.UP_LEFT:
                Avatar1.sprite = dialog.m_Avatar;
                Avatar1.color = new Color(Avatar1.color.r, Avatar1.color.g, Avatar1.color.b, 1f);
                m_TextDialog1.text = dialog.m_Text;
                m_TextName1.text = dialog.m_Avatar.name.ToUpper();
                break;
            case Dialog.DialogPosition.DOWN_LEFT:
                Avatar2.sprite = dialog.m_Avatar;
                Avatar2.color = new Color(Avatar2.color.r, Avatar2.color.g, Avatar2.color.b, 1f);
                m_TextDialog2.text = dialog.m_Text;
                m_TextName2.text = dialog.m_Avatar.name.ToUpper();
                break;
            case Dialog.DialogPosition.DOWN_RIGHT:
                Avatar3.sprite = dialog.m_Avatar;
                Avatar3.color = new Color(Avatar3.color.r, Avatar3.color.g, Avatar3.color.b, 1f);
                m_TextDialog3.text = dialog.m_Text;
                m_TextName3.text = dialog.m_Avatar.name.ToUpper();
                break;
        }
    }

    public void EraseDialog(Dialog dialog)
    {
        switch (dialog.m_DialogPosition)
        {
            case Dialog.DialogPosition.UP_LEFT:
                Avatar1.color = new Color(Avatar1.color.r, Avatar1.color.g, Avatar1.color.b, 0f);
                m_TextDialog1.text = "";
                m_TextName1.text = "";
                break;
            case Dialog.DialogPosition.DOWN_LEFT:
                Avatar2.color = new Color(Avatar2.color.r, Avatar2.color.g, Avatar2.color.b, 0f);
                m_TextDialog2.text = "";
                m_TextName2.text = "";
                break;
            case Dialog.DialogPosition.DOWN_RIGHT:
                Avatar3.color = new Color(Avatar3.color.r, Avatar3.color.g, Avatar3.color.b, 0f);
                m_TextDialog3.text = "";
                m_TextName3.text = "";
                break;
        }
    }

    /// <summary>
    /// Use this to display tips or notifications to the player
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void SetTextMessage(string text)
    {
        m_TextMessage.text = text;
    }

    /// <summary>
    /// Erase text linked to tips and notifications after 'time' seconds
    /// </summary>
    /// <param name="time"></param>
    public void EraseTextMessage(float time = 0.0f)
    {
        if (time > 0.0f)
        {
            StartCoroutine(EraseText(time));
        }
        else
        {
            m_TextMessage.text = "";
        }
    }

    IEnumerator EraseText(float time)
    {
        yield return new WaitForSeconds(time);
        m_TextMessage.text = "";
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
        Avatar1.color = new Color(Avatar1.color.r, Avatar1.color.g, Avatar1.color.b, 0f);
        Avatar2.color = new Color(Avatar2.color.r, Avatar2.color.g, Avatar2.color.b, 0f);
        Avatar3.color = new Color(Avatar3.color.r, Avatar3.color.g, Avatar3.color.b, 0f);
    }
}
