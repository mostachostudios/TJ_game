using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Script_WorldInit : MonoBehaviour
{
    PostProcessVolume postProcessVolume;
    void Start()
    {
        postProcessVolume = gameObject.GetComponent<PostProcessVolume>();

        StartCoroutine(EnableDisableGlobal());
    }

    IEnumerator EnableDisableGlobal()
    {
        yield return new WaitForSecondsRealtime(1f);
        postProcessVolume.isGlobal = true;
        yield return new WaitForSecondsRealtime(1f);
        postProcessVolume.isGlobal = false;
        yield return null;
    }
}
