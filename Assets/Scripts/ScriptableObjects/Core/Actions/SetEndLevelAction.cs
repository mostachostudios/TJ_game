using UnityEngine;
using UnityEngine.Localization;

public class SetEndLevelAction : Action
{
    [Tooltip("Choose between win game, lose game, or go to the next level")]
    public Script_GameController.EndOption endOption;
    public LocalizedString localizedText;

    private Script_GameController script_GameController;

    protected override bool StartDerived()
    {
        script_GameController = FindObjectOfType<Script_GameController>();

        if (script_GameController == null)
        {
            throw new UnityException("There isn't a Script_GameController instance on the scene");
        }

        var localizedString = localizedText.GetLocalizedString();

        if (localizedString.IsDone)
        {
            DoAction(localizedString.Result);
        }
        else
        {
            localizedText.RegisterChangeHandler(DoAction);
        }

        return true;
    }

    private void DoAction(string s)
    {
        script_GameController.EndLevel(endOption, s);
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    public override void forceFinish()
    {
        localizedText.ClearChangeHandler();
    }

    protected override Action CloneDerived()
    {
        SetEndLevelAction clone = ScriptableObject.CreateInstance<SetEndLevelAction>();

        clone.endOption = this.endOption;

        return clone;
    }
}
