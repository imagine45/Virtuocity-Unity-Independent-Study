using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public Transform groundcheck;
    public LayerMask groundLayer;
    //public ParticleSystem particles;
    //public ParticleSystem groundSlamParticles;

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
    private bool canGroundSmash = false;
    private bool isGroundSmashing = false;

    private float accelerationCap = 1.0f;
    private float acceleration = 0.1f;
    private float airAcceleration = 0.1f;

    public float groundAcceleration = 0.1f;
    public float groundDeceleration = 0.2f;
    private float iceAcceleration = 0.02f;
    private float iceDeceleration = 0.005f;

    private float dx = 0f;
    //private float dy = 0f;

    private int coyoteTimer = 0;
    private int coyoteLimit = 5;
    private bool hasCoyoteTime;

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        print("is Grounded? " + isGrounded() + "   jumped? " + jumped + "   velocity: " + rb.velocity.y);
        if (isGrounded())
        {
            //if (isGroundSmashing == true) { groundSlamParticles.Play(); }
            //groundSlamParticles.particleCount.Equals(0);

            animator.SetBool("isFalling", false);

            coyoteTimer = 0;
            jumped = false;
            hasCoyoteTime = false;
            canGroundSmash = true;
            isGroundSmashing = false;
            //particles.Stop();

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
            groundSmashParticleCount();

            coyoteTime();

            airMovement();
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(dx * speed, rb.velocity.y);

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

    private void groundSmashParticleCount()
    {
        //groundSlamParticles.particleCount.Equals(groundSlamParticles.particleCount + 1);
    }
    public void groundSmashInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isGrounded() && canGroundSmash)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - groundSmashSpeed);
            canGroundSmash = false;
            isGroundSmashing = true;
            dx = 0;


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
        if (Mathf.Abs(dx) > accelerationCap) { dx = accelerationCap * horizontal; }
    }

    private void hitWall()
    {
        if (Physics2D.OverlapCircle(groundcheck.position, 0.3f, groundLayer))
        {

        }
    }

    private void airMovement()
    {
        if (horizontal != 0 && !isGroundSmashing)
        {
            dx += airAcceleration * horizontal;
        }
        else if (horizontal == 0 && Mathf.Abs(dx) > 0)
        {
            dx -= Mathf.Min(stopSpeed, Mathf.Abs(dx)) * Mathf.Sign(dx);
        }
        if (Mathf.Abs(dx) > accelerationCap) { dx = accelerationCap * horizontal; }
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
}
