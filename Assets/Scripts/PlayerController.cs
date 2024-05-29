using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundcheck;
    public LayerMask groundLayer;
    private GameObject crushTrigger;
    public ParticleSystem particles;
    public ParticleSystem chargeParticles;
    public ParticleSystem chargeExplosion;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    public Animator animator;

    private float horizontal;
    private float speed = 7f;
    private float jumpingPower = 15f;
    private bool isFacingRight = true;
    private bool jumped = false;
    private bool buffered = false;
    private float stopSpeed = 0.2f;
    private bool charged = false;
    private int dyFastFallCap = 20;
    private int dyFallCap = 12;
    private int dyCap = 15;
    private int onSpeedBlock;

    private float footstepSoundTimer = 0;

    private bool inZoomArea = false;

    //the amount of charge given by batteries
    //private float batteryCharge = 2.5f;

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

    private float targetX = 0;
    private float targetY = 0;

    public float groundAcceleration = 0.1f;
    public float groundDeceleration = 0.2f;
    private float crouchStopMultiplier = 2f;
    private float iceAcceleration = 0.02f;
    private float iceDeceleration = 0.005f;

    private bool onMovingBlock = false;
    public bool isDead = false;

    private float dx = 0f;
    //private float dy = 0f;

    private int coyoteTimer = 0;
    private int coyoteLimit = 7;
    private bool hasCoyoteTime;

    private bool crouching = false;
    private bool fastFalling = false;
    private bool waitToUncrouch = false;
    private bool wallHit = false;

    private bool checkpointSet = false;
    private Vector2 checkpoint;

    public bool isPaused = false;


    //Audio
    private EventInstance playerFootsteps;

    private void Awake()
    {
        Application.targetFrameRate = 144;

        crushTrigger = GameObject.Find("Crush Trigger");
    }
    void Start()
    {
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
        if (SettingsManagement.instance != null && !SettingsManagement.instance.loadedFromContinue)
        {
            if (GameObject.Find("Start Pos"))
            {
                this.transform.position = GameObject.Find("Start Pos").GetComponent<Transform>().position;
            }
            resetCharacter();
        }
        else
        {
            if (SettingsManagement.instance.checkpoint != null)
            {
                checkpoint = SettingsManagement.instance.checkpoint;
                checkpointSet = true;
                SettingsManagement.instance.loadedFromContinue = false;
                rb.position = checkpoint;
            }
            resetCharacter();
        }
    }

    public float getSpeed ()
    {
        return Mathf.Abs(rb.velocity.x / speed); 
    }

    public float getCharge ()
    {
        return chargeMeter;
    }
    private void FixedUpdate()
    {

        canStand();

        //dx * speed applied to player velocity
        if (!isDead/* && (!isCrouched() || isCrouched() && !isGrounded())*/) { rb.velocity = new Vector2(dx * speed + onSpeedBlock + targetX, Mathf.Max(rb.velocity.y, -dyCap)); }
        else { rb.velocity = new Vector2(targetX, 0); }

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

        //Parent player to moving block if on one
        OnMovingBlock();

        //Moves camera back to default position
        cameraToDefault();

        if (isGrounded())
        {

            animator.SetBool("isFalling", false);
            animator.SetBool("isFastFalling", false);
            animator.SetBool("jumped", false);

            dyCap = dyFallCap;  

            coyoteTimer = 0;
            jumped = false;
            hasCoyoteTime = false;

            if (waitToUncrouch)
            {
                if (canStand())
                {
                    animator.SetBool("isCrouched", false);
                    crouching = false;
                    fastFalling = false;
                    waitToUncrouch = false;
                }
            }

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

                if(isCrouched())
                {
                    dyCap = dyFastFallCap;
                } else
                {
                    dyCap = dyFallCap;
                }
            } else
            {
                animator.SetBool("jumped", true);
                animator.SetBool("isFalling", false);
            }

            coyoteTime();

            airMovement();
        }

        if (accelerationCap > 1) { accelerationCap -= Mathf.Max(0.02f, accelerationCap - Mathf.Abs(dx)); } else { accelerationCap = 1; }
        if (Mathf.Abs(dx) > accelerationCap + targetX) { dx = accelerationCap * horizontal; }

        //CHANGED THIS /\ /\ /\ /\ /\

    }

    void Update()
    {

        if (!isFacingRight && horizontal > 0 || isFacingRight && horizontal < 0 && !isPaused)
        {
            flip();
        }
        if (Mathf.Abs(dx) < 0.05f || horizontal == 0 || wallHit)
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

        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Player Jump");

        if (!isCrouched())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower / 1.25f);
        }

        jumped = true;
        buffered = false;

        if (canStand())
        {
            animator.SetBool("isCrouched", false);
            animator.SetBool("jumped", true);
            animator.SetBool("isFalling", false);
        } 
    }
    public void jumpInput(InputAction.CallbackContext context)
    {
        //normal jump
        if (context.performed && (isGrounded() || hasCoyoteTime) && !isDead && !isPaused)
        {
            jump();
        }

        //buffer jump
        if (context.performed && !isGrounded() && canBuffer() && !isDead && !isPaused)
        {
            buffered = true;
        }

        //cancel jump
        if (context.canceled && rb.velocity.y > 0f && !isDead && !isPaused)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            animator.SetBool("isFalling", true);
            animator.SetBool("jumped", false);
        }
    }

    public void chargeInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead && !isPaused)
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
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Player Boost");
            charged = true;
            if (isCrouched())
            {
                accelerationCap += 0.75f;
            } 
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

    public void speedBlockBoost (int direction)
    {
        accelerationCap += 2;
        dx = accelerationCap * ((direction == 0) ? 1 : -1);
    }

    public void speedBlockIdle(int direction, bool isOn)
    {
        onSpeedBlock = (isOn ? (direction == 0 ? 1 : -1) : 0);
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
        if (coyoteTimer < coyoteLimit && !jumped && !isPaused) { coyoteTimer++; }
        hasCoyoteTime = coyoteTimer < coyoteLimit && !jumped;
    }
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.35f, groundLayer) && Mathf.Abs(rb.velocity.y) <= 0.01f;
    }

    private bool canStand()
    {
        int playerLayer = 9;
        int layerMask = ~(1 << playerLayer);

        RaycastHit2D topHitLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.25f, transform.position.y - 0.2f), (Vector2.up), 0.9f, layerMask);
        RaycastHit2D topHitRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.25f, transform.position.y - 0.2f), (Vector2.up), 0.9f, layerMask);

        if(topHitLeft || topHitRight)
        {
            //animator.SetBool("isCrouched", true);
        } else if (!crouching && !(topHitLeft || topHitRight))
        {
            //animator.SetBool("isCrouched", false);
        }

        return !(topHitLeft || topHitRight);
    }

    public bool isCrouched ()
    {
        return animator.GetBool("isCrouched");
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
        if (other.gameObject.CompareTag("Metal"))
        {
            acceleration = groundAcceleration;
            stopSpeed = groundDeceleration;
            playerFootsteps.setParameterByName("Ground Type", (float)GroundType.METAL);
        }
        if (other.gameObject.CompareTag("Ice"))
        {
            acceleration = iceAcceleration;
            stopSpeed = iceDeceleration;
        }
        if (other.gameObject.CompareTag("SpeedBoost"))
        {
            acceleration = groundAcceleration;
            stopSpeed = groundDeceleration;
            Debug.Log("on Speed Boost");
        }
        if (other.gameObject.CompareTag("MovingBlock"))
        {
            acceleration = groundAcceleration;
            stopSpeed = groundDeceleration;
            Debug.Log("on Moving Block");
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("SpeedBoost"))
        {
            Debug.Log("off Speed Boost");
        }
    }

    private void OnMovingBlock()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundcheck.position, 0.3f, groundLayer);
        if (isGrounded())
        {
            if (groundCollider != null && groundCollider.CompareTag("MovingBlock"))
            {
                onMovingBlock = true;
            }
            else
            {
                onMovingBlock = false;
            }
        }
        else if (animator.GetBool("isFalling"))
        {
            onMovingBlock = false;
        }
        if (animator.GetBool("jumped"))
        {
            onMovingBlock = false;
        }

        if (onMovingBlock == true && groundCollider != null)
        {
            targetX = groundCollider.GetComponent<Rigidbody2D>().velocity.x;
            targetY = groundCollider.GetComponent<Rigidbody2D>().velocity.y;
        }
        else
        {
            targetX = 0;
        }
    }

    //for collectibles 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            batteryCollect(other.gameObject);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Collectibles/Battery/Battery Collect");
            if (other.GetComponent<collectibleBehavior>().getState().Equals("Respawns")) { StartCoroutine(collectibleRespawn(other, 5)); }
        }

        if (other.gameObject.CompareTag("SpeedBoost"))
        {
            speedBoostCollect(other.gameObject);
            if (other.GetComponent<collectibleBehavior>().getState().Equals("Respawns")) { StartCoroutine(collectibleRespawn(other, 5)); }
        }

        if (other.gameObject.CompareTag("Fade Music"))
        {
            GameObject.Find("FMODEvents").GetComponent<Timer>().fadeOutMusic(3f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Zoom In"))
        {
            inZoomArea = false; 
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Zoom In"))
        {
            inZoomArea = true;
            var fov = virtualCamera.m_Lens.FieldOfView;
            int target = 75;
            Vector2 pos = Vector2.MoveTowards(new Vector2(fov, 0), new Vector2(target, 0), 0.5f);
            virtualCamera.m_Lens.FieldOfView = pos.x;
        }
    }

    private void cameraToDefault()
    {
        if (!inZoomArea && !virtualCamera.m_Lens.Orthographic)
        {
            var fov = virtualCamera.m_Lens.FieldOfView;
            int max = 100;
            Vector2 pos = Vector2.MoveTowards(new Vector2(fov, 0), new Vector2(max, 0), 0.5f);
            virtualCamera.m_Lens.FieldOfView = pos.x;
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
        playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
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
        if (other.GetComponent<CircleCollider2D>().enabled == true)
        {
            chargeMeter += Mathf.Min(other.GetComponent<batteryBehavior>().BoostAmount(), chargeMeterCap - chargeMeter);
            other.GetComponent<CircleCollider2D>().enabled = false;
            other.GetComponent<batteryBehavior>().isCollected();
            StartCoroutine(batteryCollectAnim(0.4f, other));
        }
    }

    IEnumerator batteryCollectAnim(float time, GameObject other)
    {
        yield return new WaitForSeconds(time);

        other.SetActive(false);
        other.GetComponent<CircleCollider2D>().enabled = true;
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

    private void hitWall()
    {
        int playerLayer = 9;
        int layerMask = ~(1 << playerLayer);

        RaycastHit2D leftHitLow = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), (Vector2.left), .4f, layerMask);
        RaycastHit2D leftHitHigh = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + ((isGrounded() && isCrouched()) ? -0.2f : 0.5f)), (Vector2.left), .4f, layerMask);
        RaycastHit2D rightHitLow = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), (Vector2.right), .4f, layerMask);
        RaycastHit2D rightHitHigh = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + ((isGrounded() && isCrouched()) ? -0.2f : 0.5f)), (Vector2.right), .4f, layerMask);

        if (leftHitLow || rightHitLow || leftHitHigh || rightHitHigh)
        {
            accelerationCap = 1;
            wallHit = true;
            dx = 0;
        } else
        {
            wallHit = false;
        }
    }

    private void groundMovement()
    {
        if (!isCrouched())
        {
            if (horizontal != 0)
            {
                dx += acceleration * horizontal;
            }
            else if (horizontal == 0 && Mathf.Abs(dx - targetX) > 0)
            {
                dx -= Mathf.Min(stopSpeed, Mathf.Abs(dx)) * Mathf.Sign(dx);
            }
        } else
        {
            if (Mathf.Abs(dx - targetX) > 0)
            {
                dx -= Mathf.Min(stopSpeed * crouchStopMultiplier, Mathf.Abs(dx)) * Mathf.Sign(dx);
            }
        }
    }
    private void airMovement()
    {
        if (horizontal != 0)
        {
            dx += airAcceleration * horizontal;
        }
        else if (horizontal == 0 && Mathf.Abs(dx - targetX) > 0)
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
        if (!isDead) { horizontal = Mathf.Round(context.ReadValue<Vector2>().x); }
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

    public void setDead ()
    {
        isDead = true;
    }
    public void setAlive ()
    {
        isDead = false;
    }

    IEnumerator deathTransition()
    {
        setDead();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Player Killed");

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
        setAlive();

        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Player Respawn");

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
        changeCheckpoint(new Vector2(rb.position.x, rb.position.y));
    }

    public void changeCheckpoint(Vector2 pos)
    {
        checkpointSet = true;
        checkpoint = pos;
        print("respawn set at (" + checkpoint.x + ", " + checkpoint.y + ")");
    }

    public void crouchInput(InputAction.CallbackContext context)
    {
        if (context.started && !isDead)
        {
            animator.SetBool("isCrouched", true);

            crouching = true;

            if (!isGrounded())
            {
                fastFalling = true;
            }
        }
        if (context.canceled && !isDead)
        {
            if (!canStand())
            {
                waitToUncrouch = true;
            } else if (isGrounded() && canStand())
            {
                animator.SetBool("isCrouched", false);
                crouching = false;
                fastFalling = false;
            }
            if (!isGrounded())
            {
                animator.SetBool("isCrouched", false);
                fastFalling = false;
            }
        }
    }

    private void UpdateSound()
    {
        PLAYBACK_STATE playbackState;
        playerFootsteps.getPlaybackState(out playbackState);

        if (Mathf.Abs(dx) <= 0.1f || !isGrounded() || isDead || isPaused || isCrouched())
        {
            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
            footstepSoundTimer = 0;
        //playerFootsteps.setParameterByName("Ground Type", (float)GroundType.NOT_ON_GROUND);
        } else 
        {
            footstepSoundTimer += Time.deltaTime;
            if (footstepSoundTimer >= 0.2f && (playbackState.Equals(PLAYBACK_STATE.STOPPING) || playbackState.Equals(PLAYBACK_STATE.STOPPED))) {
                playerFootsteps.start();
            }
        }
    }
}
