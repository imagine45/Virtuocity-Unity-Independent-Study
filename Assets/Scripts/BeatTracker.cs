using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeatTracker : MonoBehaviour
{

    public TMP_Text canvasText;
    private GameObject timer;
    private int bar;

    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.Find("FMODEvents");
    }

    // Update is called once per frame
    void Update()
    {
        bar = timer.GetComponent<Timer>().currentBeat;
        canvasText.text = "Current Bar: " + bar;
    }
}
