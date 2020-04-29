using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraAction : Action
{
    public Camera camera;
    public float timeSeconds = .0f;

    private Vector3 originalPosition;
    private Vector3 originalDirection;
    private float currentTimeSeconds = .0f;


    protected override bool StartDerived()
    {
        if(camera == Camera.current)
        {
            return true;
        }

        originalPosition = Camera.current.transform.position;
        originalPosition = Camera.current.transform.forward;

        currentTimeSeconds = .0f;

        return false;
    }

    protected override bool UpdateDerived()
    {
        float t = (timeSeconds != .0f) ? (currentTimeSeconds / timeSeconds) : 1.0f;

        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        camera.transform.position = Vector3.Lerp(originalPosition, camera.transform.position, t);
        camera.transform.forward = Vector3.Slerp(originalDirection, camera.transform.forward, t);

        if (currentTimeSeconds == timeSeconds)
        {
            Camera oldCamera = Camera.current;
            camera.enabled = true;
            oldCamera.enabled = false;
            return true;
        }

        return false;
    }

    protected override Action CloneDerived()
    {
        SetCameraAction clone = ScriptableObject.CreateInstance<SetCameraAction>();

        clone.camera = this.camera;
        clone.currentTimeSeconds = this.currentTimeSeconds;

        return clone;
    }
}
