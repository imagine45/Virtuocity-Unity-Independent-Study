using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerRoomScript : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] GameObject keySprite;
    private GameObject player;
    private GameObject sky;
    private float radius;
    private bool inRadius = false;
    private bool clicked = false;
    private SpriteRenderer indicatorSpriteRenderer;
    private Vector2 buttonPos;

    void Start()
    {
        player = GameObject.Find("Player");
        sky = GameObject.Find("Neon City Background Sky");
        radius = 5f;
        buttonPos = new Vector2(180, 11);

        if (keySprite != null)
        {
            indicatorSpriteRenderer = keySprite.GetComponent<SpriteRenderer>();
            if (indicatorSpriteRenderer != null)
            {
                Color color = indicatorSpriteRenderer.color;
                color.a = 0f; // Set the initial alpha to 0
                indicatorSpriteRenderer.color = color;
                //keySprite.SetActive(false);
            }
        }
    }

    void Update()
    {
        InRadius();
    }

    public void PowerOn()
    {
        if (inRadius && !clicked)
        {
            animator.SetTrigger("powerOn");
            //playButtonSound();
            clicked = true;
            Object.Destroy(keySprite);

            sky.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
    }


    private void InRadius()
    {
        if (Vector2.Distance(player.transform.position, buttonPos) <= radius && !clicked)
        {
            Color color = indicatorSpriteRenderer.color;
            color.a = Mathf.Clamp01(radius - Vector2.Distance(player.transform.position, buttonPos));
            indicatorSpriteRenderer.color = color;

            inRadius = true;
        }
        else
        {
            inRadius = false;
        }
    }

    public bool isClicked()
    {
        return clicked;
    }
}
