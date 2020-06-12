using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public enum Mode { MENU, GAMEPLAY, GAMEOVER, MANUAL };

    public AudioSource m_musicAudioSource;
    public AudioClip[] m_menuClips;
    public AudioClip[] m_gameplayClips;
    public AudioClip[] m_gameOverClips;

    private Mode m_currentMode = Mode.MANUAL;
    private Mode m_nextMode = Mode.MANUAL;

    private int[] m_gameplayClipsIndexOrder;
    private int[] m_menuClipsIndexOrder;
    private int[] m_gameOverClipsIndexOrder;

    private int m_menuClipsCurrentIndex = 0;
    private int m_gameplayClipsCurrentIndex = 0;
    private int m_gameoverClipsCurrentIndex = 0;

    private bool m_fading = false;
    private float m_startFadeVolume;
    private float m_targetFadeVolume;
    private float m_fadeSeconds;
    private float m_currentFadeSeconds;

    private AudioClip m_manualAudioClip;

    private bool m_paused = false;
    private bool m_stopped = false;
    private bool m_random = true;
    private float m_volume = 1.0f;
    private bool m_loop = true;


    private void Awake()
    {
        // Singleton pattern
        if (FindObjectsOfType<SoundManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        if (!Empty())
        {
            RebuildQueue();
        }
    }

    private void OnDisable()
    {
        if (m_musicAudioSource != null)
        {
            m_musicAudioSource.Stop();
        }
    }

    public void Pause()
    {
        m_paused = true;
        m_musicAudioSource.Pause();
    }

    public void PlayClip(AudioClip audioClip, float volume, float fadeSeconds, bool loop)
    {
        m_nextMode = Mode.MANUAL;
        m_manualAudioClip = audioClip;
        m_loop = loop;
        m_volume = volume;

        if (IsPlaying() && fadeSeconds != .0f)
        {
            m_fading = true;
            m_startFadeVolume = m_musicAudioSource.volume;
            m_targetFadeVolume = .0f;
            m_fadeSeconds = fadeSeconds;
            m_currentFadeSeconds = .0f;
        }
        else
        {
            m_currentMode = Mode.MANUAL;
            SetClipMode(Mode.MANUAL);
        }
    }

    public void Play()
    {
        m_paused = false;
        m_stopped = false;
        m_musicAudioSource.volume = m_volume;
        m_musicAudioSource.UnPause();
        m_musicAudioSource.Play();
    }

    public bool IsPlaying()
    {
        return m_fading || (!m_paused && !m_stopped);
    }

    public bool IsPlaying(AudioClip audioClip)
    {
        return m_musicAudioSource.clip == audioClip && m_musicAudioSource.isPlaying;
    }

    public void Stop(float fadeSeconds = .0f)
    {
        if (fadeSeconds != .0f)
        {
            m_fading = true;
            m_startFadeVolume = m_musicAudioSource.volume;
            m_targetFadeVolume = .0f;
            m_fadeSeconds = fadeSeconds;
            m_stopped = true;
            m_currentFadeSeconds = .0f;
        }
        else
        {
            m_musicAudioSource.Stop();
            m_stopped = true;
        }
    }

    public void SetVolume(float volume)
    {
        m_volume = volume;
    }

    public void ChangeMode(Mode mode, bool loop, float fadeSeconds = 1.0f)
    {
        m_loop = loop;

        if (mode == m_currentMode)
        {
            return;
        }

        m_nextMode = mode;

        if (IsPlaying())
        {
            if (fadeSeconds > .0f)
            {
                m_fading = true;
                m_startFadeVolume = m_musicAudioSource.volume;
                m_targetFadeVolume = .0f;
                m_volume = m_musicAudioSource.volume;
                m_fadeSeconds = fadeSeconds;
                m_currentFadeSeconds = .0f;
            }
        }
        else
        {
            m_currentMode = m_nextMode;
        }
    }

    public void ChangeMode(Mode mode, float volume, bool loop, float fadeSeconds = 1.0f)
    {
        m_volume = volume;
        ChangeMode(mode, loop, fadeSeconds);
    }

    public void RebuildQueue()
    {
        if(Empty())
        {
            return;
        }

        if (m_random)
        {
            m_gameplayClipsIndexOrder = GenerateRandomSequence(m_gameplayClips.Length);
            m_menuClipsIndexOrder = GenerateRandomSequence(m_menuClips.Length);
            m_gameOverClipsIndexOrder = GenerateRandomSequence(m_gameOverClips.Length);
        }
        else
        {
            m_gameplayClipsIndexOrder = Enumerable.Range(0, m_gameplayClips.Length - 1).ToArray();
            m_menuClipsIndexOrder = Enumerable.Range(0, m_menuClips.Length - 1).ToArray();
            m_gameOverClipsIndexOrder = Enumerable.Range(0, m_gameOverClips.Length - 1).ToArray();
        }

        m_gameplayClipsCurrentIndex = 0;
        m_menuClipsCurrentIndex = 0;
        m_gameoverClipsCurrentIndex = 0;

        SetClip(0);
    }

    void Update()
    {
        // Nothing to do
        if (m_musicAudioSource == null)
        {
            return;
        }

        // Nothing to do and no plans to do anything in the future
        if(Empty() && m_currentMode == m_nextMode)
        {
            return;
        }

        // Doing nothing and no plans to do anything in the future
        if (IsPlaying() == false && m_currentMode == m_nextMode)
        {
            return;
        }

        // We have to change mode
        if (m_currentMode != m_nextMode)
        {
            // No transition? Go to new mode
            if (m_fading == false)
            {
                m_currentMode = m_nextMode;
                SetClipMode(m_currentMode);
            }
        }

        // Are we fading?
        if(m_fading)
        {
            m_currentFadeSeconds = Math.Min(m_currentFadeSeconds + Time.unscaledDeltaTime, m_fadeSeconds);
            float volumeDiff = m_targetFadeVolume - m_startFadeVolume;
            float ratio = m_currentFadeSeconds / m_fadeSeconds;
            m_musicAudioSource.volume = m_startFadeVolume + ratio * volumeDiff;

            // Finished fading?
            if (m_currentFadeSeconds == m_fadeSeconds)
            {
                m_fading = false;

                // Finished transitioning? Apply new mode
                if (m_currentMode != m_nextMode)
                {
                    m_currentMode = m_nextMode;
                    SetClipMode(m_currentMode);
                }
            }
        }
        else if(m_currentMode != Mode.MANUAL) // We should be playing something
        {
            // Clip finished, DJ please, put me a good one! :P
            if (m_musicAudioSource.isPlaying == false)
            {
                NextClip();
            }
        }
    }

    private bool Empty()
    {
        if (m_currentMode == Mode.GAMEPLAY)
        {
            return m_gameplayClips == null || m_gameplayClips.Length == 0;
        }
        else if (m_currentMode == Mode.MENU)
        {
            return m_menuClips == null || m_menuClips.Length == 0;
        }
        else if (m_currentMode == Mode.GAMEOVER)
        {
            return m_gameOverClips == null || m_gameOverClips.Length == 0;
        }
        /*
        else if (m_currentMode == Mode.MANUAL)
        {
            return m_manualAudioClip == null;
        }
        */

        return false;
    }

    private void SetClip(int index)
    {
        if (m_currentMode == Mode.GAMEPLAY)
        {
            m_gameplayClipsCurrentIndex = index;
            m_musicAudioSource.time = 0;
            m_musicAudioSource.loop = false;
            m_musicAudioSource.clip = m_gameplayClips[m_gameplayClipsIndexOrder[m_gameplayClipsCurrentIndex]];
        }
        else if (m_currentMode == Mode.MENU)
        {
            m_menuClipsCurrentIndex = index;
            m_musicAudioSource.time = 0;
            m_musicAudioSource.loop = false;
            m_musicAudioSource.clip = m_menuClips[m_menuClipsIndexOrder[m_menuClipsCurrentIndex]];
        }
        else if (m_currentMode == Mode.GAMEOVER)
        {
            m_gameoverClipsCurrentIndex = index;
            m_musicAudioSource.time = 0;
            m_musicAudioSource.loop = false;
            m_musicAudioSource.clip = m_gameOverClips[m_gameOverClipsIndexOrder[m_gameoverClipsCurrentIndex]];
        }
        else if(m_currentMode == Mode.MANUAL)
        {
            m_musicAudioSource.clip = null;
            m_musicAudioSource.Stop();
        }

        if (m_musicAudioSource.isPlaying == false)
        {
            m_musicAudioSource.Play();
        }
    }

    private void SetClipMode(Mode mode)
    {
        if (mode == Mode.GAMEPLAY)
        {
            m_musicAudioSource.loop = false;
            m_musicAudioSource.time = 0;
            m_musicAudioSource.clip = m_gameplayClips[m_gameplayClipsIndexOrder[m_gameplayClipsCurrentIndex]];
        }
        else if (mode == Mode.MENU)
        {
            m_musicAudioSource.loop = false;
            m_musicAudioSource.time = 0;
            m_musicAudioSource.clip = m_menuClips[m_menuClipsIndexOrder[m_menuClipsCurrentIndex]];
        }
        else if (m_currentMode == Mode.GAMEOVER)
        {
            m_musicAudioSource.loop = false;
            m_musicAudioSource.time = 0;
            m_musicAudioSource.clip = m_gameOverClips[m_gameOverClipsIndexOrder[m_gameoverClipsCurrentIndex]];
        }
        else if(mode == Mode.MANUAL)
        {
            m_musicAudioSource.loop = m_loop;
            m_musicAudioSource.time = 0;
            if(m_manualAudioClip != null)
            {
                m_musicAudioSource.clip = m_manualAudioClip;
            }
            else
            {
                m_musicAudioSource.Stop();
            }
        }

        if(m_musicAudioSource.isPlaying == false)
        {
            m_musicAudioSource.volume = m_volume;
            m_musicAudioSource.Play();
        }
    }

    public void NextClip()
    {
        int index = AdvanceModeIndex(m_currentMode);
        if (index != -1)
        {
            SetClip(index);
        }
    }

    private int AdvanceModeIndex(Mode mode)
    {
        if (mode == Mode.GAMEPLAY)
        {
            if(m_gameplayClipsCurrentIndex == (m_gameplayClipsIndexOrder.Length - 1))
            {
                if(m_loop == false)
                {
                    return -1;
                }
                m_gameplayClipsCurrentIndex = 0;
            }
            else
            {
                m_gameplayClipsCurrentIndex++;
            }
            return m_gameplayClipsCurrentIndex;
        }
        else if (mode == Mode.MENU)
        {
            if (m_menuClipsCurrentIndex == (m_menuClipsIndexOrder.Length - 1))
            {
                if (m_loop == false)
                {
                    return -1;
                }
                m_menuClipsCurrentIndex = 0;
            }
            else
            {
                m_menuClipsCurrentIndex++;
            }
            return m_menuClipsCurrentIndex;
        }
        if (mode == Mode.GAMEOVER)
        {
            if (m_gameoverClipsCurrentIndex == (m_gameOverClipsIndexOrder.Length - 1))
            {
                if (m_loop == false)
                {
                    return -1;
                }
                m_gameoverClipsCurrentIndex = 0;
            }
            else
            {
                m_gameoverClipsCurrentIndex++;
            }
            return m_gameoverClipsCurrentIndex;
        }

        return -1;
    }

    private static int[] GenerateRandomSequence(int n)
    {
        int[] sequence = Enumerable.Range(0, n).ToArray();
        System.Random random = new System.Random();

        // Shuffle the array
        for (int i = 0; i < sequence.Length; ++i)
        {
            int randomIndex = random.Next(sequence.Length);
            int temp = sequence[randomIndex];
            sequence[randomIndex] = sequence[i];
            sequence[i] = temp;
        }

        return sequence;
    }

}
