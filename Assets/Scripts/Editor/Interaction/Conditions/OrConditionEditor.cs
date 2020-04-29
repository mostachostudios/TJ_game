using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OrCondition))]
public class OrConditionEditor : ConditionEditor
{
    private OrCondition orCondition;

    public SerializedProperty childrenConditionsProperty;
    private const string onChildrenConditionsPropName = "conditions";

    private ConditionEditor[] subEditors = null;

    private Type[] conditionTypes;
    private string[] conditionTypeNames;
    private int selectedAllConditionsIndex = 0;

    protected override void Init()
    {
        orCondition = (OrCondition)target;

        childrenConditionsProperty = serializedObject.FindProperty(onChildrenConditionsPropName);
    }

    protected override void DrawCondition()
    {
        CheckAndCreateSubEditors(orCondition.conditions);

        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        
        ExpandedGUI();

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

    }

    private void ExpandedGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        //Color defaultColor = GUI.color;
        //GUI.color = Color.gray;
        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI();
        }
        //GUI.color = defaultColor;
        EditorGUILayout.EndVertical();
    }

    protected override void DrawConditionHeader()
    {
        UpdateConditionNamesArray();

        selectedAllConditionsIndex = EditorGUILayout.Popup(selectedAllConditionsIndex, conditionTypeNames, GUILayout.Width(200));

        if (GUILayout.Button("+", GUILayout.Width(buttonWidth)))
        {
            Type conditionType = conditionTypes[selectedAllConditionsIndex];
            Condition newCondition = ConditionEditor.CreateCondition(conditionType);

            // Only if editting scriptable object without state machine
            if (statesProperty == null)
            {
                AssetDatabase.AddObjectToAsset(newCondition, base.trigger);
                AssetDatabase.SaveAssets();
                //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/Triggers/" + trigger.name + ".asset");
            }

            childrenConditionsProperty.AddToObjectArray(newCondition);
        }

        base.DrawConditionHeader();
    }

    private void UpdateConditionNamesArray()
    {
        Type conditionType = typeof(Condition);

        Type[] allTypes = conditionType.Assembly.GetTypes();

        List<Type> conditionSubTypeList = new List<Type>();

        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].IsSubclassOf(conditionType) && !allTypes[i].IsAbstract)
            {
                conditionSubTypeList.Add(allTypes[i]);
            }
        }

        conditionTypes = conditionSubTypeList.ToArray();

        List<string> conditionTypeNameList = new List<string>();

        for (int i = 0; i < conditionTypes.Length; i++)
        {
            conditionTypeNameList.Add(conditionTypes[i].Name);
        }

        conditionTypeNames = conditionTypeNameList.ToArray();
    }

    private void CleanupSubEditors()
    {
        if (subEditors == null)
            return;

        for (int i = 0; i < subEditors.Length; i++)
        {
            DestroyImmediate(subEditors[i]);
        }

        subEditors = null;
    }

    private void CheckAndCreateSubEditors(Condition[] subEditorTargets)
    {
        if (subEditors != null && subEditors.Length == subEditorTargets.Length)
        {
            return;
        }

        CleanupSubEditors();

        subEditors = new ConditionEditor[subEditorTargets.Length];

        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i] = CreateEditor(subEditorTargets[i] as Condition) as ConditionEditor;
            SubEditorSetup(subEditors[i]);
        }
    }

    public void SubEditorSetup(ConditionEditor conditionEditor)
    {
        conditionEditor.trigger = trigger;
        conditionEditor.conditionEditors = subEditors;
        conditionEditor.conditionsProperty = childrenConditionsProperty;
    }
}
