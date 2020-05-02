using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAction : Action
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool loop = false;
    [Range(0f,1f)] public float volume = 1f;


    protected override bool StartDerived()
    {
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.time = 0;
        audioSource.Play();

        return false;
    }

    protected override bool UpdateDerived()
    {
        return !audioSource.isPlaying;
    }

    protected override Action CloneDerived()
    {
        PlayAudioAction clone = new PlayAudioAction();
        clone.audioSource = this.audioSource;
        clone.audioClip = this.audioClip;
        clone.loop = this.loop;
        clone.volume = this.volume;
        return clone;
    }
}
