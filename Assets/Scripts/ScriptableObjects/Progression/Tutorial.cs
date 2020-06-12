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
        // QUICK_FIX: TODO, CLEAN UP
        var tutorials = FindObjectsOfType<Tutorial>();
        if (tutorials.Length > 1)
        {
            if(tutorials[0] == this)
            {
                this.done = tutorials[1].done;
                this.cutsceneSeen = tutorials[1].cutsceneSeen;
            }
            else
            {

                this.done = tutorials[0].done;
                this.cutsceneSeen = tutorials[0].cutsceneSeen;
            }
            //Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
