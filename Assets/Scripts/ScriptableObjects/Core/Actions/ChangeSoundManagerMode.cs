using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSoundManagerMode : Action
{
    public SoundManager.Mode mode = SoundManager.Mode.GAMEPLAY;
    public float fadeOffSeconds = 1.0f;
    public float volume = 1.0f;
    public bool loop = false;

    private SoundManager soundManager;
    private float currentFadeOffSeconds;


    protected override bool StartDerived()
    {
        soundManager = FindObjectOfType<SoundManager>();
        soundManager.ChangeMode(mode, volume, loop, fadeOffSeconds);
        
        currentFadeOffSeconds = .0f;

        return currentFadeOffSeconds == fadeOffSeconds;
    }

    protected override bool UpdateDerived()
    {
        currentFadeOffSeconds = Mathf.Min(currentFadeOffSeconds + Time.deltaTime, fadeOffSeconds);
        return currentFadeOffSeconds == fadeOffSeconds;
    }

    protected override Action CloneDerived()
    {
        ChangeSoundManagerMode clone = ScriptableObject.CreateInstance<ChangeSoundManagerMode>();
        clone.mode = this.mode;
        clone.fadeOffSeconds = this.fadeOffSeconds;
        clone.loop = this.loop;
        clone.volume = this.volume;
        return clone;
    }
}
