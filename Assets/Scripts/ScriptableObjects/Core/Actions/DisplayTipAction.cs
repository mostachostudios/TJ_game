using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTipAction : Action
{
    public string tip;
    public float timeSeconds = .0f;
    public KeyCode key = KeyCode.None;

    private float currentTimeSeconds;
    private Script_UIController script_UIController;

    private float epsilon = 0.05f;
    protected override bool StartDerived()
    {
        script_UIController = FindObjectOfType<Script_UIController>();
        script_UIController.SetTextMessage(tip, true);
        script_UIController.EraseTextMessage(timeSeconds - epsilon);
        //A small epsilon time has been substracted from time, in case a DisplayTipAction is just called after this one,
        // maybe resulting in deleting it just after writing the message, depeding on Coroutines execution order

        currentTimeSeconds = .0f;

        //return .0f == timeSeconds && key == KeyCode.None;
        return false;
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        if(currentTimeSeconds == timeSeconds && (key == KeyCode.None || Input.GetKeyDown(key)))
        {
            return true;
        }

        return false;
    }

    protected override Action CloneDerived()
    {
        DisplayTipAction clone = ScriptableObject.CreateInstance<DisplayTipAction>();

        clone.tip = this.tip;
        clone.timeSeconds = this.timeSeconds;
        clone.key = this.key;

        return clone;
    }
}
