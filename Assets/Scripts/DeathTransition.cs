using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{
    public Transform tr;
    private Vector3 currentPos = tr.position;
    private Camera mainCam;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = FindGameObjectsWithTag("MainCamera");
        transform.position = new Vector3(mainCam.transform.position.x + 142, mainCam.transform.position.y, 0);
        player = FindGameObjectsWithTag("Player");
        StartCoroutine(WaitForSeconds(3.0f));
        for(int i = 142;  tr.position.x > mainCam.getComponent<Transform>.position.x-142; i-=2)
        {
            transform.position = new Vector3(mainCam.transform.position.x + i, mainCam.transform.position.y, 0);
            if (tr.position.x < mainCam.getComponent<Transform>.position.x+60)
            {
                mainCam.position = new Vector3(71.8, 9.3, -10);
                player.respawnCharacter();
            }

        }
    }
}
