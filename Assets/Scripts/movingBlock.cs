using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingBlock : MonoBehaviour
{
    public FMODUnity.EventReference EventName;
    private FMOD.Studio.EventInstance movingBlockInstance;

    [SerializeField] float timeToMove;
    [SerializeField] bool[] beats;
    private GameObject keyFrame;
    private Rigidbody2D rb;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isMoving = false;
    private bool movingForward = false;
    private float speed;

    private GameObject timer;

    private void Start()
    {
        movingBlockInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);
        rb = GetComponent<Rigidbody2D>();
        timer = GameObject.Find("FMODEvents");
        Timer.beatUpdated += Move;
        keyFrame = GameObject.Find("Key Frame");
        startPos = this.transform.position;
        endPos = this.transform.Find("Key Frame").position;

        speed = Vector3.Distance(startPos, endPos) / timeToMove;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 targetPos = movingForward ? endPos : startPos;
            Vector3 direction = (targetPos - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            float moveDistance = speed * Time.fixedDeltaTime;

            if (moveDistance >= distanceToTarget)
            {
                rb.velocity = Vector3.zero;
                rb.position = targetPos;
                isMoving = false;
                movingForward = !movingForward;
            }
            else
            {
                rb.velocity = direction * speed;
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void Update()
    {
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D
        {
            position = RuntimeUtils.ToFMODVector(transform.position),
            forward = RuntimeUtils.ToFMODVector(transform.forward),
            up = RuntimeUtils.ToFMODVector(transform.up)
        };
        movingBlockInstance.set3DAttributes(attributes);
    }

    private void Move()
    {
        if (beats[timer.GetComponent<Timer>().currentBeat - 1])
        {
            isMoving = true; 
            movingBlockInstance.start();
        }
    }

    private void EndSound()
    {
        movingBlockInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        movingBlockInstance.release();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EndSound();
    }
}