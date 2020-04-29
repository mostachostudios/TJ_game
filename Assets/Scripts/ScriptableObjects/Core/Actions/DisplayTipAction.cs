using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
public class DisplayTipAction : Action
{
    // Tip tipData; // TODO
    public float timeSeconds = .0f;
    public KeyCode key = KeyCode.None;

    private float currentTimeSeconds;

    protected override bool StartDerived()
    {
        // TODO: display tip

        currentTimeSeconds = .0f;

        return .0f == timeSeconds && key == KeyCode.None;
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        return currentTimeSeconds == timeSeconds && (key == KeyCode.None || Input.GetKeyDown(key));
    }

    protected override Action CloneDerived()
    {
        DisplayTipAction clone = ScriptableObject.CreateInstance<DisplayTipAction>();

        clone.timeSeconds = this.timeSeconds;
        clone.key = this.key;

        return clone;
    }
}
