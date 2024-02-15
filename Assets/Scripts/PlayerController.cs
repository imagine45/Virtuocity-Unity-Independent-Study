using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public Transform groundcheck;
    public LayerMask groundLayer;
    public ParticleSystem particles;
    public ParticleSystem chargeParticles;
    //private int groundSlamParticleCount = 0;

    public Animator animator;

    private float horizontal;
    private float speed = 7f;
    private float jumpingPower = 14f;
    private bool isFacingRight = true;
    private bool jumped = false;
    private bool buffered = false;
    private float stopSpeed = 0.2f;
    private float groundSmashSpeed = 25f;
    private bool charged = false;

    private float accelerationCap = 1.0f;
    private float acceleration = 0.1f;
    private float airAcceleration = 0.1f;

    public float groundAcceleration = 0.1f;
    public float groundDeceleration = 0.2f;
    private float iceAcceleration = 0.02f;
    private float iceDeceleration = 0.005f;
    private int scaleDivisor = 1;

    private float dx = 0f;
    //private float dy = 0f;

    private int coyoteTimer = 0;
    private int coyoteLimit = 5;
    private bool hasCoyoteTime;

    private bool crouching = false;


    private void Awake()
    {
        Application.targetFrameRate = 120;
    }
    void Start()
    {

    }

    public float getSpeed ()
    {
        return Mathf.Abs(dx); 
    }
    private void FixedUpdate()
    {
        //dx * speed applied to player velocity
        rb.velocity = new Vector2(dx * speed, rb.velocity.y);

        if (Mathf.Abs(dx) > 1.5) { chargeParticles.Play(); }
        else { chargeParticles.Stop(); charged = false; animator.SetFloat("runningSpeed", 1);  }

        hitWall();

        if (isGrounded())
        {

            animator.SetBool("isFalling", false);

            coyoteTimer = 0;
            jumped = false;
            hasCoyoteTime = false;

            if (buffered)
            {
                jump();
            }

            groundMovement();
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("jumped", false);
            }

            coyoteTime();

            airMovement();
        }

        if (accelerationCap > 1) { accelerationCap -= Mathf.Max(0.02f, accelerationCap - Mathf.Abs(dx)); } else { accelerationCap = 1; }
        if (Mathf.Abs(dx) > accelerationCap) { dx = accelerationCap * horizontal; }

    }

    // Update is called once per frame
    void Update()
    {

        if (!isFacingRight && horizontal > 0 || isFacingRight && horizontal < 0)
        {
            flip();
        }
        if (Mathf.Abs(dx) < 0.05f && horizontal == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }

    private void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        jumped = true;
        buffered = false;
        animator.SetBool("jumped", true);
        animator.SetBool("isFalling", false);
    }
    public void jumpInput(InputAction.CallbackContext context)
    {
        //normal jump
        if (context.performed && (isGrounded() || hasCoyoteTime))
        {
            jump();
        }

        //buffer jump
        if (context.performed && !isGrounded() && canBuffer())
        {
            buffered = true;
        }

        //cancel jump
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            animator.SetBool("isFalling", true);
            animator.SetBool("jumped", false);
        }
    }

    public void chargeInput(InputAction.CallbackContext context)
    {
        if (context.performed && horizontal != 0)
        {
            print("Charged");

            animator.SetFloat("runningSpeed", 1 + dx / 2);
            charged = true;
            accelerationCap += 2;
            dx = accelerationCap * horizontal;


            //particles.Play();
        }
    }
    private void coyoteTime()
    {
        if (coyoteTimer < coyoteLimit && !jumped) { coyoteTimer++; }
        hasCoyoteTime = coyoteTimer < coyoteLimit && !jumped;
    }
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.4f, groundLayer) && Mathf.Abs(rb.velocity.y) <= 0.01f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            acceleration = groundAcceleration;
            stopSpeed = groundDeceleration;
        }
        if (other.gameObject.CompareTag("Ice"))
        {
            acceleration = iceAcceleration;
            stopSpeed = iceDeceleration;
        }
    }

    private bool canBuffer()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.6f, groundLayer) && rb.velocity.y < 0;
    }

    private void groundMovement()
    {
        if (horizontal != 0)
        {
            dx += acceleration * horizontal;
        }
        else if (horizontal == 0 && Mathf.Abs(dx) > 0)
        {
            dx -= Mathf.Min(stopSpeed, Mathf.Abs(dx)) * Mathf.Sign(dx);
        }
    }

    private void hitWall()
    {
        int playerLayer = 9;
        int layerMask = ~(1 << playerLayer);

        RaycastHit2D leftHitLow = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), (Vector2.left), .4f, layerMask);
        RaycastHit2D leftHitHigh = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.6f), (Vector2.left), .4f, layerMask);
        RaycastHit2D rightHitLow = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), (Vector2.right), .4f, layerMask);
        RaycastHit2D rightHitHigh = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.6f), (Vector2.right), .4f, layerMask);

        if (leftHitLow || rightHitLow || leftHitHigh || rightHitHigh)
        {
            accelerationCap = 1;
            dx = 0;
        }
    }

    private void airMovement()
    {
        if (horizontal != 0)
        {
            dx += airAcceleration * horizontal;
        }
        else if (horizontal == 0 && Mathf.Abs(dx) > 0)
        {
            dx -= Mathf.Min(stopSpeed, Mathf.Abs(dx)) * Mathf.Sign(dx);
        }
    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void moveInput(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    

    public void crouchInput(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded())
        {
            scaleDivisor = 2;
            //Debug.Log("crouching!");
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / scaleDivisor, transform.localScale.z);
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y / scaleDivisor);
        }
        //else{ fastfall ?}
        if (context.canceled)
        {
            //Debug.Log("stopped crouching!");
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * scaleDivisor, transform.localScale.z);
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y * scaleDivisor);
            scaleDivisor = 1;
        }
    }
}
