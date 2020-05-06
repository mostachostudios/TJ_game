using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Script_UIController : MonoBehaviour
{
    [SerializeField] Text m_TextMessage;
    [SerializeField] Text m_TextCountdown;

    public void SetTextMessage(string text, float time = 0.0f)
    {
        m_TextMessage.enabled = true;
        m_TextMessage.text = text;
        if (time > 0.0f) // Don't hide text if time == 0
        {
            HideTextMessage(time);
        }
    }

    public void HideTextMessage(float time = 0.0f)
    {
        if (time > 0.0f)
        {
            StartCoroutine(HideText(time));
        }
        else
        {
            m_TextMessage.enabled = false;
        }
    }

    IEnumerator HideText(float time)
    {
        yield return new WaitForSeconds(time);
        m_TextMessage.enabled = false;
    }

    public void SetTextCountdown(string text)
    {
        m_TextCountdown.text = text;
    }

    public void ClearUI()
    {

    }

    //TODO add API for dialog text
}
