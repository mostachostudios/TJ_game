using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(State))]
public class StateEditor : EditorWithSubEditorsTwoTypes31<ActionEditor, Action, TriggerEditor, Trigger>
{
    private State state;

    public StateEditor[] stateEditors;
    public SerializedProperty statesProperty = null;
    private SerializedProperty nameProperty;
    public SerializedProperty onEnterProperty;
    public SerializedProperty actionsProperty;
    public SerializedProperty onExitProperty;
    public SerializedProperty triggersProperty;

    private const string namePropName = "m_Name";
    private const string onEnterActionsPropName = "onEnterActions";
    private const string actionsPropName = "actions";
    private const string onExitActionsPropName = "onExitActions";
    private const string triggersPropName = "triggers";

    private Type[] actionTypes;
    private string[] actionTypeNames;
    private int selectedActionIndex;

    private int selectedActionSectionIndex = 1;
    private string[] actionSectionsNames = { onEnterActionsPropName, actionsPropName, onExitActionsPropName };

    private string[] availableTriggerTypeNames = new string[0];
    private int availableTriggersIndex;

    private const float dropAreaHeight = 50f;
    private const float controlSpacing = 5f;

    private const float stateButtonWidth = 30f;
    private const float removeStateButtonWidth = 30f;

    private void OnEnable()
    {
        state = (State)target;

        nameProperty = serializedObject.FindProperty(namePropName);
        onEnterProperty = serializedObject.FindProperty(onEnterActionsPropName);
        actionsProperty = serializedObject.FindProperty(actionsPropName);
        onExitProperty = serializedObject.FindProperty(onExitActionsPropName);
        triggersProperty = serializedObject.FindProperty(triggersPropName);
    }

    private void OnDisable()
    {
        CleanupEditors();
    }

    public override void SubEditorSetup(ActionEditor actionEditor)
    {
        if(Array.Exists(state.onEnterActions, a => a == actionEditor.target))
        {
            actionEditor.actionEditors = subEditors1[0];
            actionEditor.actionsProperty = onEnterProperty;
        }
        else if (Array.Exists(state.actions, a => a == actionEditor.target))
        {
            actionEditor.actionEditors = subEditors1[1];
            actionEditor.actionsProperty = actionsProperty;
        }
        else if (Array.Exists(state.onExitActions, a => a == actionEditor.target))
        {
            actionEditor.actionEditors = subEditors1[2];
            actionEditor.actionsProperty = onExitProperty;
        }
    }

    public override void SubEditorSetup(TriggerEditor triggerEditor)
    {
        triggerEditor.triggerEditors = subEditors2;
        triggerEditor.triggersProperty = triggersProperty;
        triggerEditor.statesProperty = statesProperty;
    }

