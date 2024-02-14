using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{

    public TMP_Text canvasText;
    public GameObject player;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speed = (int)(player.GetComponent<PlayerController>().getSpeed() * 100) / 100f;
        canvasText.text = "Speed: " + speed + " units";
    }
}
