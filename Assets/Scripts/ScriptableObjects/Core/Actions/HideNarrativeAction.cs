using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class HideNarrativeAction : Action
{
    private Script_UIController script_UIController;

    protected override bool StartDerived()
    {
        script_UIController = FindObjectOfType<Script_UIController>();

        if (script_UIController == null)
        {
            throw new UnityException("There isn't a Script_UIController instance on the scene");
        }

        script_UIController.HideNarrative(0.5f);

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    public override void forceFinish()
    {
        script_UIController.HideNarrative(0);
    }

    protected override Action CloneDerived()
    {
        HideNarrativeAction clone = ScriptableObject.CreateInstance<HideNarrativeAction>();
        return clone;
    }
}
