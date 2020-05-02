using UnityEngine;

public class CameraFocusAction : Action
{
    public Camera camera;
    public Transform target;
    public Vector3 offsetTarget = new Vector3(.0f, .0f, .0f);
    public float cameraDistance = 3.0f;
    public float timeSeconds = .0f;
    public bool forceForward = false;
    public float degreesToRotateOrbit = .0f;

    private Vector3 originalPosition;
    private Vector3 originalDirection;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private float currentTimeSeconds;

    protected override bool StartDerived()
    {
        if (camera == null)
        {
            throw new UnityEngine.UnityException("Camera is null");
        }

        if (target == null)
        {
            throw new UnityEngine.UnityException("Target is null");
        }

        originalPosition = camera.transform.position;
        originalDirection = camera.transform.forward;

        if (forceForward)
        {
            // Focus looking at target's forward
            targetPosition = target.position + target.forward * cameraDistance + offsetTarget;
        }
        else
        {
            // Focus looking from original camera position
            Vector3 direction = (camera.transform.position - target.position).normalized;
            targetPosition = camera.transform.position + direction * cameraDistance + offsetTarget;
        }

        // Rotate in orbit
        targetPosition = Quaternion.Euler(Vector3.up * degreesToRotateOrbit) * (targetPosition - target.position) + target.position;

        // Final orientation
        targetDirection = (target.position + offsetTarget - targetPosition).normalized;

        currentTimeSeconds = .0f;

        return UpdateDerived();
    }

    protected override bool UpdateDerived()
    {
        float t = (timeSeconds != .0f) ? (currentTimeSeconds / timeSeconds) : 1.0f;

        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        camera.transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
        camera.transform.forward = Vector3.Slerp(originalDirection, targetDirection, t);

        return currentTimeSeconds == timeSeconds;
    }

    protected override Action CloneDerived()
    {
        CameraFocusAction clone = ScriptableObject.CreateInstance<CameraFocusAction>();
        clone.camera = this.camera;
        clone.target = this.target;
        clone.offsetTarget = this.offsetTarget;
        clone.cameraDistance = this.cameraDistance;
        clone.timeSeconds = this.timeSeconds;
        clone.forceForward = this.forceForward;
        clone.degreesToRotateOrbit = this.degreesToRotateOrbit;

        return clone;
    }
}
