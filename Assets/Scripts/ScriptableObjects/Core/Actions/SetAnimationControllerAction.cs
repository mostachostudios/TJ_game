using UnityEngine;

public class SetAnimationControllerAction : Action
{
    public GameObject instance;
    public RuntimeAnimatorController animatorController;

    private Animator animator;

    protected override bool StartDerived()
    {
        if(instance == null)
        {
            throw new UnityEngine.UnityException("Instance is null");
        }
        if (animatorController== null)
        {
            throw new UnityEngine.UnityException("AnimatorController is null");
        }

        animator = instance.GetComponent<Animator>();

        if (animator == null)
        {
            throw new UnityEngine.UnityException("Instance does not have an Animator component");
        }

        animator.runtimeAnimatorController = animatorController;
        animator.Play("Idle", 0);

        return false;
    }

    protected override bool UpdateDerived()
    {
        //animator.Update(Time.deltaTime);
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0);
    }

    protected override Action CloneDerived()
    {
        SetAnimationControllerAction clone = ScriptableObject.CreateInstance<SetAnimationControllerAction>();
        clone.instance = this.instance;
        clone.animatorController = this.animatorController;
        clone.animator = this.animator;
        
        return clone;
    }
}
