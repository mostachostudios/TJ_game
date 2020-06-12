//https://forum.unity.com/threads/simple-ui-animation-fade-in-fade-out-c.439825/
using System.Collections;
using System.Collections.Generic;
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
            StartCoroutine(FadeOutAndGoNext());
        }
    }

    IEnumerator FadeOutAndGoNext()
    {
        var mat = gameObject.GetComponent<MeshRenderer>().material;

        for (float i = mat.color.a; i >= 0.0f; i -= Time.deltaTime / 6f)
        {
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, i);
            yield return null;
        }
        m_Script_CheckpointManager.CheckAndGoNext(m_IncreaseTime);
        yield return null;
    }
}
