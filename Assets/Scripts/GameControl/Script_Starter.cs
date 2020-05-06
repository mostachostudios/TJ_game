using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script makes sure the game will always launch the startup scene first, no matter from what scene we are playing the game from the editor
/// This script must be attached to a single GameObject in the scene, being it its only component 
/// </summary>
public class Script_Starter : MonoBehaviour
{
    private int startSceneIndex = -1;
    void Awake()
    {
        if(FindObjectsOfType<Script_Starter>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Scene currentScene = SceneManager.GetActiveScene();
            startSceneIndex = currentScene.buildIndex;

            Debug.Log("Running from '" + currentScene.name + "' scene with index: "+ startSceneIndex);

            if (currentScene.buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public int StartSceneIndex()
    {
        return startSceneIndex;
    }
}
