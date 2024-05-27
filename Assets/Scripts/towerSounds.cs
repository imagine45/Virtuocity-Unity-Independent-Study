using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towerSounds : MonoBehaviour
{

    private GameObject player;
    public FMODUnity.EventReference HumEventName;
    public FMODUnity.EventReference RevealEventName;
    private FMOD.Studio.EventInstance towerHumInstance;
    private FMOD.Studio.EventInstance towerRevealInstance;
    private float radius = 30f;
    private float min = 5f;
    private bool soundPlayed = false;

    void Awake()
    {
        player = GameObject.Find("Player");
        towerHumInstance = FMODUnity.RuntimeManager.CreateInstance(HumEventName);
        towerRevealInstance = FMODUnity.RuntimeManager.CreateInstance(RevealEventName);

        towerHumInstance.start();
    }

    void Update()
    {
        var distance = Mathf.Clamp01(Vector3.Distance(player.transform.position, this.transform.position) / radius);
        Debug.Log(distance);
        //if (Vector3.Distance(player.transform.position, this.transform.position) <= min) { distance = 0; }
        if (player.transform.position.y <= this.transform.position.y - 10) { distance = 1;  }

        towerHumInstance.setParameterByName("Distance", (float) distance);

        if (distance <= 0.5f && !soundPlayed)
        {
            soundPlayed = true;
            towerRevealInstance.start();
        }
        
    }

    private void OnDestroy()
    {
        towerHumInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        towerHumInstance.release();

        towerRevealInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        towerRevealInstance.release();
    }
}