    public override void OnInspectorGUI()
    {
        CheckAndCreateSubEditors(state.onEnterActions, state.actions, state.onExitActions, state.triggers);

        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();

        actionsProperty.isExpanded = EditorGUILayout.Foldout(actionsProperty.isExpanded, nameProperty.stringValue);

        // != null in case of just editting scriptableobject wihtout state machine
        if (statesProperty != null)
        {
            UpDownArrowsGUI();
        }

        // != null in case of just editting scriptableobject wihtout state machine
        if (statesProperty != null && GUILayout.Button("-", GUILayout.Width(removeStateButtonWidth)))
        {
            statesProperty.RemoveFromObjectArray(state);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;

        if (actionsProperty.isExpanded)
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
        if (statesProperty != null)
        {
            string oldStateName = state.name;
            state.name = EditorGUILayout.TextField("Name", state.name);

            if (oldStateName != state.name)
            {
                /*
                // Check if exists in assets
                string[] allStates = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/Scripts/ScriptableObjects/Core/States" });
                if (Array.Exists(allStates, s => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(s)) == state.name))
                {
                    state.name = oldStateName;
                    EditorUtility.DisplayDialog("Duplicated name Error", "Name already exists in '/Core/States/...', reverted to previous name. Please enter a new name", "ok");
                }*/
                // Check if exists in states list
                int occurences = 0;
                for (int i = 0; i < triggersProperty.arraySize; i++)
                {
                    string currentStateName = statesProperty.GetArrayElementAtIndex(i).objectReferenceValue.name;
                    if (state.name == currentStateName)
                    {
                        occurences++;
                        if (occurences == 2)
                        {
                            state.name = oldStateName;
                            EditorUtility.DisplayDialog("Duplicated name Error", "Name already exists in states list, reverted to previous name. Please enter a new name", "ok");
                            break;
                        }
                    }
                }
            }
        }

        // -- Actions GUI -------------------------------------------------

        EditorUtils.DrawHorizontalLine(Color.gray, 2, 10, 10);

        UpdateActionNamesArray();

        EditorGUILayout.LabelField("On Enter Actions:");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        Color defaultColor = GUI.color;
        GUI.color = Color.gray;
        for (int i = 0; i < subEditors1[0].Length; i++)
        {
            subEditors1[0][i].OnInspectorGUI();
        }
        GUI.color = defaultColor;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Actions:");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        defaultColor = GUI.color;
        GUI.color = Color.gray;
        for (int i = 0; i < subEditors1[1].Length; i++)
        {
            subEditors1[1][i].OnInspectorGUI();
        }
        GUI.color = defaultColor;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("On Exit Actions:");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        defaultColor = GUI.color;
        GUI.color = Color.gray;
        for (int i = 0; i < subEditors1[2].Length; i++)
        {
            subEditors1[2][i].OnInspectorGUI();
        }
        GUI.color = defaultColor;
        EditorGUILayout.EndVertical();

        if (state.actions.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        defaultColor = GUI.color;
        GUI.color = Color.grey; // Change background dragging box color

        Rect fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(dropAreaHeight + EditorGUIUtility.standardVerticalSpacing));

        Rect leftAreaRect = fullWidthRect;
        leftAreaRect.y += EditorGUIUtility.standardVerticalSpacing * 0.5f;
        leftAreaRect.width *= 0.5f;
        leftAreaRect.width -= controlSpacing * 0.5f;
        leftAreaRect.height = dropAreaHeight;

        Rect rightAreaRect = leftAreaRect;
        rightAreaRect.x += rightAreaRect.width + controlSpacing;

        ActionsTypeSelectionGUI(leftAreaRect);
        ActionsDragAndDropAreaGUI(rightAreaRect);
        GUI.color = defaultColor; // Restore GUI default color

        ActionsDraggingAndDropping(rightAreaRect, this);

        // -------------------------------------------------------------------
        // -- Triggers GUI ---------------------------------------------------

        EditorGUILayout.Space();

        EditorUtils.DrawHorizontalLine(Color.gray, 2, 10, 10);

        EditorGUILayout.LabelField("Triggers:");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        defaultColor = GUI.color;
        GUI.color = Color.gray;
        // Display already created triggers
        for (int i = 0; i < subEditors2.Length; i++)
        {
            subEditors2[i].OnInspectorGUI();
            EditorGUILayout.Space();
        }
        GUI.color = defaultColor;
        EditorGUILayout.EndVertical();

        // Update triggers to display
        UpdateTriggerNamesArray();

        // EditorUtils.DrawHorizontalLine(Color.gray, 2, 10, 10);

        EditorGUILayout.BeginHorizontal();

        // Add new triggers buttons 
        defaultColor = GUI.color;
        GUI.color = Color.grey; // Change background dragging box color
        TriggersNewButtonGUI();

        fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(dropAreaHeight + EditorGUIUtility.standardVerticalSpacing));

        leftAreaRect = fullWidthRect;
        leftAreaRect.y += EditorGUIUtility.standardVerticalSpacing * 0.5f;
        leftAreaRect.width *= 0.5f;
        leftAreaRect.width -= controlSpacing * 0.5f;
        leftAreaRect.height = dropAreaHeight;

        rightAreaRect = leftAreaRect;
        rightAreaRect.x += rightAreaRect.width + controlSpacing;

        TriggersTypeSelectionGUI(leftAreaRect);
        TriggersDragAndDropAreaGUI(rightAreaRect);
        TriggersDraggingAndDropping(rightAreaRect, this);

        GUI.color = defaultColor; // Restore GUI default color

        EditorGUILayout.EndHorizontal();

        // -------------------------------------------------------------------
    }

    public void UpDownArrowsGUI()
    {
        int index = statesProperty.GetIndexFromObjectArray(state);

        if (index == 0)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↑", GUILayout.Width(removeStateButtonWidth)))
        {
            StateEditor stateEditor1 = stateEditors[index];
            stateEditors[index] = stateEditors[index - 1];
            stateEditors[index - 1] = stateEditor1;

            statesProperty.SwapInObjectArray(index, index - 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }

        if (index == (statesProperty.arraySize - 1))
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("↓", GUILayout.Width(removeStateButtonWidth)))
        {
            StateEditor stateEditor1 = stateEditors[index];
            stateEditors[index] = stateEditors[index + 1];
            stateEditors[index + 1] = stateEditor1;

            statesProperty.SwapInObjectArray(index, index + 1);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }
    }

