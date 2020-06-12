using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAction : Action
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool loop = false;
    [Range(0f,1f)] public float volume = 1f;

    public bool onSoundManager = true;

    private SoundManager soundManager;


    protected override bool StartDerived()
    {
        if (onSoundManager)
        {
            soundManager = FindObjectOfType<SoundManager>();
            soundManager.PlayClip(audioClip, volume, 1.0f, loop);
        }
        else
        {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.time = 0;
            audioSource.Play();
        }

        return false;
    }

    protected override bool UpdateDerived()
    {
        if (onSoundManager)
        {
            return soundManager.IsPlaying(audioClip);
        }
        else
        {
            return audioSource.isPlaying == false;
        }
    }

    protected override Action CloneDerived()
    {
        PlayAudioAction clone = ScriptableObject.CreateInstance<PlayAudioAction>();
        clone.audioSource = this.audioSource;
        clone.audioClip = this.audioClip;
        clone.loop = this.loop;
        clone.volume = this.volume;
        return clone;
    }
}
