using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    protected bool started   = false; 
    //protected bool finished = false; // Not required by now

    public bool waitToFinish = true;

    public bool HasStarted() { return started; }

    public void Reset()
    {
        started = false;
        //finished = false;
    }

    public bool Start()
    {
        started = true;

        return (!waitToFinish || StartDerived());
    }

    public bool Update()
    {
        return UpdateDerived();
    }

    public Action Clone()
    {
        Action clone = CloneDerived();
        clone.started = this.started;
        clone.waitToFinish = this.waitToFinish;

        return clone;
    }

    // Making it pure abstract to avoid forgetting implement it (even if we waste a bit of performance)
    // protected abstract void ResetDerived(); // Maybe reset not required, let's see what future requires
    protected abstract bool StartDerived();
    protected abstract bool UpdateDerived();
    protected abstract Action CloneDerived();
}
