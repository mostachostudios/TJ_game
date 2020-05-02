using UnityEngine;
using System;

public class SetAnimationAction : Action
{
    public Animator animator;
    public string animationParameterName;

    protected override bool StartDerived()
    {
        if (animator == null)
        {
            throw new UnityEngine.UnityException("Animator is null");
        }

        if (animationParameterName == null)
        {
            throw new UnityEngine.UnityException("Animator parameter name is null");
        }

        AnimatorControllerParameter animationParameter = Array.Find(animator.parameters, parameter => parameter.name == animationParameterName);

        if (animationParameter == null)
        {
            throw new UnityEngine.UnityException("Animator parameter couldn't be found");
        }

        switch (animationParameter.type)
        {
            case AnimatorControllerParameterType.Bool:
                {
                    animator.SetBool(animationParameter.nameHash, true);
                    break;
                }
            case AnimatorControllerParameterType.Trigger:
                {
                    animator.SetTrigger(animationParameter.nameHash);
                    break;
                }
        }

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
