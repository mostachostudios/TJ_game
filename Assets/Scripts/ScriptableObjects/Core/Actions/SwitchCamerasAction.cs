using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamerasAction : Action
{
    public bool usingFreeCamera = false;
    private Script_CameraSwitcher script_CameraSwitcher;

    protected override bool StartDerived()
    {
        script_CameraSwitcher = GameObject.FindWithTag("World").GetComponent<Script_CameraSwitcher>();
        
        if (script_CameraSwitcher == null)
        {
            throw new UnityEngine.UnityException("No Camera Switcher script attaced");
        }

        if (usingFreeCamera)
        {
            script_CameraSwitcher.SwitchToFreeCamera();
        }
        else
        {
            script_CameraSwitcher.SwitchToMainCamera();
        }

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SwitchCamerasAction clone = ScriptableObject.CreateInstance<SwitchCamerasAction>();
        clone.script_CameraSwitcher = this.script_CameraSwitcher;
        clone.usingFreeCamera = this.usingFreeCamera;

        return clone;
    }
}
