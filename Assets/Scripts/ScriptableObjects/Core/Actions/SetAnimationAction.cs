using UnityEngine;

public class SetAnimationAction : Action
{
    public GameObject instance;
    public string parameterName;
    public bool isTrigger = false;
 
    private Animator animator;

    protected override bool StartDerived()
    {
        if(instance == null)
        {
            throw new UnityEngine.UnityException("Instance is null");
        }

        animator = instance.GetComponent<Animator>();

        if (animator == null)
        {
            throw new UnityEngine.UnityException("Instance does not have an Animator component");
        }

        if (isTrigger)
        {
            animator.SetTrigger(parameterName);
        }
        else
        {
            animator.SetBool(parameterName, true);
        }

        return true;
    }

    protected override bool UpdateDerived()
    {
        return true;
    }

    protected override Action CloneDerived()
    {
        SetAnimationAction clone = ScriptableObject.CreateInstance<SetAnimationAction>();
        clone.instance = this.instance;
        clone.parameterName = this.parameterName;
        clone.isTrigger = this.isTrigger;
        clone.animator = this.animator;

        return clone;
    }
}
