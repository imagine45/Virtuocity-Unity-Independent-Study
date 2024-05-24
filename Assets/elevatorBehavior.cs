using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorBehavior : MonoBehaviour 
{
    public GameObject keySprite;
    private GameObject player;
    private float radius;
    private float fadeRadius;
    private bool inRadius = false;
    private SpriteRenderer indicatorSpriteRenderer;
    private Vector2 buttonPos;

    private Rigidbody2D rb;
    private bool isMoving = false;
    void Start()
    {
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
        rb.velocity = (isMoving) ? new Vector3(0, 1, 0) : new Vector3(0, 0, 0);
    }

    public void Move()
    {
        if (inRadius)
        {
            isMoving = !isMoving;
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
}
