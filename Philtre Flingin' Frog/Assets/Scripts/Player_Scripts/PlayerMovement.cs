using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerMovement : PotionTarget
{
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplyer;
    bool readyToJump = true;

    public KeyCode jumpKey = KeyCode.Space;

    public float groundDrag;
    bool onGround;
    public float gravity;
    float startGravity;
    float fallingGravity;
    public int layerNumber;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    static float speed;

    Vector3 moveDirection;
    Rigidbody rb;
    Enemy hit;

    Vector3 speedOnPause;
    bool wasPaused;
    RaycastHit slope;

    GameManager gm;
    [SerializeField] FMODUnity.EventReference DamageSFX;
    [SerializeField] Checkpoint initialCheckpoint;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentMoveSpeed = baseMoveSpeed;
        InitHealthBar();
        wasPaused = false;
        startGravity = gravity;
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        if(initialCheckpoint != null) //so that the player will respawn if they die before the first checkpoint
        {
            initialCheckpoint.SetAsCheckpoint(gameObject);
        }
    }

    private void getInput()
    {
        if (!GameManager.GetPaused())
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(jumpKey) && readyToJump && onGround)
            {
                //UnityEngine.Debug.Log(readyToJump);
                readyToJump = false;
                Jump();

            }
        }
    }

    void FixedUpdate()
    {
        getInput();
        MovePlayer();
    }

    void Update()
    {
        SpeedControl();

        if (onGround)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        
        if (rb.velocity.y <= 0)
        {
            gravity = startGravity;
        }

        if (health <= 0)
        {
            gm.ClearEnemies();
            GameEvents.current.playerDies(transform.position);
            Destroy(gameObject);
            gm.Respawn();
        }
        PotionPulseTimer();
    }


    //Function that handles player movement and behaviour while paused
    private void MovePlayer()
    {
        float yVelo = 0.0f;
        Vector3 forceToAdd = Vector3.zero;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (!onGround)
        {
            yVelo = rb.velocity.y;
            yVelo -= gravity;
        }
        moveDirection.y = rb.velocity.y;
        //Behaviour while unpaused
        if (!GameManager.GetPaused())
        { 
            //Behaviour if just became unpaused
            if (wasPaused)
            {
                rb.useGravity = true;
                wasPaused = false;
                rb.velocity = speedOnPause;
            }
            if (!onSlope() && onGround)
            {
                moveDirection = moveDirection.normalized;
                forceToAdd = moveDirection * currentMoveSpeed;
            }
            else if (onSlope() && onGround)
            {
                forceToAdd = getSlopeAngle() * currentMoveSpeed;
            }
            else if (!onGround)
            {
                float xVelo = moveDirection.x * currentMoveSpeed * airMultiplyer;
                float zVelo = moveDirection.z * currentMoveSpeed * airMultiplyer;
                forceToAdd = new Vector3(xVelo, yVelo, zVelo);
            }
            if(forceToAdd.magnitude > 0.0f)
                rb.AddForce(forceToAdd, ForceMode.Force);
        }
        else
        {
            //Behaviour when first paused
            if(!wasPaused)
            {
                rb.useGravity = false;
                speedOnPause = rb.velocity;
                rb.velocity = Vector3.zero;
                wasPaused = true;
            }
        }
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    //Function that prevents the player from going over a set max speed
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (flatVel.magnitude > currentMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x,rb.velocity.y,limitedVel.z);
        }
        speed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y + rb.velocity.z * rb.velocity.z);
    }

    //Function that handles jumping
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        onGround = false;
        readyToJump = false;
    }

    //Function that activates when the player is able to jump again
    private void ResetJump()
    {
        readyToJump = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.GetComponent(typeof(Enemy))) != null)
        {
            hit = collision.gameObject.GetComponent(typeof(Enemy)) as Enemy;
            TakeDamage(hit.damage);
        }
        //Detects if the player is on the ground and allows them to jump if they are
        if(collision.gameObject.layer == layerNumber)
        {
            onGround = true;
            gravity = startGravity;
            Invoke("ResetJump", jumpCooldown);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == layerNumber)
        {
            onGround = false;
        }
    }

    public float getSpeed()
    {
        return speed;
    }

    bool onSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down, out slope,1.0f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slope.normal);
            return angle != 0;
        }
        return false;
    }

    Vector3 getSlopeAngle()
    {
        if(horizontalInput != 0 || verticalInput != 0)
            return Vector3.ProjectOnPlane(moveDirection, slope.normal).normalized;
        return Vector3.zero;
    }

    public new void TakeDamage(int amount)
    {
        AudioEngineManager.PlaySound(DamageSFX, 1f, this.gameObject);
        AddHealth(amount * -1);
        healthBar.fillAmount = (float)health / baseHealth;
        GameEvents.current.hitByPotion(gameObject.transform.position);
    }
}

