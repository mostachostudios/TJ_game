using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
public class PlayDialogAction : Action
{
    // public Dialog dialog;

    protected override bool StartDerived()
    {
        // TODO
        return false;
    }

    protected override bool UpdateDerived()
    {
        //dialog.Update();
        return /*dialog.IsFinished()*/true;
    }

    protected override Action CloneDerived()
    {
        PlayDialogAction clone = ScriptableObject.CreateInstance<PlayDialogAction>();

        //clone.dialog = this.dialog;

        return clone;
    }
}
