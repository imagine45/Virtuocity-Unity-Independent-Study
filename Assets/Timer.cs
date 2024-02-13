using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private static Timer TimerInstance;

    public float BPM;
    public float[] timeSignature = new float[2];
    private float measureInterval, beatInterval;
    private float measure;
    private float beat;
    private bool beatFull, measureFull;
    private int measureCountFull, beatCountFull;

    private void Awake()
    {
        if (TimerInstance != null && TimerInstance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            TimerInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void FixedUpdate()
    {
        measureFull = false;
        measureInterval = (60 * timeSignature[1]) / BPM;
        measure += Time.deltaTime;

        if (measure >= measureInterval)
        {
            measure -= measureInterval;
            measureFull = true;
            measureCountFull++;
            //Debug.Log("beat");
            //beat -= beatInterval;
        }

        beatFull = false;
        beatInterval = 60 / (BPM * timeSignature[0]);
        beat += Time.deltaTime;

        if (beat >= beatInterval)
        {
            beat -= beatInterval;
            beatFull = true;
            beatCountFull++;
            Debug.Log("tick");
        }

    }
}
