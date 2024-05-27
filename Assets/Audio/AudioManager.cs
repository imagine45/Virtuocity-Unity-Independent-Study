using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    private List<EventInstance> eventInstances;

    private EventInstance musicEventInstance;
    private EventInstance ambianceEventInstance;
    private EventInstance footstepEventInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            eventInstances = new List<EventInstance>();
            InitializeAmbiance(FMODEvents.instance.ambiance);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Debug.Log("Menu Screen");
            Destroy(gameObject);
        }
    }
    private void InitializeAmbiance(EventReference ambianceEventReference)
    {
        ambianceEventInstance = CreateInstance(ambianceEventReference);
        ambianceEventInstance.start(); 
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
