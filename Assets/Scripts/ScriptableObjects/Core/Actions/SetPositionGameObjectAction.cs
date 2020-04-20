using UnityEngine;

public class SetPositionGameObjectAction : Action
{
    public GameObject instance;
    public Vector3 targetPosition;

    protected override bool StartDerived()
    {
        instance.transform.position = targetPosition;
        return false;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetPositionGameObjectAction clone = ScriptableObject.CreateInstance<SetPositionGameObjectAction>();

        clone.instance = this.instance;
        clone.targetPosition = this.targetPosition;

        return clone;
    }

}
