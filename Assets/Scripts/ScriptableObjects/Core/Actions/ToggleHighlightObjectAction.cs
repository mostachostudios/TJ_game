using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHighlightObjectAction : Action
{
    public GameObject instance;

    protected override bool StartDerived()
    {
        Outline outline = instance.GetComponent<Outline>();

        if (outline != null)
        {
            Destroy(outline);
        }
        else
        {
            instance.AddComponent<Outline>();
        }
        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        ToggleHighlightObjectAction clone = ScriptableObject.CreateInstance<ToggleHighlightObjectAction>();
        clone.instance = this.instance;

        return clone;
    }
}
