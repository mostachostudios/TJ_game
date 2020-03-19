using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Script_UIController : MonoBehaviour
{
    [SerializeField] Text m_TextCountdown;
    [SerializeField] Text m_TextCheckpointReward;
    [SerializeField] float m_CheckpointDuration = 5f;

    public void SetTextCountdown(string text)
    {
        m_TextCountdown.text = text;
    }

    public void SetTextCheckpointReward(string text)
    {
        m_TextCheckpointReward.enabled = true;
        m_TextCheckpointReward.text = text;
        StartCoroutine(DisableCheckpointReward());
    }

    IEnumerator DisableCheckpointReward()
    {
        yield return new WaitForSeconds(m_CheckpointDuration);
        m_TextCheckpointReward.enabled = false;
    }
}
