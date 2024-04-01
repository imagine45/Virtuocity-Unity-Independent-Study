using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathTransition : MonoBehaviour
{
    [SerializeField] GameObject player;
    public Animator animator;


    void Update()
    {

        animator.SetBool("isDead", player.GetComponent<PlayerController>().isDead);
        
    }
}
