using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State")]
public class State : ScriptableObject
{
    public StateMachine parentStateMachine;
    public bool enabled = true;
    public bool loop = true;

    public Action[] onEnterActions = new Action[0];
    public Action[] actions = new Action[0];
    public Action[] onExitActions = new Action[0];

    public Trigger[] triggers = new Trigger[0];

    public bool CheckTriggers()
    {
        foreach(Trigger trigger in triggers)
        {
            if(trigger.Check())
            {
                return true;
            }
        }
        return false;
    }

    public State Clone()
    {
        State clone = Instantiate(this);

        clone.name = this.name;
        for (int i = 0; i < onEnterActions.Length; i++)
        {
            clone.onEnterActions[i] = onEnterActions[i].Clone();
            clone.onEnterActions[i].parentState = clone;
        }
        for (int i = 0; i < actions.Length; i++)
        {
            clone.actions[i] = actions[i].Clone();
            clone.onEnterActions[i].parentState = clone;
        }
        for (int i = 0; i < onExitActions.Length; i++)
        {
            clone.onExitActions[i] = onExitActions[i].Clone();
            clone.onEnterActions[i].parentState = clone;
        }
        for (int i = 0; i < triggers.Length; i++)
        {
            clone.triggers[i] = triggers[i].Clone();
        }

        return clone;
    }
}
