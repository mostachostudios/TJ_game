using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Checkpoint : MonoBehaviour
{
    private Script_CheckpointsManager m_Script_CheckpointManager;

    void Awake()
    {
        m_Script_CheckpointManager = GameObject.FindObjectOfType<Script_CheckpointsManager>();
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_Script_CheckpointManager.CheckAndGoNext();
        }
    }
}
