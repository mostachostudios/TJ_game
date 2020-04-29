using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : EditorWithSubEditors<StateEditor, State>
{
    private StateMachine stateMachine;
    private SerializedProperty initialStateProperty;
    private SerializedProperty statesProperty;

    private const string stateMachinePropCurrentStateName = "initialState";
    private const string stateMachinePropStatesName = "states";
    
    private string[] availableStateTypeNames = new string[0];
    private int availableStatesIndex;
    private int initialStateIndex;

    private const float dropAreaHeight = 50f;
    private const float controlSpacing = 5f;

    private void OnEnable()
    {
        stateMachine = (StateMachine)target;

        if (target == null)
        {
            DestroyImmediate(this);
            return;
        }

        initialStateProperty = serializedObject.FindProperty(stateMachinePropCurrentStateName);
        statesProperty = serializedObject.FindProperty(stateMachinePropStatesName);

        SetInitialStateIndex();
    }

    private void OnDisable()
    {
        CleanupEditors();
    }

    public override void SubEditorSetup(StateEditor stateEditor)
    {
        stateEditor.stateEditors = subEditors;
        stateEditor.statesProperty = statesProperty;
    }

    public override void OnInspectorGUI()
    {
        CheckAndCreateSubEditors(stateMachine.states);

        UpdateStateNamesArray();

        serializedObject.Update();

        // Initial state GUI --------------------------------------------
        if (stateMachine.states.Length > 0)
        {
            InitialStateGUI();
            EditorUtils.DrawHorizontalLine(Color.gray);
        }
        // End of Initial state GUI -------------------------------------

        // States GUI ---------------------------------------------------

        EditorGUILayout.LabelField("States:");

        // Already created states
        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI();
            EditorGUILayout.Space();
        }

        // Add new states
        if (stateMachine.states.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        EditorUtils.DrawHorizontalLine(Color.gray, 2, 10, 10);

        EditorGUILayout.BeginHorizontal();

        // Add new triggers buttons 
        StatesNewButtonGUI();

        Rect fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(dropAreaHeight + EditorGUIUtility.standardVerticalSpacing));

        Rect leftAreaRect = fullWidthRect;
        leftAreaRect.y += EditorGUIUtility.standardVerticalSpacing * 0.5f;
        leftAreaRect.width *= 0.5f;
        leftAreaRect.width -= controlSpacing * 0.5f;
        leftAreaRect.height = dropAreaHeight;

        Rect rightAreaRect = leftAreaRect;
        rightAreaRect.x += rightAreaRect.width + controlSpacing;

        StateTypeSelectionGUI(leftAreaRect);
        StateDragAndDropAreaGUI(rightAreaRect);

        StateDraggingAndDropping(rightAreaRect, this);

        EditorGUILayout.EndHorizontal();

        // End of States GUI -----------------------------------------------

        serializedObject.ApplyModifiedProperties();
    }

    private void InitialStateGUI()
    {
        string[] statesInUse = new string[stateMachine.states.Length];
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            statesInUse[i] = stateMachine.states[i].name;
        }

        // Don't let the index go out of bounds
        if (initialStateIndex >= stateMachine.states.Length)
        {
            initialStateIndex = 0;
        }

        initialStateIndex = EditorGUILayout.Popup("Initial state", initialStateIndex, statesInUse);

        initialStateProperty.SetValue(stateMachine.states[initialStateIndex]);
    }

    private void StatesNewButtonGUI()
    {
        if (GUILayout.Button("New State", GUILayout.Width(100), GUILayout.Height(dropAreaHeight + EditorGUIUtility.standardVerticalSpacing)))
        {
            State newState = new State();
            newState.name = "CustomState";
            newState.parentStateMachine = stateMachine;

            int instanceNumber = 1;
            bool ok = false;
            while (!ok)
            {
                ok = true;
                foreach (State state in stateMachine.states)
                {
                    if (state.name == newState.name)
                    {
                        ok = false;
                        newState.name = "CustomState" + instanceNumber++;
                        break;
                    }
                }
            }

            statesProperty.AddToObjectArray(newState);
        }
    }

    private void StateTypeSelectionGUI(Rect containingRect)
    {
        Rect topHalf = containingRect;
        topHalf.height *= 0.5f;

        Rect bottomHalf = topHalf;
        bottomHalf.y += bottomHalf.height;

        if (availableStateTypeNames.Length == 0)
        {
            GUI.enabled = false;
        }

        availableStatesIndex = EditorGUI.Popup(topHalf, availableStatesIndex, availableStateTypeNames);

        if (GUI.Button(bottomHalf, "Add Selected State"))
        {
            State newStateTemplate = AssetDatabase.LoadAssetAtPath<State>(InteractionPaths.STATES_PATH + "/" + availableStateTypeNames[availableStatesIndex] + ".asset");
            newStateTemplate.name = availableStateTypeNames[availableStatesIndex];

            if (newStateTemplate == null)
            {
                throw new UnityException(availableStateTypeNames[availableStatesIndex] + " could not be instantiated");
            }

            State newState = newStateTemplate.Clone();
            newState.parentStateMachine = stateMachine;
            statesProperty.AddToObjectArray(newState);
        }

        if (GUI.enabled == false)
        {
            GUI.enabled = true;
        }
    }

    private static void StateDragAndDropAreaGUI(Rect containingRect)
    {
        GUIStyle centredStyle = GUI.skin.box;
        centredStyle.alignment = TextAnchor.MiddleCenter;
        centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

        GUI.Box(containingRect, "Drop new States here", centredStyle);
    }

    private static void StateDraggingAndDropping(Rect dropArea, StateMachineEditor editor)
    {
        Event currentEvent = Event.current;

        if (!dropArea.Contains(currentEvent.mousePosition))
        {
            return;
        }

        switch (currentEvent.type)
        {
            case EventType.DragUpdated:

                DragAndDrop.visualMode = StateIsDragValid(ref editor.availableStateTypeNames) ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                currentEvent.Use();

                break;
            case EventType.DragPerform:

                DragAndDrop.AcceptDrag();

                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    State newStateTemplate = DragAndDrop.objectReferences[i] as State;
                    if (newStateTemplate == null)
                    {
                        throw new UnityException(DragAndDrop.objectReferences[i].name + " could not be instantiated");
                    }

                    State newState = newStateTemplate.Clone();
                    newState.parentStateMachine = editor.stateMachine;

                    editor.statesProperty.AddToObjectArray(newState);
                }

                currentEvent.Use();

                break;
        }
    }

    private static bool StateIsDragValid(ref string[] validStates)
    {
        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
        {
            if (DragAndDrop.objectReferences[i].GetType() != typeof(State))
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
    private void UpdateStateNamesArray()
    {
        // Cache 'already in use' states
        string[] statesInUse = new string[stateMachine.states.Length];
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            statesInUse[i] = stateMachine.states[i].name;
        }

        // Get all state names without path
        string[] allStates = AssetDatabase.FindAssets("t:ScriptableObject", new[] { InteractionPaths.STATES_PATH });
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i] = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(allStates[i]));
        }

        // Find all that don't exist in 'statesInUse'
        availableStateTypeNames = Array.FindAll(allStates, (x => (Array.Exists(statesInUse, element => element == x) == false)));
    }

    private void SetInitialStateIndex()
    {
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            if (stateMachine.initialState == stateMachine.states[i])
            {
                initialStateIndex = i;
                return;
            }
        }
    }
}