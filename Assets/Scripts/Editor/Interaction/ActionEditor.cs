using System;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(Action), true)]
public class ActionEditor : Editor
{
    public bool showAction = true;

    public SerializedProperty actionsProperty;
    public ActionEditor[] actionEditors;

    private Action action;

    private const float buttonWidth = 30f;

    private static readonly string[] fieldsToIgnore = new string[] { "waitToFinish", "m_Script" };

    private void OnEnable()
    {
        action = (Action)target;

        Init();
    }

    protected virtual void Init() { }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();

        // Regex to change from "PlayAudioAction" to "Play Audio Action"
        showAction = EditorGUILayout.Foldout(showAction, Regex.Replace(action.GetType().Name, "(\\B[A-Z])", " $1"));

        action.waitToFinish = GUILayout.Toggle(action.waitToFinish, new GUIContent("Wait to finish", "Wait for this action to finish"), GUILayout.Width(100));

        UpDownArrowsGUI();

        if (GUILayout.Button("-", GUILayout.Width(buttonWidth)))
        {
            // This is only to edit it from scriptableobject without state machine
            AssetDatabase.RemoveObjectFromAsset(action);
            AssetDatabase.SaveAssets();

            actionsProperty.RemoveFromObjectArray(action);
        }
        EditorGUILayout.EndHorizontal();

        if (showAction)
        {
            DrawAction();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    public void UpDownArrowsGUI()
    {
        int index = actionsProperty.GetIndexFromObjectArray(action);

        if (index == 0)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↑", GUILayout.Width(buttonWidth)))
        {
            ActionEditor actionEditor1 = actionEditors[index];
            actionEditors[index] = actionEditors[index - 1];
            actionEditors[index - 1] = actionEditor1;

            actionsProperty.SwapInObjectArray(index, index - 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }

        if (index == (actionsProperty.arraySize - 1))
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↓", GUILayout.Width(buttonWidth)))
        {
            ActionEditor actionEditor1 = actionEditors[index];
            actionEditors[index] = actionEditors[index + 1];
            actionEditors[index + 1] = actionEditor1;

            actionsProperty.SwapInObjectArray(index, index + 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }
    }

    public static Action CreateAction(Type actionType)
    {
        return (Action)CreateInstance(actionType);
    }

    // Can be overriden
    protected virtual void DrawAction()
    {
        DrawPropertiesExcluding(serializedObject, fieldsToIgnore);
    }
}
