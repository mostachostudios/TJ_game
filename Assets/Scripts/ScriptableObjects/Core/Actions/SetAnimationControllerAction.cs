using UnityEngine;

public class SetAnimationControllerAction : Action
{
    public Animator animator;
    public RuntimeAnimatorController animatorController;

    protected override bool StartDerived()
    {
        if(animator == null)
        {
            throw new UnityEngine.UnityException("Instance is null");
        }

        if (animatorController== null)
        {
            throw new UnityEngine.UnityException("AnimatorController is null");
        }

        animator.runtimeAnimatorController = animatorController;
        animator.Play("Idle", 0);

        return false;
    }

    protected override bool UpdateDerived()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0);
    }

    protected override Action CloneDerived()
    {
        SetAnimationControllerAction clone = ScriptableObject.CreateInstance<SetAnimationControllerAction>();
        clone.animator = this.animator;
        clone.animatorController = this.animatorController;
        clone.animator = this.animator;
        
        return clone;
    }
}
