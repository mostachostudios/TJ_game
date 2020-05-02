using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAction : Action
{
    public string debugMessage;

    protected override bool StartDerived()
    {
        Debug.Log(debugMessage);

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        DebugAction clone = ScriptableObject.CreateInstance<DebugAction>();

        return clone;
    }
}
