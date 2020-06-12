using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsFarCondition : Condition
{
    public Transform origin;
    public Transform target;
    public float radius;

    public override bool Check()
    {
        return Vector3.Distance(origin.position, target.position) >= radius;
    }

    public override Condition Clone()
    {
        IsFarCondition clone = new IsFarCondition();

        clone.origin = this.origin;
        clone.target = this.target;
        clone.radius = this.radius;

        return clone;
    }
}
