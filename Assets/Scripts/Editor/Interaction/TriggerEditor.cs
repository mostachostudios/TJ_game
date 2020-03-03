using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Trigger))]
public class TriggerEditor : EditorWithSubEditors<ConditionEditor, Condition>
{
    private Trigger trigger;

    public SerializedProperty statesProperty = null;
    public SerializedProperty triggersProperty = null;
    private SerializedProperty nameProperty;
    public SerializedProperty conditionsProperty;

    public TriggerEditor[] triggerEditors;

    private const string namePropName = "m_Name";
    private const string conditionsPropName = "conditions";

    private Type[] conditionTypes;
    private string[] conditionTypeNames;
    private int selectedIndex;

    private const float dropAreaHeight = 50f;
    private const float controlSpacing = 5f;

    private const float removeTriggerButtonWidth = 30f;

    private readonly float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

    private void OnEnable()
    {
        trigger = (Trigger)target;

        if(trigger == null)
        {
            throw new UnityException("Trigger is null");
        }

        nameProperty = serializedObject.FindProperty(namePropName);
        conditionsProperty = serializedObject.FindProperty(conditionsPropName);

        if (conditionsProperty == null)
        {
            throw new UnityException("conditionsProperty is null");
        }
    }

    private void OnDisable()
    {
        CleanupEditors();
    }

    public override void SubEditorSetup(ConditionEditor conditionEditor)
    {
        conditionEditor.parentTrigger = trigger;
        conditionEditor.statesProperty = statesProperty;
        conditionEditor.conditionEditors = subEditors;
        conditionEditor.conditionsProperty = conditionsProperty;
    }

