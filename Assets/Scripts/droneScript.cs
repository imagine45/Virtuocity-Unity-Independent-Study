using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droneScript : MonoBehaviour
{
    public FMODUnity.EventReference EventName;
    private FMOD.Studio.EventInstance droneInstance;

    private void Start()
    {
        droneInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);
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
        droneInstance.set3DAttributes(attributes);
    }

    public void StartSound()
    {
        droneInstance.start();
    }

    public void EndSound()
    {
        droneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        droneInstance.release();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EndSound();
    }
}

public static class RuntimeUtils
{
    public static FMOD.VECTOR ToFMODVector(Vector3 vector)
    {
        return new FMOD.VECTOR { x = vector.x, y = vector.y, z = vector.z };
    }
}
