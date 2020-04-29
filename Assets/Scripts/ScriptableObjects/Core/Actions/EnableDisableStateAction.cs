using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableStateAction : Action
{
    public State state;
    public bool enable = true;

    protected override bool StartDerived()
    {
        if(state == null)
        {
            throw new UnityException("State is null");
        }
        state.enabled = enable;
        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        EnableDisableStateAction clone = ScriptableObject.CreateInstance<EnableDisableStateAction>();

        clone.state = this.state;
        clone.enabled = this.enable;

        return clone;
    }
}
