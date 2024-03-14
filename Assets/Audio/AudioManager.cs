using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    private List<EventInstance> eventInstances;

    private EventInstance musicEventInstance;
    private EventInstance footstepEventInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            eventInstances = new List<EventInstance>();
            InitializeMusic(FMODEvents.instance.music);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }
    private void InitializeFootsteps(EventReference footstepEventReference)
    {
        footstepEventInstance = CreateInstance(footstepEventReference);
    }
    public void SetMusicArea(MusicArea area) 
    {
        musicEventInstance.setParameterByName("Time Signature", (float) area);        
    }

    public void SetMusicIntensity(float intensity)
    {
        musicEventInstance.setParameterByName("Song Intensity", intensity);
    }

    public void SetFootstepType(float ground)
    {
        footstepEventInstance.setParameterByName("Ground Type", ground);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
}
