using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{
    
    public GameObject player;
    public Animator animator;
    
    void Update()
    {
        animator.SetBool("isDead", player.GetComponent<PlayerController>().isDead);
    }
    
}
