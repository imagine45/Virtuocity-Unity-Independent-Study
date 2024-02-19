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
    private bool charged = false;

    //the amount of charge given by batteries
    private float batteryCharge = 2.5f;

    private float chargeMeter;
    private float chargeMeterCap = 10f;

    //units decreased per second
    private float chargeDecreaseRate = 1f;

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

    private bool checkpointSet = false;
    private Vector2 checkpoint;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }
    void Start()
    {
        resetCharacter();
    }

    public float getSpeed ()
    {
        return Mathf.Abs(dx); 
    }
    private void FixedUpdate()
    {
        //dx * speed applied to player velocity
        rb.velocity = new Vector2(dx * speed, rb.velocity.y);
        animator.SetFloat("runningSpeed", 1 + dx / 2);

        if (Mathf.Abs(dx) <= 1.5) { chargeParticles.Stop(); charged = false; }

        //natural decrease of the player charge meter
        chargeDecrease();

        hitWall();

        AudioManager.instance.SetMusicIntensity((getSpeed() / 6 >= 1 ? 1 : getSpeed() / 6));

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
        if (context.performed && horizontal != 0 && chargeMeter > 0)
        {
            chargeParticles.Play();
            charged = true;
            accelerationCap += 2;
            dx = accelerationCap * horizontal;
            chargeMeter -= 2f;
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

    //for collectibles 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            batteryCollect(other.gameObject);
            if (other.GetComponent<collectibleBehavior>().getState().Equals("Respawns")) { StartCoroutine(collectibleRespawn(other, 5)); }
        }

        if (other.gameObject.CompareTag("SpeedBoost"))
        {
            speedBoostCollect(other.gameObject);
            if (other.GetComponent<collectibleBehavior>().getState().Equals("Respawns")) { StartCoroutine(collectibleRespawn(other, 5)); }
        }
    }

    private bool canBuffer()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.6f, groundLayer) && rb.velocity.y < 0;
    }

    private void resetCharacter()
    {
        dx = 0;
        accelerationCap = 1;
        chargeMeter = 0;
    }

    IEnumerator collectibleRespawn (Collider2D collision, int time)
    {
        yield return new WaitForSeconds(time);

        collision.gameObject.SetActive(true);
    }

    public void batteryCollect(GameObject other)
    {
        chargeMeter += Mathf.Min(batteryCharge, chargeMeterCap - batteryCharge);
        other.SetActive(false);
    }

    public void speedBoostCollect(GameObject other)
    {

        var direction = other.GetComponent<collectibleBehavior>().getDirection();
        float boostAmount = 3;

        accelerationCap += boostAmount - 1;

        if (direction.Equals("Player"))
        {
            if (horizontal > 0)
            {
                dx += boostAmount;
            }
            else if (horizontal < 0)
            {
                dx -= boostAmount;
            }
            else
            {
                dx += (isFacingRight) ? boostAmount : -boostAmount;
            }
        }
        if (direction.Equals("Right")) { dx += boostAmount; }
        //if (direction.Equals("Down-right")) { dx += boostAmount; }
        //if (direction.Equals("Down")) { dx += boostAmount; }
        //if (direction.Equals("Down-left")) { dx += boostAmount; }
        if (direction.Equals("Left")) { dx += -boostAmount; }
        //if (direction.Equals("Up-left")) { dx += boostAmount; }
        //if (direction.Equals("Up")) { dy += boostAmount; }
        //if (direction.Equals("Up-right")) { dx += boostAmount; }

        other.SetActive(false);
    }


    private void chargeDecrease()
    {
        if (chargeMeter <= chargeMeterCap && chargeMeter > 0)
        {
            chargeMeter -= chargeDecreaseRate * Time.deltaTime;
        } else if (chargeMeter <= 0)
        {
            chargeMeter = 0;
        }
        print(chargeMeter); 
    }

    private float getCharge()
    {
        return chargeMeter;
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

    public void reset(InputAction.CallbackContext context)
    {
        if (checkpointSet)
        {
            rb.position = checkpoint;
            resetCharacter();
        } else
        {
            print("No respawn point set");
        }
    }

    public void setCheckpoint(InputAction.CallbackContext context)
    {
        checkpointSet = true;
        checkpoint = new Vector2(rb.position.x, rb.position.y);
        print("respawn set at (" + checkpoint.x + ", " + checkpoint.y + ")");
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
