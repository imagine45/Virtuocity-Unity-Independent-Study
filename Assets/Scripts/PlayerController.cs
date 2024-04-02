using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundcheck;
    public LayerMask groundLayer;
    public ParticleSystem particles;
    public ParticleSystem chargeParticles;
    public ParticleSystem chargeExplosion;
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
    private int dyCap = 15;

    //the amount of charge given by batteries
    private float batteryCharge = 2.5f;

    private float chargeMeter;
    private float chargeMeterCap = 10f;
    private int chargeUsage = 2;
    private float chargeLeniancyTimer = 0f;
    private float chargeBufferTimer = 0f;
    private bool chargeBuffer = false; 

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

    public bool isDead = false;

    private float dx = 0f;
    //private float dy = 0f;

    private int coyoteTimer = 0;
    private int coyoteLimit = 5;
    private bool hasCoyoteTime;

    private bool crouching = false;

    private bool checkpointSet = false;
    private Vector2 checkpoint;

    //Audio
    private EventInstance playerFootsteps;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }
    void Start()
    {
        resetCharacter();
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
    }

    public float getSpeed ()
    {
        return Mathf.Abs(dx); 
    }

    public float getCharge ()
    {
        return chargeMeter;
    }
    private void FixedUpdate()
    {

        //dx * speed applied to player velocity
        if (!isDead) { rb.velocity = new Vector2(dx * speed, Mathf.Sign(rb.velocity.y) * Mathf.Min(Mathf.Abs(rb.velocity.y), dyCap)); }
        animator.SetFloat("runningSpeed", 0.5f + getSpeed() / 2);

        if (Mathf.Abs(dx) <= 1.5) { chargeParticles.Stop(); charged = false; }

        //natural decrease of the player charge meter
        chargeDecrease();

        //Audio
        UpdateSound();

        //Checks if player hits a wall
        hitWall();

        //Music intensity
        AudioManager.instance.SetMusicIntensity((getSpeed() / 6 >= 1 ? 1 : getSpeed() / 6));

        //Boost direction leniancy
        if (charged) { chargeDirectionLeniancy(); }

        //Buffer a charge 
        chargeBufferTime();

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
        if (context.performed && (isGrounded() || hasCoyoteTime) && !isDead)
        {
            jump();
        }

        //buffer jump
        if (context.performed && !isGrounded() && canBuffer() && !isDead)
        {
            buffered = true;
        }

        //cancel jump
        if (context.canceled && rb.velocity.y > 0f && !isDead)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            animator.SetBool("isFalling", true);
            animator.SetBool("jumped", false);
        }
    }

    public void chargeInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead)
        {
            charge();
        } 
    }

    private void charge()
    {
        if (horizontal != 0 && chargeMeter > 0)
        {
            chargeParticles.Play();
            chargeExplosion.Play();
            charged = true;
            accelerationCap += 2;
            dx = accelerationCap * horizontal;
            chargeMeter -= chargeUsage;
            chargeLeniancyTimer = 5f;
            chargeBufferTimer = 0;
        }
        else
        {
            chargeBufferTimer = 4;
        }
    }

    private void chargeDirectionLeniancy()
    {
        if (chargeLeniancyTimer > 0)
        {
            if ((dx > 0 && !isFacingRight) || (dx < 0 && isFacingRight))
            {
                dx = -dx;
                chargeLeniancyTimer = 0;
            }
            chargeLeniancyTimer -= 1f;
        }
    }

    private void chargeBufferTime()
    {
        if (chargeBufferTimer > 0)
        {
            if (chargeMeter >= chargeUsage)
            {
                chargeBufferTimer = 0;
                charge();
            }
            chargeBufferTimer -= 1;
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
        if (other.gameObject.CompareTag("Concrete"))
        {
            acceleration = groundAcceleration;
            stopSpeed = groundDeceleration;
            playerFootsteps.setParameterByName("Ground Type", (float) GroundType.CONCRETE); 
        }
        if (other.gameObject.CompareTag("Wood"))
        {
            acceleration = groundAcceleration;
            stopSpeed = groundDeceleration;
            playerFootsteps.setParameterByName("Ground Type", (float) GroundType.WOOD);
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

    public void respawnCharacter()
    {
        transform.position = new Vector3(0, -1, 0);
        resetCharacter();
    }
    private void resetCharacter()
    {
        playerFootsteps.stop(STOP_MODE.IMMEDIATE);
        accelerationCap = 1;
        dx = 0;
        chargeMeter = 0;
    }

    IEnumerator collectibleRespawn (Collider2D collision, int time)
    {
        yield return new WaitForSeconds(time);

        collision.gameObject.SetActive(true);
    }

    public void batteryCollect(GameObject other)
    {
        chargeMeter += Mathf.Min(batteryCharge, chargeMeterCap - chargeMeter);
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
        if (chargeMeter > 0)
        {
            chargeMeter -= chargeDecreaseRate * Time.deltaTime;
        } else if (chargeMeter <= 0)
        {
            chargeMeter = 0;
        }
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
        RaycastHit2D leftHitHigh = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), (Vector2.left), .4f, layerMask);
        RaycastHit2D rightHitLow = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), (Vector2.right), .4f, layerMask);
        RaycastHit2D rightHitHigh = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), (Vector2.right), .4f, layerMask);

        if (leftHitLow || rightHitLow || leftHitHigh || rightHitHigh)
        {
            accelerationCap = 1;
            dx = 0;
            print("hit");
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
        ParticleSystemRenderer explosionRenderer = chargeExplosion.GetComponent<ParticleSystemRenderer>();
        explosionRenderer.flip = new Vector3((explosionRenderer.flip.x == 0) ? 1 : 0, 0, 0);
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void moveInput(InputAction.CallbackContext context)
    {
        if (!isDead) { horizontal = context.ReadValue<Vector2>().x; }
        else { horizontal = 0; }
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

    IEnumerator deathTransition()
    {
        isDead = true;

        resetCharacter();
        rb.velocity = new Vector2(0, 0);

        //death animation here
        animator.SetBool("isDead", true);

        //transition animation here...
        yield return new WaitForSeconds(0.9f);
            print("Timer 1 passed");

        if (checkpointSet)
        {
            rb.position = checkpoint;
        }

        animator.SetBool("isDead", false);

        isDead = false;

        yield return new WaitForSeconds(0.65f);
            print("Timer 2 passed");
        //respawn animation here
    }

    public void kill()
    {
        StartCoroutine(deathTransition());
    }

    public void setCheckpoint(InputAction.CallbackContext context)
    {
        checkpointSet = true;
        checkpoint = new Vector2(rb.position.x, rb.position.y);
        print("respawn set at (" + checkpoint.x + ", " + checkpoint.y + ")");
    }

    public void crouchInput(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded() && !isDead)
        {
            scaleDivisor = 2;
            //Debug.Log("crouching!");
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / scaleDivisor, transform.localScale.z);
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y / scaleDivisor);
        }
        //else{ fastfall ?}
        if (context.canceled && !isDead)
        {
            //Debug.Log("stopped crouching!");
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * scaleDivisor, transform.localScale.z);
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y * scaleDivisor);
            scaleDivisor = 1;
        }
    }

    private void UpdateSound()
    {
        PLAYBACK_STATE playbackState;
        playerFootsteps.getPlaybackState(out playbackState);

        if (dx == 0 || !isGrounded() && !isDead)
        {
            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        //playerFootsteps.setParameterByName("Ground Type", (float)GroundType.NOT_ON_GROUND);
        } else if (!isDead)
        {
            if (playbackState.Equals(PLAYBACK_STATE.STOPPING) || playbackState.Equals(PLAYBACK_STATE.STOPPED)) {
                playerFootsteps.start();
            }
        }
    }
}
