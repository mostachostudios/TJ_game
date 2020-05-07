using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CheckpointsManager : MonoBehaviour
{
    [SerializeField] float m_CheckpointDuration = 5f;
    [SerializeField] AudioClip m_AudioReward;

    List<GameObject> checkpoints;

    private int currentCheckpoint;

    private AudioSource m_AudioSourceReward;

    private Script_GameController m_Script_GameController;
    private Script_UIController m_Script_UIController;
    private Script_Countdown m_Script_Countdown;
   
    void Start()
    {
        checkpoints = new List<GameObject>();
        currentCheckpoint = 0;

        var checkpointList = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (var checkpoint in checkpointList)
        {
            checkpoint.SetActive(false);
            checkpoints.Add(checkpoint);
        }

        m_AudioSourceReward = gameObject.AddComponent<AudioSource>();
        m_AudioSourceReward.playOnAwake = false;
        m_AudioSourceReward.clip = m_AudioReward;
        m_AudioSourceReward.volume = 0.2f;

        m_Script_GameController = GameObject.FindWithTag("RootGame").GetComponent<Script_GameController>();
        m_Script_UIController = GameObject.FindWithTag("UI").GetComponent<Script_UIController>();
        m_Script_Countdown = GameObject.FindWithTag("World").GetComponent<Script_Countdown>();
        
        if (checkpoints.Count > 0)
        {
            checkpoints[currentCheckpoint].SetActive(true);
        }
        else
        {
            Debug.LogError("There are no checkpoints in the scene. Please, make sure there is always at least one checkpoint in the scene");
        }
    }

    public void CheckAndGoNext()
    {
        checkpoints[currentCheckpoint].SetActive(false);

        currentCheckpoint++;

        if (currentCheckpoint >= checkpoints.Count)
        {
            m_Script_GameController.GoNextLevel();
        }
        else
        {
            string rewardMessage;
            if(checkpoints.Count - currentCheckpoint == 1)
            {
                rewardMessage = "Almost there!! Only one place to go!";
            }
            else if (checkpoints.Count - currentCheckpoint <= 3)
            {
                rewardMessage = "Keep going!! You are about to make it!";
            }
            else
            {
                rewardMessage = "Great!! You can do it!";
            }

            m_Script_UIController.SetTextMessage(rewardMessage);
            m_Script_UIController.EraseTextMessage(m_CheckpointDuration);

            m_Script_Countdown.IncreaseTime();
            m_AudioSourceReward.Play();
            checkpoints[currentCheckpoint].SetActive(true);
        }
    }
}
