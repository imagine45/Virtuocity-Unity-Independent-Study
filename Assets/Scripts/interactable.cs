using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactable : MonoBehaviour
{
    public GameObject keySprite; 
    private GameObject player;
    private float radius;
    private float fadeRadius;
    private SpriteRenderer indicatorSpriteRenderer;

    private void Start()
    {
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

    private void FixedUpdate()
    {
        inRadius();
    }

    //Find if the player is in radius
    private void inRadius()
    {
        if (Vector2.Distance(player.transform.position, this.transform.position) <= radius)
        {
            Color color = indicatorSpriteRenderer.color;
            color.a = Mathf.Clamp01(fadeRadius - Vector2.Distance(player.transform.position, this.transform.position));
            indicatorSpriteRenderer.color = color;

            keySprite.SetActive(true);
            //StartCoroutine(FadeIn());
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interacting");
            }
            Debug.Log("in radius!");
        } else
        {
            //StopAllCoroutines();
            keySprite.SetActive(false);
        }
    }
}
