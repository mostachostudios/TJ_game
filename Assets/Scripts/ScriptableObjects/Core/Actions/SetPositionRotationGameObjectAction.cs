using UnityEngine;

public class SetPositionRotationGameObjectAction : Action
{
    public enum TargetType { VectorSet, VectorAdd, Transform };

    public GameObject instance;
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public Transform targetTransform;
    public float timeSeconds = .0f;
    public TargetType targetPositionType = TargetType.Transform;
    public TargetType targetRotationType = TargetType.Transform;

    private Vector3 originalPosition;
    private Vector3 originalDirection;
    private Vector3 targetDirection;
    private float currentTimeSeconds;

    protected override bool StartDerived()
    {
        if(targetPositionType == TargetType.Transform)
        {
            targetPosition = targetTransform.position;
        }
        if (targetRotationType == TargetType.Transform)
        {
            targetRotation = targetTransform.rotation.eulerAngles;
        }

        originalPosition = instance.transform.position;
        originalDirection = instance.transform.forward;

        if (targetPositionType == TargetType.VectorAdd)
        {
            targetPosition += originalPosition;
        }
        if (targetRotationType == TargetType.VectorAdd)
        {
            targetRotation += Quaternion.FromToRotation(Vector3.forward, originalDirection).eulerAngles;
        }

        targetDirection = Quaternion.Euler(targetRotation) * Vector3.forward;

        currentTimeSeconds = .0f;

        return UpdateDerived();
    }

    protected override bool UpdateDerived()
    {
        float t = (timeSeconds != .0f) ? (currentTimeSeconds / timeSeconds) : 1.0f;

        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        instance.transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
        instance.transform.forward = Vector3.Slerp(originalDirection, targetDirection, t);

        return currentTimeSeconds == timeSeconds;
    }

    protected override Action CloneDerived()
    {
        SetPositionRotationGameObjectAction clone = ScriptableObject.CreateInstance<SetPositionRotationGameObjectAction>();

        clone.instance = this.instance;
        clone.targetPosition = this.targetPosition;
        clone.targetRotation = this.targetRotation;
        clone.targetTransform = this.targetTransform;
        clone.timeSeconds = this.timeSeconds;
        clone.targetPositionType = this.targetPositionType;
        clone.targetRotationType = this.targetRotationType;

        return clone;
    }
}
