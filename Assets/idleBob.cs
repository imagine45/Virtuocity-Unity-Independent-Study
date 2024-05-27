using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idleBob : MonoBehaviour
{

    private float time = 0;
    private float yPos;

    // Start is called before the first frame update
    void Start()
    {
        yPos = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (time >= Mathf.PI)
        {
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
        }

        this.transform.position = new Vector3(transform.position.x, yPos + Mathf.Sin(time) / 4, 0);
    }
}
