using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Script_Countdown : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Remaining time available")] 
    [SerializeField] float m_TimeLeft = 120.0f;

    [Header("Audio Clips")]
    [SerializeField] AudioClip m_TickTock;
    [SerializeField] AudioClip m_HeartBeatSlow;
    [SerializeField] AudioClip m_HeartBeatMid;
    [SerializeField] AudioClip m_HeartBeatFast;

    [Header("Post-Process Effect")]
    [Tooltip("A post process effect applied when the time is over (and so is the game)")]
    [SerializeField] PostProcessProfile m_PostProcessProfile;

    [Header("Slot values for different fatigue/anxiety levels according to remaining time. (To be refactored by using triggers and conditions)")]
    [SerializeField] float m_Slot3 = 90.0f;
    [SerializeField] float m_Slot2 = 60.0f;
    [SerializeField] float m_Slot1 = 30.0f;
    [SerializeField] float m_Slot0 = 0.0f;

    [Header("Color saturation in Post-Process effect. (To be refactored by using triggers and conditions)")]
    [SerializeField] int m_SaturationSlot3 = 0;
    [SerializeField] int m_SaturationSlot2 = 50;
    [SerializeField] int m_SaturationSlot1 = 100;

    private AudioSource m_AudioSourceHeartBeat;
    private AudioSource m_AudioSourceTickTock;
    private PostProcessVolume m_PostProcessVolume; // Reference to Post Process Volume added at runtime
    private ColorGrading m_ColorGrading; // Reference used to update saturation color in PostProcess
    
    private Script_GameController m_Script_GameController;

    private bool m_Frozen = false;

    void Start()
    {
        m_AudioSourceTickTock = gameObject.AddComponent<AudioSource>();
        m_AudioSourceTickTock.playOnAwake = false;
        m_AudioSourceTickTock.clip = m_TickTock;
        m_AudioSourceTickTock.volume = 0.2f;

        m_AudioSourceHeartBeat = gameObject.AddComponent<AudioSource>();
        m_AudioSourceHeartBeat.playOnAwake = false;

        m_PostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
        m_PostProcessVolume.isGlobal = true;
        m_PostProcessVolume.profile = m_PostProcessProfile;
        m_PostProcessVolume.enabled = false;

        // https://answers.unity.com/questions/1355103/modifying-the-new-post-processing-stack-through-co.html
        m_PostProcessVolume.profile.TryGetSettings<ColorGrading>(out m_ColorGrading);

        m_Script_GameController = FindObjectOfType<Script_GameController>();
    }

    void Update()
    {
        if (!m_Frozen)
        {
            m_TimeLeft -= Time.deltaTime;
            m_Script_GameController.DisplayCountdown((m_TimeLeft).ToString("0"));

            CheckRemainingTime();
        }
    }

    void CheckRemainingTime()
    {
        if (m_TimeLeft > m_Slot3)
        {
            m_PostProcessVolume.enabled = false;
            m_AudioSourceHeartBeat.Stop();
        }
        else if (m_TimeLeft > m_Slot2)
        {
            SetIntensity(m_HeartBeatSlow, m_SaturationSlot3);
        }
        else if (m_TimeLeft > m_Slot1)
        {
            SetIntensity(m_HeartBeatMid, m_SaturationSlot2);
        }
        else if (m_TimeLeft > m_Slot0)
        {
            SetIntensity(m_HeartBeatFast, m_SaturationSlot1);
        }
        else
        {
            m_Script_GameController.EndLevel(Script_GameController.EndOption.Lose);
        }
    }
    void SetIntensity(AudioClip audioClip,  int saturation)
    {
        m_PostProcessVolume.enabled = true;
        m_ColorGrading.saturation.value = saturation;

        if (!m_AudioSourceHeartBeat.isPlaying)
        {
            m_AudioSourceHeartBeat.clip = audioClip;
            m_AudioSourceHeartBeat.Play();
        }
    }

    public void IncreaseTime(float time)
    {
        m_TimeLeft += time;
    }

    public void DecreaseTime(float time)
    {
        m_TimeLeft -= time;
    }

    public void Freeze(bool active)
    {
        m_Frozen = active;
    }

    private void OnEnable()
    {
        if (m_Script_GameController && m_AudioSourceTickTock) 
        {
            m_Script_GameController.ShowCountdown(true);
            if (!m_AudioSourceTickTock.isPlaying) // play one clock tick
            {
                m_AudioSourceTickTock.Play();
            }
        }
    }

    private void OnDisable()
    {
        if (m_Script_GameController)
        {
            m_Script_GameController.ShowCountdown(false);
        }
    }
}

