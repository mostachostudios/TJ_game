//https://forum.unity.com/threads/simple-ui-animation-fade-in-fade-out-c.439825/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CheckpointsManager : MonoBehaviour
{    
    [SerializeField] AudioClip m_AudioReward;

    [SerializeField] private GameObject[] checkpoints;
    private int currentCheckpoint;

    private AudioSource m_AudioSourceReward;

    private Script_GameController m_Script_GameController;
    private Script_Countdown m_Script_Countdown;
    private HashSet<Script_Checkpoint> m_enabledCheckpoints = new HashSet<Script_Checkpoint>();

    void Start()
    {
        currentCheckpoint = 0;

        m_AudioSourceReward = gameObject.AddComponent<AudioSource>();
        m_AudioSourceReward.playOnAwake = false;
        m_AudioSourceReward.clip = m_AudioReward;
        m_AudioSourceReward.volume = 0.1f;

        m_Script_GameController = FindObjectOfType<Script_GameController>();
        m_Script_Countdown = FindObjectOfType<Script_Countdown>();

        if (checkpoints.Length > 0)
        {
            checkpoints[currentCheckpoint].SetActive(true);
        }
    }

    public void CheckAndGoNext(float increaseTime, Script_Checkpoint checkpoint)
    {
        if(m_enabledCheckpoints.Contains(checkpoint) == false)
        {
            m_enabledCheckpoints.Add(checkpoint);

            StartCoroutine(FadeOutAndDisable(checkpoints[currentCheckpoint]));

            currentCheckpoint++;

            if (currentCheckpoint >= checkpoints.Length)
            {
                m_Script_GameController.GoNextLevel();
            }
            else
            {
                m_Script_Countdown.IncreaseTime(increaseTime);
                m_AudioSourceReward.Play();
                checkpoints[currentCheckpoint].SetActive(true);
            }
        }
    }

    IEnumerator FadeOutAndDisable(GameObject checkpoint)
    {
        var mat = checkpoint.GetComponent<MeshRenderer>().material;

        for (float i = mat.color.a; i > 0f; i -= Time.deltaTime / 6f)
        {
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, i);
            yield return null;
        }
        checkpoint.SetActive(false);
        yield return null;
    }
}
