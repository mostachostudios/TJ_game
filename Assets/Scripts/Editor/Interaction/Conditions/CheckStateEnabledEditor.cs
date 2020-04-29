using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(CheckStateEnabledCondition))]
public class CheckStateEnabledEditor : ConditionEditor
{
    private CheckStateEnabledCondition checkStateEnabledCondition;

    private StateMachine stateMachine;

    private string[] statesNamesList = new string[0];
    private int selectedStateIndex;

    protected override void Init()
    {
        checkStateEnabledCondition = (CheckStateEnabledCondition)target;

        if (checkStateEnabledCondition.state != null)
        {
            stateMachine = checkStateEnabledCondition.state.parentStateMachine;
            selectedStateIndex = Array.IndexOf(stateMachine.states, checkStateEnabledCondition.state);
        }
        else
        {
            stateMachine = null;
            selectedStateIndex = 0;
        }
    }


    protected override void DrawCondition()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        DisplayStateMachineInputBox();

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayStateMachineInputBox()
    {
        StateMachine oldStateMachine = stateMachine;

        stateMachine = (StateMachine)EditorGUILayout.ObjectField(stateMachine, typeof(StateMachine), true);

        if (stateMachine != null)
        {
            if (oldStateMachine != stateMachine)
            {
                selectedStateIndex = 0;
                checkStateEnabledCondition.state = null;
            }
            DisplayStatesSelector();
        }
    }

    private void DisplayStatesSelector()
    {
        UpdateStateNameListSelector();

        if (statesNamesList.Length > 0)
        {
            int oldSelectedStateIndex = selectedStateIndex;

            selectedStateIndex = EditorGUILayout.Popup(selectedStateIndex, statesNamesList);

            if (oldSelectedStateIndex != selectedStateIndex || checkStateEnabledCondition.state == null)
            {
                checkStateEnabledCondition.state = stateMachine.states[selectedStateIndex];
            }

            DisplayEnableDisableStateCheckBox();
        }
    }

    private void UpdateStateNameListSelector()
    {
        statesNamesList = stateMachine.states.Select(state => state.name).ToArray();
    }

    private void DisplayEnableDisableStateCheckBox()
    {
        checkStateEnabledCondition.expected = EditorGUILayout.Toggle(checkStateEnabledCondition.expected);
    }
}
