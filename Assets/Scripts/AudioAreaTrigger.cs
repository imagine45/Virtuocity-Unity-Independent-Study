using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAreaTrigger : MonoBehaviour
{

    public AudioSource audioSource;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag=="Player" && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("playing!");
        }
    }
}
