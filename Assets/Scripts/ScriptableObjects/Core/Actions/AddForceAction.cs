using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceAction : Action
{
    public Rigidbody rigidbody;
    public float strength;
    protected override bool StartDerived()
    {
        rigidbody.AddForceAtPosition(rigidbody.transform.forward * strength, rigidbody.transform.position);

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        AddForceAction clone = ScriptableObject.CreateInstance<AddForceAction>();

        return clone;
    }

}
