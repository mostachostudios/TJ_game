using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public Trigger parentTrigger;

    abstract public bool Check();
    public abstract Condition Clone();
}
