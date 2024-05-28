using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedGate : MonoBehaviour
{
    private GameObject player;
    public Animator animator;
    public FMODUnity.EventReference OpenEventName;
    private FMOD.Studio.EventInstance speedDoorOpenInstance;
    public FMODUnity.EventReference CloseEventName;
    private FMOD.Studio.EventInstance speedDoorCloseInstance;
    public float openSpeed = 1;
    private float initY;
    private float playerY;
    private float playerX;
    private float speed;
    private bool isPlayerUnder = false;
    private bool isOpened = false;

    private void Start()
    {
        player = GameObject.Find("Player");
        initY = transform.position.y - 3;
        speedDoorOpenInstance = FMODUnity.RuntimeManager.CreateInstance(OpenEventName);
        speedDoorCloseInstance = FMODUnity.RuntimeManager.CreateInstance(CloseEventName);
    }

    private void Update()
    {
        // Update the sound position to match the drone's position
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D
        {
            position = RuntimeUtils.ToFMODVector(transform.position),
            forward = RuntimeUtils.ToFMODVector(transform.forward),
            up = RuntimeUtils.ToFMODVector(transform.up)
        };
        speedDoorOpenInstance.set3DAttributes(attributes);
        speedDoorCloseInstance.set3DAttributes(attributes);
    }

    private void FixedUpdate()
    {
        speed = player.GetComponent<PlayerController>().getSpeed();

        playerX = player.GetComponent<PlayerController>().transform.position.x;
        playerY = player.GetComponent<PlayerController>().transform.position.y;

        if (playerY >= initY - 3 && playerX >= transform.position.x -.4 && playerX <= transform.position.x + .4)
        {
            isPlayerUnder = true;
        } else
        {
            isPlayerUnder = false;
        }

        if (speed >= openSpeed || isPlayerUnder)
        {
            animator.SetBool("doorOpen", true);
            if (isOpened == false) { speedDoorOpenInstance.start(); }
            isOpened = true;
        } else
        {
            animator.SetBool("doorOpen", false);
            if (isOpened == true) { speedDoorCloseInstance.start(); }
            isOpened = false;
        }
    }

    private void OnDestroy()
    {
        speedDoorOpenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        speedDoorOpenInstance.release();

        speedDoorCloseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        speedDoorCloseInstance.release();
    }
}
