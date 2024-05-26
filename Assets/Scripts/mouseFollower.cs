using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouseFollower : MonoBehaviour
{
    Vector3 mousePos;
    float moveLim = 50f;
    List<GameObject> assets = new List<GameObject>();
    private GameObject timer;
    private Color c;

    private void Start()
    {
        timer = GameObject.Find("FMODEvents");
        Timer.beatUpdated += beatEffect;

        foreach (Transform child in transform)
        {
            assets.Add(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= beatEffect;
    }

    void Update()
    {
        followMouse();
    }

    private void followMouse()
    {
        mousePos = Input.mousePosition;

        foreach (GameObject obj in assets)
        {
            var zMult = obj.transform.position.z;
            obj.transform.position = new Vector3(Screen.width / 2 + mousePos.x / zMult * 1.05f, Screen.height / 2 + mousePos.y / zMult * 1.05f, zMult);
        }
    }

    private void beatEffect()
    {
        foreach (GameObject obj in assets)
        {
            c = obj.GetComponent<RawImage>().color;
            obj.GetComponent<RawImage>().color = new Color((c.r + 0.4f) % 1, (c.g + 0.35f) % 1, (c.b + 0.64f) % 1);
        }
    }
}