    private void ActionsTypeSelectionGUI(Rect containingRect)
    {
        Rect topHalf = containingRect;
        topHalf.height *= 0.5f;

        Rect bottomHalfLeft = topHalf;
        bottomHalfLeft.y += bottomHalfLeft.height;
        bottomHalfLeft.width *= 0.5f;

        Rect bottomHalfRight = bottomHalfLeft;
        bottomHalfRight.x += bottomHalfRight.width;

        bottomHalfLeft.width *= 0.95f;

        selectedActionIndex = EditorGUI.Popup(topHalf, selectedActionIndex, actionTypeNames);

        selectedActionSectionIndex = EditorGUI.Popup(bottomHalfLeft, selectedActionSectionIndex, actionSectionsNames);

        if (GUI.Button(bottomHalfRight, "Add Selected Action"))
        {
            Type actionType = actionTypes[selectedActionIndex];
            Action newAction = ActionEditor.CreateAction(actionType);

            // Only if editting scriptable object without state machine
            if (statesProperty == null)
            {
                AssetDatabase.AddObjectToAsset(newAction, state);
                AssetDatabase.SaveAssets();
                //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/States/" + state.name + ".asset");
            }

            if(selectedActionSectionIndex == 0) // on Enter
            {
                onEnterProperty.AddToObjectArray(newAction);
            }
            else if (selectedActionSectionIndex == 1) // actions
            {
                actionsProperty.AddToObjectArray(newAction);
            }
            else if (selectedActionSectionIndex == 2) // on Exit
            {
                onExitProperty.AddToObjectArray(newAction);
            }
        }
    }

    private static void ActionsDraggingAndDropping(Rect dropArea, StateEditor editor)
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

                    Type actionType = script.GetClass();

                    Action newAction = ActionEditor.CreateAction(actionType);

                    // Only if editting scriptable object without state machine
                    if (editor.statesProperty == null)
                    {
                        AssetDatabase.AddObjectToAsset(newAction, editor.state);
                        AssetDatabase.SaveAssets();
                        //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/States/" + editor.state.name + ".asset");
                    }

                    if (editor.selectedActionSectionIndex == 0) // on Enter
                    {
                        editor.onEnterProperty.AddToObjectArray(newAction);
                    }
                    else if (editor.selectedActionSectionIndex == 1) // actions
                    {
                        editor.actionsProperty.AddToObjectArray(newAction);
                    }
                    else if (editor.selectedActionSectionIndex == 2) // on Exit
                    {
                        editor.onExitProperty.AddToObjectArray(newAction);
                    }
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

            if (!scriptType.IsSubclassOf(typeof(Action)))
                return false;

