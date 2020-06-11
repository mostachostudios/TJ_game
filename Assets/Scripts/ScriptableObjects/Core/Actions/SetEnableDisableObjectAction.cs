using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnableDisableObjectAction : Action
{
    public GameObject gameObject;
    public bool active;

    protected override bool StartDerived()
    {
        gameObject.SetActive(active);
        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetEnableDisableObjectAction clone = ScriptableObject.CreateInstance<SetEnableDisableObjectAction>();

        clone.gameObject = this.gameObject;
        clone.active = this.active;

        return clone;
    }
}
