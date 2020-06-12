using UnityEngine;

public class Script_Checkpoint : MonoBehaviour
{
    [Tooltip("Time added to TimeLeft each time user gets to a at check point")]
    [SerializeField] float m_IncreaseTime = 20.0f;

    private Script_CheckpointsManager m_Script_CheckpointManager;

    void Awake()
    {
        m_Script_CheckpointManager = GameObject.FindObjectOfType<Script_CheckpointsManager>();
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_Script_CheckpointManager.CheckAndGoNext(m_IncreaseTime);
        }
    }
}
