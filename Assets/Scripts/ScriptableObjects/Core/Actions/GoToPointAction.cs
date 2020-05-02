using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO, rework this action to make character walk, run, etc..
public class GoToPointAction : Action
{
    public Transform targetEntity = null;
    public Transform targetPoint;
    public float timeSeconds = 1.0f;
    public bool lookAtTarget = true;

    private Vector3 sourcePosition;
    private Vector3 path;
    private float currentTimeSeconds;

    protected override bool StartDerived()
    {
        sourcePosition = targetEntity.position;
        path = targetPoint.position - targetEntity.position;
        currentTimeSeconds = 0;
        if (lookAtTarget) 
        { 
            targetEntity.LookAt(targetPoint); // To be changed by a smooth rotation started here and performed in 'UpdateDerived'
        } 

        return false;
    }

    protected override bool UpdateDerived()
    {
        // Rename this class -> MOVE, and create another one that makes character walk, run...
        float t = (timeSeconds != .0f) ? (currentTimeSeconds / timeSeconds) : 1.0f;
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        targetEntity.position = sourcePosition + path * t;

        return currentTimeSeconds == timeSeconds;
    }

    protected override Action CloneDerived()
    {
        GoToPointAction clone = new GoToPointAction();
        clone.targetEntity = null;
        clone.targetPoint = this.targetPoint;
        clone.timeSeconds = this.timeSeconds;

        return clone;
    }
}
