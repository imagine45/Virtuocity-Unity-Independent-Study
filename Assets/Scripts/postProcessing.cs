using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class postProcessing : MonoBehaviour
{
    private GameObject player;
    public GameObject beatTracker;
    private Volume volume;
    private LensDistortion lensDistortion;
    private Bloom bloom;

    private bool noPlayer = true;
    private float speed;

    private void Awake()
    {
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player");
            noPlayer = false;
        }
        Timer.beatUpdated += onBeat;
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= onBeat;
    }

    private void FixedUpdate()
    {
        if (!noPlayer)
        {
            speed = player.GetComponent<PlayerController>().getSpeed();
            volume.profile.TryGet(out lensDistortion);
            lensDistortion.intensity.value = -speed / 20;
        }

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out bloom);


        if (bloom.intensity.value >= 1)
        {
            bloom.intensity.value -= 0.5f; 
        } else
        {
            bloom.intensity.value = 1;
        }
    }

    public void onBeat()
    {
        bloom.intensity.value = 10.0f;
    }
}
