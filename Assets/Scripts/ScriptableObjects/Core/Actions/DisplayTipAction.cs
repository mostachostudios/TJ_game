using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTipAction : Action
{
    public LocalizationTableKey tableKey;
    public LocalizationAssetKey tip;
    public float timeSeconds = .0f;
    public EventType inputEvent = EventType.KeyDown;
    public KeyCode key = KeyCode.None;
    public float minTimeSeconds = .0f;
    public bool pauseGame = true;

    private float currentTimeSeconds;
    private Script_UIController script_UIController;
    private Script_GameController script_GameController;

    protected override bool StartDerived()
    {
        script_UIController = FindObjectOfType<Script_UIController>();
        script_GameController = FindObjectOfType<Script_GameController>();

        if(script_UIController == null)
        {
            throw new UnityException("There isn't a Script_UIController instance on the scene");
        }
        if (script_GameController == null)
        {
            throw new UnityException("There isn't a Script_GameController instance on the scene");
        }

        string message = FindObjectOfType<LocalizationManager>().GetStringAsset(tableKey, tip);
        script_UIController.SetTextMessage(message, true);

        currentTimeSeconds = .0f;

        if(pauseGame)
        {
            //TODO(adrian)
            //script_GameController.PauseGame();
        }

        return false;
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        if( (currentTimeSeconds == timeSeconds && ExpectsInput() == false) || (InputReceived() && currentTimeSeconds >= minTimeSeconds) )
        {
            if (pauseGame)
            {
                //TODO(adrian)
                //script_GameController.ResumeGame();
            }

            script_UIController.EraseTextMessage(0);
            return true;
        }

        return false;
    }

    private bool ExpectsInput()
    {
        // key
        if (inputEvent == EventType.KeyDown && key != KeyCode.None)
        {
            return true;
        }
        // mouse wheel
        else if (inputEvent == EventType.ScrollWheel)
        {
            return true;
        }
        // TODO: add more here if required

        return false;
    }

    private bool InputReceived()
    {
        // key
        if(inputEvent == EventType.KeyDown && (key == KeyCode.None || Input.GetKeyDown(key)))
        {
            return true;
        }
        // mouse wheel
        else if(inputEvent == EventType.ScrollWheel && (Input.mouseScrollDelta.x != 0 || Input.mouseScrollDelta.y != 0))
        {
            return true;
        }
        // TODO: add more here if required

        return false;
    }

    protected override Action CloneDerived()
    {
        DisplayTipAction clone = ScriptableObject.CreateInstance<DisplayTipAction>();

        clone.tableKey = this.tableKey;
        clone.tip = this.tip;
        clone.timeSeconds = this.timeSeconds;
        clone.inputEvent = this.inputEvent;
        clone.key = this.key;
        clone.minTimeSeconds = this.minTimeSeconds;
        clone.pauseGame = this.pauseGame;

        return clone;
    }
}
