using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class postProcessing : MonoBehaviour
{
    public GameObject player;
    public GameObject beatTracker;
    private Volume volume;
    private LensDistortion lensDistortion;
    private Bloom bloom;

    private float speed;

    private void Awake()
    {
        Timer.beatUpdated += onBeat;
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= onBeat;
    }

    private void FixedUpdate()
    {
        speed = player.GetComponent<PlayerController>().getSpeed();

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out bloom);

        lensDistortion.intensity.value = -speed / 20;

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
