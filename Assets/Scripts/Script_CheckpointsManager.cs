﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CheckpointsManager : MonoBehaviour
{    
    [SerializeField] AudioClip m_AudioReward;

    private List<GameObject> checkpoints;
    private int currentCheckpoint;

    private AudioSource m_AudioSourceReward;

    private Script_GameController m_Script_GameController;
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
        m_AudioSourceReward.volume = 0.1f;

        m_Script_GameController = FindObjectOfType<Script_GameController>();
        m_Script_Countdown = FindObjectOfType<Script_Countdown>();

        if (checkpoints.Count > 0)
        {
            checkpoints[currentCheckpoint].SetActive(true);
        }
    }

    public void CheckAndGoNext(float increaseTime = 20f)
    {
        checkpoints[currentCheckpoint].SetActive(false);

        currentCheckpoint++;

        if (currentCheckpoint >= checkpoints.Count)
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