    public override void OnInspectorGUI()
    {
        CheckAndCreateSubEditors(trigger.conditions);

        UpdateConditionNamesArray();

        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();

        conditionsProperty.isExpanded = EditorGUILayout.Foldout(conditionsProperty.isExpanded, nameProperty.stringValue);

        if (triggersProperty != null)
        {
            UpDownArrowsGUI();

            if (GUILayout.Button("-", GUILayout.Width(removeTriggerButtonWidth)))
            {
                AssetDatabase.RemoveObjectFromAsset(trigger);
                AssetDatabase.SaveAssets();
                triggersProperty.RemoveFromObjectArray(trigger);
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;

        if (conditionsProperty.isExpanded)
        {
            ExpandedGUI();
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void ExpandedGUI()
    {
        EditorGUILayout.Space();

        // If it already exists an asset with that name, back to old name and show warning
        // Only for scriptableobject editting, not valid in statemachine
        if (statesProperty != null || triggersProperty != null)
        {
            string oldTriggerName = trigger.name;
            trigger.name = EditorGUILayout.TextField("Name", trigger.name);

            if (triggersProperty != null && oldTriggerName != trigger.name)
            {
                /*
                // Check if exists in assets
                string[] allTriggers = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/Scripts/ScriptableObjects/Core/Triggers" });
                if (Array.Exists(allTriggers, s => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(s)) == trigger.name))
                {
                    trigger.name = oldTriggerName;
                    EditorUtility.DisplayDialog("Duplicated name Error", "Name already exists in '/Core/Triggers/...', reverted to previous name. Please enter a new name", "ok");
                }*/
                // Check if exists in triggers list
                int occurences = 0;
                for(int i = 0; i < triggersProperty.arraySize; i++)
                {
                    string currentTriggerName = triggersProperty.GetArrayElementAtIndex(i).objectReferenceValue.name;
                    if (trigger.name == currentTriggerName)
                    {
                        occurences++;
                        if(occurences == 2)
                        {
                            trigger.name = oldTriggerName;
                            EditorUtility.DisplayDialog("Duplicated name Error", "Name already exists in triggers list, reverted to previous name. Please enter a new name", "ok");
                            break;
                        }
                    }
                }
            }
        }

        EditorGUILayout.LabelField("Conditions:");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        Color defaultColor = GUI.color;
        GUI.color = Color.gray;
        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI();
        }
        GUI.color = defaultColor;
        EditorGUILayout.EndVertical();

        if (trigger.conditions.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        EditorUtils.DrawHorizontalLine(Color.gray, 2, 10, 10);

        Rect fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(dropAreaHeight + verticalSpacing));

        Rect leftAreaRect = fullWidthRect;
        leftAreaRect.y += verticalSpacing * 0.5f;
        leftAreaRect.width *= 0.5f;
        leftAreaRect.width -= controlSpacing * 0.5f;
        leftAreaRect.height = dropAreaHeight;

        Rect rightAreaRect = leftAreaRect;
        rightAreaRect.x += rightAreaRect.width + controlSpacing;

        defaultColor = GUI.color;
        GUI.color = Color.grey; // Change background dragging box color
        TypeSelectionGUI(leftAreaRect);
        DragAndDropAreaGUI(rightAreaRect);
        GUI.color = defaultColor; // Restore GUI default color

        DraggingAndDropping(rightAreaRect, this);
    }


    public void UpDownArrowsGUI()
    {
        int index = triggersProperty.GetIndexFromObjectArray(trigger);

        if (index == 0)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↑", GUILayout.Width(removeTriggerButtonWidth)))
        {
            TriggerEditor triggerEditor1 = triggerEditors[index];
            triggerEditors[index] = triggerEditors[index - 1];
            triggerEditors[index - 1] = triggerEditor1;

            triggersProperty.SwapInObjectArray(index, index - 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }

        if (index == (triggersProperty.arraySize - 1))
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↓", GUILayout.Width(removeTriggerButtonWidth)))
        {
            TriggerEditor triggerEditor1 = triggerEditors[index];
            triggerEditors[index] = triggerEditors[index + 1];
            triggerEditors[index + 1] = triggerEditor1;

            triggersProperty.SwapInObjectArray(index, index + 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }
    }

    private void TypeSelectionGUI(Rect containingRect)
    {
        Rect topHalf = containingRect;
        topHalf.height *= 0.5f;

        Rect bottomHalf = topHalf;
        bottomHalf.y += bottomHalf.height;

        selectedIndex = EditorGUI.Popup(topHalf, selectedIndex, conditionTypeNames);

        if (GUI.Button(bottomHalf, "Add Selected Condition"))
        {
            Type conditionType = conditionTypes[selectedIndex];
            Condition newCondition = ConditionEditor.CreateCondition(conditionType);

            // Only if editting scriptable object without state machine
            if (statesProperty == null)
            {
                AssetDatabase.AddObjectToAsset(newCondition, trigger);
                AssetDatabase.SaveAssets();
                //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/Triggers/" + trigger.name + ".asset");
            }

            conditionsProperty.AddToObjectArray(newCondition);
        }
    }

    private static void DraggingAndDropping(Rect dropArea, TriggerEditor editor)
    {
        Event currentEvent = Event.current;

        if (!dropArea.Contains(currentEvent.mousePosition))
            return;

        switch (currentEvent.type)
        {
            case EventType.DragUpdated:

                DragAndDrop.visualMode = IsDragValid() ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                currentEvent.Use();

                break;
            case EventType.DragPerform:

                DragAndDrop.AcceptDrag();

                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;

                    Type conditionType = script.GetClass();

                    Condition newCondition = ConditionEditor.CreateCondition(conditionType);

                    // Only if editting scriptable object without state machine
                    if (editor.statesProperty == null)
                    {
                        AssetDatabase.AddObjectToAsset(newCondition, editor.trigger);
                        AssetDatabase.SaveAssets();
                        //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/Triggers/" + editor.trigger.name + ".asset");
                    }

                    editor.conditionsProperty.AddToObjectArray(newCondition);
                }

                currentEvent.Use();

                break;
        }
    }

    private static bool IsDragValid()
    {
        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
        {
            if (DragAndDrop.objectReferences[i].GetType() != typeof(MonoScript))
                return false;

            MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;
            Type scriptType = script.GetClass();

            if (!scriptType.IsSubclassOf(typeof(Condition)))
                return false;

            if (scriptType.IsAbstract)
                return false;
        }

        return true;
    }

    private static void DragAndDropAreaGUI(Rect containingRect)
    {
        GUIStyle centredStyle = GUI.skin.box;
        centredStyle.alignment = TextAnchor.MiddleCenter;
        centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

        GUI.Box(containingRect, "Drop new Condition here", centredStyle);
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
}
