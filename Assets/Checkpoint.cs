using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            collider.GetComponent<PlayerController>().changeCheckpoint(new Vector2(this.transform.position.x, this.transform.position.y));
        }
    }
}
