using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeatTracker : MonoBehaviour
{

    public TMP_Text canvasText;
    public GameObject timer;
    private int bar;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bar = timer.GetComponent<Timer>().currentBar;
        canvasText.text = "Current Bar: " + bar;
    }
}
