using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class DisplayTipAction : Action
{
    public LocalizedString localizedTip;
    public float timeSeconds = .0f;
    [Tooltip("Ignores 'timeSeconds'.")]
    public bool forceInteraction = false;
    [Tooltip("Use ignore if no interaction required.")]
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

        script_UIController.ClearUI();

        localizedTip.RegisterChangeHandler(DisplayTip);
        localizedTip.GetLocalizedString();

        currentTimeSeconds = .0f;

        if(pauseGame)
        {
            script_GameController.PauseGame(false);
            script_GameController.AllowPauseGame(false);
        }

        return false;
    }

    private void DisplayTip(string s)
    {
        script_UIController.SetTextMessage(s, true);
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + (pauseGame ? Time.unscaledDeltaTime : Time.deltaTime), timeSeconds);

        // If timer is ready and we don't force to wait until interaction happens OR
        // We received the input we expected and minimum time is reached
        if( (currentTimeSeconds == timeSeconds && !forceInteraction) || (InputReceived() && currentTimeSeconds >= minTimeSeconds) )
        {
            if (pauseGame)
            {
                script_GameController.ResumeGame();
                script_GameController.AllowPauseGame(true);
            }

            script_UIController.EraseTextMessage(0);
            localizedTip.ClearChangeHandler();
            return true;
        }

        return false;
    }

    private bool InputReceived()
    {
        // early out
        if(inputEvent == EventType.Ignore)
        {
            return false;
        }

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

    public override void forceFinish()
    {
        localizedTip.ClearChangeHandler();
        script_UIController.EraseTextMessage(0);
        if (pauseGame)
        {
            script_GameController.ResumeGame();
            script_GameController.AllowPauseGame(true);
        }
    }

    protected override Action CloneDerived()
    {
        DisplayTipAction clone = ScriptableObject.CreateInstance<DisplayTipAction>();

        clone.localizedTip = this.localizedTip;
        clone.timeSeconds = this.timeSeconds;
        clone.forceInteraction = this.forceInteraction;
        clone.inputEvent = this.inputEvent;
        clone.key = this.key;
        clone.minTimeSeconds = this.minTimeSeconds;
        clone.pauseGame = this.pauseGame;

        return clone;
    }
}
