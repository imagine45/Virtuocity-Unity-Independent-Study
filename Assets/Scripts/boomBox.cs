using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomBox : MonoBehaviour
{

    public FMODUnity.EventReference EventName;
    private FMOD.Studio.EventInstance boomboxInstance;

    [SerializeField] Animator animator;
    [SerializeField] bool[] beats;
    private GameObject timer;
    private GameObject player;
    private double time;
    private bool inDistance;

    private void Start()
    {
        boomboxInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);
        timer = GameObject.Find("FMODEvents");
        player = GameObject.Find("Player");
        Timer.beatUpdated += explode;
    }

    private void OnDestroy()
    {
        Timer.beatUpdated -= explode;
        boomboxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        boomboxInstance.release();
    }

    private void Update()
    {
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D
        {
            position = RuntimeUtils.ToFMODVector(transform.position),
            forward = RuntimeUtils.ToFMODVector(transform.forward),
            up = RuntimeUtils.ToFMODVector(transform.up)
        };
        boomboxInstance.set3DAttributes(attributes);
    }


    private void explode()
    {
        if (beats[timer.GetComponent<Timer>().currentBeat - 1])
        {
            animator.SetTrigger("explode");
            boomboxInstance.start();

            startTime(Vector2.Distance(player.transform.position, this.transform.position));
            
        }
    }

    private void startTime(double t)
    {

        time = t;
        
    }

    private void FixedUpdate()
    {
        if (time > 0)
        {
            time -= 20f * Time.deltaTime;
        }
        if (time < 0)
        {
            if (inDistance)
            {
                player.GetComponent<PlayerController>().kill();
                //Debug.Log("Kill");
            }
            time = 0;
        }

        inDistance = (Vector2.Distance(player.transform.position, this.transform.position) <= 4.3f);

        //Debug.Log(inDistance);
    }
}
