using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraAction : Action
{
    public enum CameraType { Source, Target };

    public Camera targetCamera;
    public float timeSeconds = .0f;
    public CameraType makesMovement = CameraType.Source;

    private Camera sourceCamera;
    private Transform transformToChange;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private Vector3 originalPosition;
    private Vector3 originalDirection;
    private float currentTimeSeconds = .0f;


    protected override bool StartDerived()
    {
        if(targetCamera == null || Camera.current == null)
        {
            return false;
        }

        // Do nothing
        if(targetCamera == Camera.current)
        {
            return true;
        }

        sourceCamera = Camera.current;

        targetPosition = targetCamera.transform.position;
        targetDirection = targetCamera.transform.forward;
        originalPosition = sourceCamera.transform.position;
        originalDirection = sourceCamera.transform.forward;

        if (makesMovement == CameraType.Source) // Source camera makes movement
        {
            transformToChange = sourceCamera.transform;
        }
        else // Target camera makes movement
        {
            transformToChange = targetCamera.transform;
            transformToChange.position = originalPosition;
            transformToChange.forward = originalDirection;
            targetCamera.gameObject.SetActive(true);
            sourceCamera.gameObject.SetActive(false);
        }

        currentTimeSeconds = .0f;

        return false;
    }

    protected override bool UpdateDerived()
    {
        float t = (timeSeconds != .0f) ? (currentTimeSeconds / timeSeconds) : 1.0f;

        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        transformToChange.position = Vector3.Lerp(originalPosition, targetPosition, t);
        transformToChange.forward = Vector3.Slerp(originalDirection, targetDirection, t);

        if (currentTimeSeconds == timeSeconds)
        {
            if(makesMovement == CameraType.Source)
            {
                targetCamera.gameObject.SetActive(true);
                sourceCamera.gameObject.SetActive(false);
            }

            return true;
        }

        return false;
    }

    protected override Action CloneDerived()
    {
        SetCameraAction clone = ScriptableObject.CreateInstance<SetCameraAction>();

        clone.targetCamera = this.targetCamera;
        clone.currentTimeSeconds = this.currentTimeSeconds;
        clone.makesMovement = this.makesMovement;

        return clone;
    }
}
