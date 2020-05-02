using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : Action
{
    public float timeSeconds = .0f;
    private float currentTimeSeconds;

    protected override bool StartDerived()
    {
        currentTimeSeconds = .0f;

        return false;
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        return currentTimeSeconds == timeSeconds;
    }

    protected override Action CloneDerived()
    {
        WaitAction clone = ScriptableObject.CreateInstance<WaitAction>();

        return clone;
    }
}
