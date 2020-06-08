using UnityEngine;

public class SetEndLevelAction : Action
{
    [Tooltip("Choose between win game, lose game, or go to the next level")]
    public Script_GameController.EndOption endOption;

    private Script_GameController script_GameController;

    protected override bool StartDerived()
    {
        script_GameController = FindObjectOfType<Script_GameController>();

        if (script_GameController == null)
        {
            throw new UnityException("There isn't a Script_GameController instance on the scene");
        }

        script_GameController.EndLevel(endOption);

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    public override void forceFinish()
    {
 
    }

    protected override Action CloneDerived()
    {
        SetEndLevelAction clone = ScriptableObject.CreateInstance<SetEndLevelAction>();

        clone.endOption = this.endOption;

        return clone;
    }
}
