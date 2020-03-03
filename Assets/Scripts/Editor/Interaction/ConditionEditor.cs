using System;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(Condition), true)]
public class ConditionEditor : Editor
{
    public Trigger parentTrigger;
    public bool showCondition = true;

    public SerializedProperty statesProperty = null;
    public SerializedProperty conditionsProperty;
    public ConditionEditor[] conditionEditors;

    private Condition condition;

    protected const float buttonWidth = 30f;

    protected static readonly string[] fieldsToIgnore = new string[] { "m_Script" };

    private void OnEnable()
    {
        condition = target as Condition;
        Init();
    }

    protected virtual void Init() { }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();

        // Regex to change from "IsNearCondition" to "Is Near Condition"
        showCondition = EditorGUILayout.Foldout(showCondition, Regex.Replace(condition.GetType().Name, "(\\B[A-Z])", " $1"));

        DrawConditionHeader();

        EditorGUILayout.EndHorizontal();

        if (showCondition)
        {
            DrawCondition();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    public static Condition CreateCondition(Type conditionType)
    {
        return (Condition)CreateInstance(conditionType);
    }

    protected virtual void DrawConditionHeader()
    {
        UpDownArrowsGUI();

        if (GUILayout.Button("-", GUILayout.Width(buttonWidth)))
        {
            // This is only to edit it from scriptableobject without state machine
            AssetDatabase.RemoveObjectFromAsset(condition);
            AssetDatabase.SaveAssets();

            conditionsProperty.RemoveFromObjectArray(condition);
        }
    }

    protected virtual void DrawCondition()
    {
        DrawPropertiesExcluding(serializedObject, fieldsToIgnore);
    }

    protected void UpDownArrowsGUI()
    {
        int index = conditionsProperty.GetIndexFromObjectArray(condition);

        if (index == 0)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↑", GUILayout.Width(buttonWidth)))
        {
            ConditionEditor conditionEditor1 = conditionEditors[index];
            conditionEditors[index] = conditionEditors[index - 1];
            conditionEditors[index - 1] = conditionEditor1;

            conditionsProperty.SwapInObjectArray(index, index - 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }

        if (index == (conditionsProperty.arraySize - 1))
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↓", GUILayout.Width(buttonWidth)))
        {
            ConditionEditor conditionEditor1 = conditionEditors[index];
            conditionEditors[index] = conditionEditors[index + 1];
            conditionEditors[index + 1] = conditionEditor1;

            conditionsProperty.SwapInObjectArray(index, index + 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }
    }
}
