using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(SetAnimationAction))]
public class SetAnimationActionEditor : ActionEditor
{
    private SetAnimationAction setAnimationAction;

    private const string animatorPropName = "animator";

    private SerializedProperty animatorProperty;

    private string[] parametersNamesList = new string[0];
    private int selectedParameterIndex;

    protected override void Init()
    {
        setAnimationAction = (SetAnimationAction)target;

        animatorProperty = serializedObject.FindProperty(animatorPropName);

        if(setAnimationAction.animator != null && setAnimationAction.animationParameterName != null)
        {
            string[] parameterNames = setAnimationAction.animator.parameters.Select(parameter => parameter.name).ToArray();
            selectedParameterIndex = Array.IndexOf(parameterNames, setAnimationAction.animationParameterName);
        }

        UpdateParameterNamesList();
    }

    protected override void DrawAction()
    {
        Animator oldAnimator = setAnimationAction.animator;

        serializedObject.Update();
        EditorGUILayout.PropertyField(animatorProperty);
        serializedObject.ApplyModifiedProperties();

        if(oldAnimator != setAnimationAction.animator)
        {
            selectedParameterIndex = 0;
            setAnimationAction.animationParameterName = null;
        }

        UpdateParameterNamesList();

        if (parametersNamesList.Length > 0)
        {
            int oldSelectedParameterIndex = selectedParameterIndex;
            selectedParameterIndex = EditorGUILayout.Popup("AnimatorParameter", selectedParameterIndex, parametersNamesList);
            if(oldSelectedParameterIndex != selectedParameterIndex || setAnimationAction.animationParameterName == null)
            {
                setAnimationAction.animationParameterName = setAnimationAction.animator.parameters[selectedParameterIndex].name;
            }
        }
    }

    private void UpdateParameterNamesList()
    {
        if (setAnimationAction != null && setAnimationAction.animator != null)
        {
            parametersNamesList = setAnimationAction.animator.parameters.Select(parameter => parameter.name).ToArray();
        }
        else
        {
            parametersNamesList = new string[0];
            setAnimationAction.animationParameterName = null;
        }
    }
}