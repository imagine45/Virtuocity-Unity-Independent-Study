using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorBehavior : MonoBehaviour 
{
    public GameObject keySprite;
    private GameObject player;
    private Animator animator;
    private float radius;
    private float fadeRadius;
    private bool inRadius = false;
    private SpriteRenderer indicatorSpriteRenderer;
    private Vector2 buttonPos;

    public FMODUnity.EventReference ButtonEvent;
    public FMODUnity.EventReference ElevatorEvent;
    private FMOD.Studio.EventInstance buttonInstance;
    private FMOD.Studio.EventInstance elevatorInstance;

    private Rigidbody2D rb;
    private bool isMoving = false;
    void Start()
    {
        buttonInstance = FMODUnity.RuntimeManager.CreateInstance(ButtonEvent);
        elevatorInstance = FMODUnity.RuntimeManager.CreateInstance(ElevatorEvent);

        buttonPos = new Vector2(-30, 12);
        rb = this.GetComponent<Rigidbody2D>();

        if (keySprite != null)
        {
            indicatorSpriteRenderer = keySprite.GetComponent<SpriteRenderer>();
            if (indicatorSpriteRenderer != null)
            {
                Color color = indicatorSpriteRenderer.color;
                color.a = 0f; // Set the initial alpha to 0
                indicatorSpriteRenderer.color = color;
                keySprite.SetActive(false);
            }
        }

        player = GameObject.Find("Player");
        radius = 3;
        fadeRadius = 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InRadius();
        //rb.velocity = (isMoving) ? new Vector3(0, 1, 0) : new Vector3(0, 0, 0);
    }

    public void playElevatorSound()
    {
        elevatorInstance.start();
    }

    private void playButtonSound()
    {
        buttonInstance.start();
    }

    public void Move()
    {
        if (inRadius && !isMoving)
        {
            GameObject.Find("Elevator Movement Animation Parent").GetComponent<Animator>().SetTrigger("animStart");
            playButtonSound();
            playElevatorSound();
            isMoving = true;
        }
    }

    //Find if the player is in radius
    private void InRadius()
    {
        if (Vector2.Distance(player.transform.position, buttonPos) <= radius)
        {
            Color color = indicatorSpriteRenderer.color;
            color.a = Mathf.Clamp01(fadeRadius - Vector2.Distance(player.transform.position, buttonPos));
            indicatorSpriteRenderer.color = color;

            keySprite.SetActive(true);
            inRadius = true;
        }
        else
        {
            keySprite.SetActive(false);
            inRadius = false;
        }
    }

    private void OnDestroy()
    {
        buttonInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        buttonInstance.release();

        elevatorInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        elevatorInstance.release();
    }
}
