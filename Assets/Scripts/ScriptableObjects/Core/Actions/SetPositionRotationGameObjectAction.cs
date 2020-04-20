using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionRotationGameObjectAction : Action
{
    public GameObject instance;
    public Vector3 targetPosition;
    public Vector3 targetRotation;

    protected override bool StartDerived()
    {
        instance.transform.position = targetPosition;
        instance.transform.rotation = Quaternion.Euler(targetRotation);
        return false;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetPositionRotationGameObjectAction clone = ScriptableObject.CreateInstance<SetPositionRotationGameObjectAction>();

        clone.instance = this.instance;
        clone.targetPosition = this.targetPosition;
        clone.targetRotation = this.targetRotation;

        return clone;
    }
}
