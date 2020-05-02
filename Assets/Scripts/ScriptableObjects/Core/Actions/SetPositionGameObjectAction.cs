using UnityEngine;

public class SetPositionGameObjectAction : Action
{
    public enum TargetType { VectorSet, VectorAdd, Transform };

    public GameObject instance;
    public Vector3 targetPosition;
    public Transform targetTransform;
    public float timeSeconds = .0f;
    public TargetType targetType = TargetType.Transform;

    private Vector3 originalPosition;
    private float currentTimeSeconds;

    protected override bool StartDerived()
    {
        if (targetType == TargetType.Transform)
        {
            targetPosition = targetTransform.position;
        }

        originalPosition = instance.transform.position;
        
        if(targetType == TargetType.VectorAdd)
        {
            targetPosition += originalPosition;
        }

        currentTimeSeconds = .0f;

        return UpdateDerived();
    }

    protected override bool UpdateDerived()
    {
        float t = (timeSeconds != .0f) ? (currentTimeSeconds / timeSeconds) : 1.0f;

        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        instance.transform.position = Vector3.Lerp(originalPosition, targetPosition, t);

        return currentTimeSeconds == timeSeconds;
    }

    protected override Action CloneDerived()
    {
        SetPositionGameObjectAction clone = ScriptableObject.CreateInstance<SetPositionGameObjectAction>();

        clone.instance = this.instance;
        clone.targetPosition = this.targetPosition;
        clone.targetTransform = this.targetTransform;
        clone.timeSeconds = this.timeSeconds;
        clone.targetType = this.targetType;

        return clone;
    }

}
