using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public bool done = false;
    public bool cutsceneSeen = false;

    private void Awake()
    {
        // Singleton pattern
        if (FindObjectsOfType<Tutorial>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
