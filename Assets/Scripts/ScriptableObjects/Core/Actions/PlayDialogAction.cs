using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class PlayDialogAction : Action
{
    public Sprite characterAvatar;
    public Dialog.DialogPosition dialogPosition;
    public float timeBetweenMessage = .0f;
    public List<string> messages;
    public List<LocalizedString> localizedMessages;

    private Dialog dialog;
    private float currentTimeSeconds = .0f;
    private int currentMessage = 0;
    private Script_UIController script_UIController;

    private float epsilon = 0.05f;

    protected override bool StartDerived()
    {
        if(localizedMessages.Count == 0) // No messages
        {
            return true;
        }

        script_UIController = FindObjectOfType<Script_UIController>();
        currentMessage = 0; 
        currentTimeSeconds = .0f;

        dialog = ScriptableObject.CreateInstance<Dialog>();
        dialog.m_Avatar = characterAvatar;
        dialog.m_DialogPosition = dialogPosition;
        //dialog.m_Text = messages[currentMessage];

        var localizedString = localizedMessages[currentMessage].GetLocalizedString();

        if (localizedString.IsDone)
        {
            DisplayDialog(localizedString.Result);
        }
        else
        {
            localizedMessages[currentMessage].RegisterChangeHandler(DisplayDialog);
        }


        // Write Dialog. Needs to set time here in case Action gets interrupted. Besides, a small epsilon value has been substracted from time
        // in order to prevent overlapping with the next dialogue, (Text might be deleted just after it was written, depending on execution order of coroutines in SetDialog) 
        script_UIController.SetDialog(dialog, timeBetweenMessage - epsilon); 

        currentMessage++;

        return false;
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeBetweenMessage);

        if (currentTimeSeconds == timeBetweenMessage)
        {
            if (currentMessage == localizedMessages.Count) // No remaining messages
            {
                localizedMessages[currentMessage - 1].ClearChangeHandler();
                return true;
            }
            else
            {
                currentTimeSeconds = .0f;

                var localizedString = localizedMessages[currentMessage].GetLocalizedString();

                if (localizedString.IsDone)
                {
                    DisplayDialog(localizedString.Result);
                }
                else
                {
                    localizedMessages[currentMessage].RegisterChangeHandler(DisplayDialog);
                }

                script_UIController.SetDialog(dialog, timeBetweenMessage - epsilon);
                localizedMessages[currentMessage].ClearChangeHandler();
                currentMessage++;
            }
        }

        return false;
    }

    private void DisplayDialog(string s)
    {
        dialog.m_Text = s;
    }

    public override void forceFinish()
    {
        localizedMessages[currentMessage -1].ClearChangeHandler();
    }

    protected override Action CloneDerived()
    {
        PlayDialogAction clone = ScriptableObject.CreateInstance<PlayDialogAction>();

        clone.characterAvatar = this.characterAvatar;
        clone.dialogPosition = this.dialogPosition;
        clone.timeBetweenMessage = this.timeBetweenMessage;
        clone.localizedMessages = new List<LocalizedString>(this.localizedMessages);

        return clone;
    }
}
