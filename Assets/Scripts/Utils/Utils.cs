using UnityEngine;
using System;

public class Utils
{
    public static void SetAnimatorParameterByName(Animator animator, string parameterName)
    {
        if (animator == null)
        {
            throw new UnityEngine.UnityException("Animator is null");
        }

        if (parameterName == null)
        {
            throw new UnityEngine.UnityException("Animator parameter name is null");
        }

        foreach (var parameter in animator.parameters)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Bool:
                    {
                        animator.SetBool(parameter.nameHash, false);
                        break;
                    }
                default: // Other types don't need to be checked for the moment
                    break;
            }
        }

        AnimatorControllerParameter animationParameter = Array.Find(animator.parameters, parameter => parameter.name == parameterName);

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
    }

}
