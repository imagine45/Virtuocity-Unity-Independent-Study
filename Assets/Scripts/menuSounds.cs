using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuSounds : MonoBehaviour
{

    private void Start()
    {
        
    }

    public void onEnter()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/HUD/Button Hover");
    }
    public void onClick()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/HUD/Button Click");
    }
}
