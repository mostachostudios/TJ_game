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

    protected override bool StartDerived()
    {
        script_UIController = GameObject.FindWithTag("UI").GetComponent<Script_UIController>();
        script_UIController.SetTextMessage(tip, true);

        currentTimeSeconds = .0f;

        //return .0f == timeSeconds && key == KeyCode.None;
        return false;
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        if(currentTimeSeconds == timeSeconds && (key == KeyCode.None || Input.GetKeyDown(key)))
        {
            script_UIController.EraseTextMessage();
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
