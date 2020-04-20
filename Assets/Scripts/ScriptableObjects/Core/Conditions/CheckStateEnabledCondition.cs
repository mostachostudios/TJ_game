using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStateEnabledCondition : Condition
{
    public State state;
    public bool expected;

    public override bool Check()
    {
        if (state == null)
        {
            return false;
        }

        return state.enabled == expected;
    }

    public override Condition Clone()
    {
        CheckStateEnabledCondition clone = ScriptableObject.CreateInstance<CheckStateEnabledCondition>();

        clone.state = this.state;
        clone.expected = this.expected;

        return clone;
    }
}
