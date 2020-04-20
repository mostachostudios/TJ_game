using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(EnableDisableStateAction))]
public class EnableDisableStateActionEditor : ActionEditor
{
    private EnableDisableStateAction enableDisableStateAction;

    //private const string statePropName = "state";

    //private SerializedProperty stateProperty;

    private StateMachine stateMachine;

    private string[] statesNamesList = new string[0];
    private int selectedStateIndex;

    protected override void Init()
    {
        enableDisableStateAction = (EnableDisableStateAction)target;

        //stateProperty = serializedObject.FindProperty(statePropName);

        if (enableDisableStateAction.state != null)
        {
            stateMachine = enableDisableStateAction.state.parentStateMachine;
            selectedStateIndex = Array.IndexOf(stateMachine.states, enableDisableStateAction.state);
        }
        else
        {
            stateMachine = null;
            selectedStateIndex = 0;
        }
    }

    protected override void DrawAction()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        DisplayStateMachineInputBox();

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayStateMachineInputBox()
    {
        StateMachine oldStateMachine = stateMachine;

        stateMachine = (StateMachine)EditorGUILayout.ObjectField(stateMachine, typeof(StateMachine), true);

        if(stateMachine != null)
        {
            if(oldStateMachine != stateMachine)
            {
                selectedStateIndex = 0;
                enableDisableStateAction.state = null;
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

            if(oldSelectedStateIndex != selectedStateIndex || enableDisableStateAction.state == null)
            {
                enableDisableStateAction.state = stateMachine.states[selectedStateIndex];
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
        enableDisableStateAction.enable = EditorGUILayout.Toggle(enableDisableStateAction.enable);
    }
}