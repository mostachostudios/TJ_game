using UnityEngine;

public class SetAnimationAction : Action
{
    public Animator animator;
    public string animationParameterName;

    protected override bool StartDerived()
    {
        Utils.SetAnimatorParameterByName(animator, animationParameterName);

        return false;
    }

    protected override bool UpdateDerived()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0);
    }

    protected override Action CloneDerived()
    {
        SetAnimationAction clone = ScriptableObject.CreateInstance<SetAnimationAction>();
        clone.animator = this.animator;
        clone.animationParameterName = this.animationParameterName;

        return clone;
    }
}