            if (scriptType.IsAbstract)
                return false;
        }

        return true;
    }

    private static void ActionsDragAndDropAreaGUI(Rect containingRect)
    {
        GUIStyle centredStyle = GUI.skin.box;
        centredStyle.alignment = TextAnchor.MiddleCenter;
        centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

        GUI.Box(containingRect, "Drop new Actions here", centredStyle);
    }

    private void UpdateActionNamesArray()
    {
        Type actionType = typeof(Action);

        Type[] allTypes = actionType.Assembly.GetTypes();

        List<Type> actionSubTypeList = new List<Type>();

        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].IsSubclassOf(actionType) && !allTypes[i].IsAbstract)
            {
                actionSubTypeList.Add(allTypes[i]);
            }
        }

        actionTypes = actionSubTypeList.ToArray();

        List<string> actionTypeNameList = new List<string>();

        for (int i = 0; i < actionTypes.Length; i++)
        {
            actionTypeNameList.Add(actionTypes[i].Name);
        }

        actionTypeNames = actionTypeNameList.ToArray();
    }

    private void TriggersNewButtonGUI()
    {
        if (GUILayout.Button("New Trigger", GUILayout.Width(100), GUILayout.Height(dropAreaHeight + EditorGUIUtility.standardVerticalSpacing)))
        {
            Trigger newTrigger = new Trigger();
            newTrigger.name = "CustomTrigger";

            int instanceNumber = 1;
            bool ok = false;
            while (!ok)
            {
                ok = true;
                foreach (Trigger trigger in state.triggers)
                {
                    if (trigger.name == newTrigger.name)
                    {
                        ok = false;
                        newTrigger.name = "CustomTrigger" + instanceNumber++;
                        break;
                    }
                }
            }

            // Only if editting scriptable object without state machine
            if (statesProperty == null)
            {
                AssetDatabase.AddObjectToAsset(newTrigger, state);
                //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/States/" + state.name + ".asset");
                AssetDatabase.SaveAssets();
            }

            triggersProperty.AddToObjectArray(newTrigger);
        }
    }

    private void TriggersTypeSelectionGUI(Rect containingRect)
    {
        Rect topHalf = containingRect;
        topHalf.height *= 0.5f;

        Rect bottomHalf = topHalf;
        bottomHalf.y += bottomHalf.height;

        if (availableTriggerTypeNames.Length == 0)
        {
            GUI.enabled = false;
        }

        availableTriggersIndex = EditorGUI.Popup(topHalf, availableTriggersIndex, availableTriggerTypeNames);

        if (GUI.Button(bottomHalf, "Add Selected Trigger"))
        {
            Trigger newTriggerTemplate = AssetDatabase.LoadAssetAtPath<Trigger>(InteractionPaths.TRIGGERS_PATH + "/" + availableTriggerTypeNames[availableTriggersIndex] + ".asset");
            newTriggerTemplate.name = availableTriggerTypeNames[availableTriggersIndex];

            if (newTriggerTemplate == null)
            {
                throw new UnityException(availableTriggerTypeNames[availableTriggersIndex] + " could not be instantiated");
            }

            Trigger newTrigger = newTriggerTemplate.Clone();

            // Only if editting scriptable object without state machine
            if (statesProperty == null)
            {
                AssetDatabase.AddObjectToAsset(newTrigger, state);
                AssetDatabase.SaveAssets();
                //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/States/" + state.name + ".asset");
            }

            triggersProperty.AddToObjectArray(newTrigger);
        }
        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }
    }

    private static void TriggersDragAndDropAreaGUI(Rect containingRect)
    {
        GUIStyle centredStyle = GUI.skin.box;
        centredStyle.alignment = TextAnchor.MiddleCenter;
        centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

        GUI.Box(containingRect, "Drop new Triggers here", centredStyle);
    }

    private static void TriggersDraggingAndDropping(Rect dropArea, StateEditor editor)
    {
        Event currentEvent = Event.current;

        if (!dropArea.Contains(currentEvent.mousePosition))
        {
            return;
        }

        switch (currentEvent.type)
        {
            case EventType.DragUpdated:

                DragAndDrop.visualMode = TriggerIsDragValid(ref editor.availableTriggerTypeNames) ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                currentEvent.Use();

                break;
            case EventType.DragPerform:

                DragAndDrop.AcceptDrag();

                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    Trigger newTriggerTemplate = DragAndDrop.objectReferences[i] as Trigger;
                    if (newTriggerTemplate == null)
                    {
                        throw new UnityException(DragAndDrop.objectReferences[i].name + " could not be instantiated");
                    }

                    Trigger newTrigger = newTriggerTemplate.Clone();

                    // Only if editting scriptable object without state machine
                    if (editor.statesProperty == null)
                    {
                        AssetDatabase.AddObjectToAsset(newTrigger, editor.state);
                        AssetDatabase.SaveAssets();
                        //AssetDatabase.ImportAsset("Assets/Scripts/ScriptableObjects/Core/States/" + editor.state.name + ".asset");
                    }

                    editor.triggersProperty.AddToObjectArray(newTrigger);
                }

                currentEvent.Use();

                break;
        }
    }

    private static bool TriggerIsDragValid(ref string[] validStates)
    {
        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
        {
            if (DragAndDrop.objectReferences[i].GetType() != typeof(Trigger))
                return false;

            // if it doesn't exists in available states list, do not accept it
            if (Array.Exists(validStates, (x => (x == DragAndDrop.objectReferences[i].name))) == false)
            {
                return false;
            }
        }

        return true;
    }

    // Maybe this method could be optimized but it is enough by now since it is only executed in editor
    private void UpdateTriggerNamesArray()
    {
        // Cache 'already in use' striggers
        string[] triggersInUse = new string[state.triggers.Length];
        for (int i = 0; i < state.triggers.Length; i++)
        {
            triggersInUse[i] = state.triggers[i].name;
        }

        // Get all trigger names without path
        string[] allTriggers = AssetDatabase.FindAssets("t:ScriptableObject", new[] { InteractionPaths.TRIGGERS_PATH });
        for (int i = 0; i < allTriggers.Length; i++)
        {
            allTriggers[i] = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(allTriggers[i]));
        }

        // Find all that don't exist in 'triggersInUse'
        availableTriggerTypeNames = Array.FindAll(allTriggers, (x => (Array.Exists(triggersInUse, element => element == x) == false)));
    }
}