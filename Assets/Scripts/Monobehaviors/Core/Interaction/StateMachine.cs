using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] // To enable being alert about state machine duplications in unity editor mode
public class StateMachine : MonoBehaviour
{
    public State initialState;

    public State[] states = new State[0];

    private State currentState;
    private State nextState = null;

    private Queue<Action> toDoList = new Queue<Action>();


    void Awake()
    {
        // HACK!! InstanceId + Complex method to deal with in-editor duplications in order to keep instance ownership consistency
#if UNITY_EDITOR
        RegisterInstance();
#endif

        if (Application.isPlaying == false)
            return;

        if (initialState == null)
        {
            throw new UnityException("State Machine must have an initial state");
        }

        Restart();
    }

    void Update()
    {
        // Avoid to update state machine in edit mode
#if UNITY_EDITOR
        if (Application.isPlaying == false)
            return;
#endif

        // If we are not in transition to another state
        if (nextState == null)
        {
            // Check if any other state should be triggered
            foreach (State state in states)
            {
                if (state.enabled && state != currentState)
                {
                    if (state.CheckTriggers())
                    {
                        SmoothChangeState(state);
                        break;
                    }
                }
            }
        }

        // Make sure we refill ToDo list if it is empty
        if (toDoList.Count == 0)
        {
            // We finish all ToDo tasks, so refill with current state actions
            if (nextState == null)
            {
                if(currentState.loop)
                {
                    FillToDoList(currentState.actions);
                }
                else
                {
                    return; // No loop, do nothing, just keep waiting until some trigger awakes
                }
            }
            // If we have to change state and there isn't any exit action left, fill with next state tasks
            else
            {
                currentState = nextState;
                nextState = null;
                if (currentState.onEnterActions.Length != 0)
                {
                    FillToDoList(currentState.onEnterActions);
                }
                else
                {
                    FillToDoList(currentState.actions);
                }
            }
        }


        // Do action, following FIFO approach
        Action actionToDo = toDoList.Peek();

        // Update while the actions that we just started, are considered finished
        while (true)
        {
            if (actionToDo.HasStarted()) // If it has been started, just update it
            {
                if (actionToDo.Update()) // If it just finished after this update, pass to the next action
                {
                    actionToDo.Reset();
                    toDoList.Dequeue();
                }
                else
                {
                    break;
                }
            }
            else // Start the action
            {
                if (actionToDo.Start()) // If started and finished at the same time, then deque and NEXT!
                {
                    actionToDo.Reset();
                    toDoList.Dequeue();
                }
                else
                {
                    break;
                }
            }

            if(toDoList.Count == 0)
            {
                break;
            }

            actionToDo = toDoList.Peek();
        }
    }

    public void Restart()
    {
        currentState = initialState;

        nextState = null;

        toDoList.Clear();

        // Fill TO-DO actions list with current state's actions
        FillToDoList(currentState.actions);
    }

    public void SmoothChangeState(State state)
    {
        toDoList.Clear();
        FillToDoList(currentState.onExitActions);

        nextState = state;
    }

    public void ForceChangeState(State state)
    {
        toDoList.Clear();

        nextState = state;
    }

    private void FillToDoList(Action[] actions)
    {
        foreach (Action action in actions)
        {
            if (action.enabled)
            {
                toDoList.Enqueue(action);
            }
        }
    }

#if UNITY_EDITOR
    //catch duplication of this GameObject
    [SerializeField]
    int instanceID = 0;

    private void RegisterInstance()
    {
        if (Application.isPlaying)
            return;

        // Object created, not duplicated
        if (instanceID == 0)
        {
            instanceID = GetInstanceID();
            return;
        }

        // Object duplicated
        if (instanceID != GetInstanceID() && GetInstanceID() < 0)
        {
            int parentInstanceId = instanceID;

            instanceID = GetInstanceID();

            UnityEngine.Object objectToFind = EditorUtility.InstanceIDToObject(parentInstanceId);

            if (objectToFind == null)
            {
                throw new UnityException("Error, instance id with id " + parentInstanceId + ", doens't exist");
            }

            StateMachine parentStateMachine = objectToFind as StateMachine;

            if (parentStateMachine == null)
            {
                throw new UnityException(parentStateMachine + "is not an State Machine");
            }

            // Actual instancing
            CopyTo(this, parentStateMachine);
        }
    }

#endif

    static void CopyTo(StateMachine sourceStateMachine, StateMachine targetStateMachine)
    {
        if (sourceStateMachine == targetStateMachine)
        {
            throw new UnityException("Source and target state machines are the same");
        }

        targetStateMachine.states = new State[sourceStateMachine.states.Length];

        for (int i = 0; i < sourceStateMachine.states.Length; i++)
        {
            targetStateMachine.states[i] = new State();
            targetStateMachine.states[i] = sourceStateMachine.states[i].Clone();
            targetStateMachine.states[i].parentStateMachine = targetStateMachine;
        }

        for (int i = 0; i < sourceStateMachine.states.Length; i++)
        {
            if (sourceStateMachine.currentState == sourceStateMachine.states[i])
            {
                targetStateMachine.currentState = targetStateMachine.states[i];
                break;
            }
        }
    }
}
