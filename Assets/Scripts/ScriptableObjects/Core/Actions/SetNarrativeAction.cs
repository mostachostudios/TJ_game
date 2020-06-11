using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class SetNarrativeAction : Action
{
    public LocalizedString m_narrativeLocalizedText;

    private Script_UIController script_UIController;

    protected override bool StartDerived()
    {
        script_UIController = FindObjectOfType<Script_UIController>();

        if (script_UIController == null)
        {
            throw new UnityException("There isn't a Script_UIController instance on the scene");
        }

        m_narrativeLocalizedText.RegisterChangeHandler(DisplayNarrative);
        m_narrativeLocalizedText.GetLocalizedString();

        return false;
    }

    private void DisplayNarrative(string s)
    {
        script_UIController.SetNarrative(s);
        m_narrativeLocalizedText.ClearChangeHandler();
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    public override void forceFinish()
    {
        m_narrativeLocalizedText.ClearChangeHandler();
    }

    protected override Action CloneDerived()
    {
        SetNarrativeAction clone = ScriptableObject.CreateInstance<SetNarrativeAction>();
        clone.m_narrativeLocalizedText = this.m_narrativeLocalizedText;
        return clone;
    }
}
