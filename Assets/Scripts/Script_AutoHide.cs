using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_AutoHide : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
