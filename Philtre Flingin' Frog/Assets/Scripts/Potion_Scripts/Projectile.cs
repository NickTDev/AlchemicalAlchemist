using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
public class Projectile : EffectCauser
{
    Rigidbody rb;
    public LayerMask whatIsGround;
    public LayerMask target;
    public float objectHeight;
    public int damage;
    GameObject[] deliveryType = new GameObject[1];
    [SerializeField] GameObject particleEffect;
    [SerializeField] Gradient particleColorRange;
    public GameObject player;
    PlayerMovement move;
    AttackScript attack;
    float speedBoost;
    bool leftClick;
    float timeHeldDown;
    public float maxHeldDown;
    public float minHeldDown;
    //public float maxDistance; //never used
    public LayerMask collidableLayer;
    public float speed;
    public float yForce;
    public float timeMultiplyer;
    Vector3 speedOnPause;
    bool wasPaused;
    public float gravity;
    bool hasExploded = false;
    float startTime;

    float lifeSpan = 0f; //howlong this projectile has been around

    void Awake()
    {
        player = GameManager.GetPlayer();
        move = player.GetComponent<PlayerMovement>();
        attack = player.GetComponent<AttackScript>();
    }
    // Start is called before the first frame update
    protected override void StartupInit()
    {
        startTime = Time.time;
        wasPaused = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        leftClick = attack.getLeftClick();
        timeHeldDown = attack.getTimeClicked();
        if (leftClick)
        {
            chargeThrow();
        }
        else
        {
            dropPotion();
        }
        SetColor();
    }
    void Update()
    {
        if(!GameManager.GetPaused())
        {
            lifeSpan += Time.deltaTime;
            if(lifeSpan > 2f)
            {
                Burst();
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ManagePause();
    }
    //Function that handles movement relative to pause state
    void ManagePause()
    {
        if (!GameManager.GetPaused())
        {
            if (wasPaused)
            {
                wasPaused = false;
                rb.AddForce(speedOnPause, ForceMode.Force);
            }
            Vector3 moveDirection = Vector3.zero;
            moveDirection.y = gravity * Time.deltaTime;
            rb.AddForce(moveDirection, ForceMode.Force);
        }
        else
        {
            if (!wasPaused)
            {
                speedOnPause = rb.velocity;
                wasPaused = true;
            }
            rb.velocity = Vector3.zero;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(!hasExploded)
        { 
            Burst(collision);
        }
        else
        {
            Burst();
        }
    }
    //Function that determines initial force relative to how long the player charged their throw
    void chargeThrow()
    {
        if (timeHeldDown > maxHeldDown)
        {
            timeHeldDown = maxHeldDown;
        }
        if (timeHeldDown < minHeldDown)
        {
            timeHeldDown = minHeldDown;
        }
        yForce *= timeHeldDown / maxHeldDown;
        timeHeldDown *= timeMultiplyer;
        Vector3 moveDirection;
        moveDirection = transform.forward * speed * (timeHeldDown / maxHeldDown);
        moveDirection.y = yForce;
        rb.AddForce(moveDirection, ForceMode.Impulse);
    }
    void dropPotion()
    {
        speedBoost = move.getSpeed();
        if (speedBoost < 1)
        {
            speedBoost = 1;
        }
        Vector3 moveDirection = transform.forward * speedBoost * 1.5f;
        rb.AddForce(moveDirection, ForceMode.Impulse);
    }
    public void SetPotionType(GameObject potion, GameObject delivery)
    {
        potionType = potion;
        potionType.GetComponent<InventoryItemPotion>().enabled = false; //don't want the UI script doing stuff
        potionType.transform.SetParent(gameObject.transform, true);
        int numDeliverys = delivery.GetComponent<DeliveryMethod>().GetNumSpawned();
        deliveryType = new GameObject[numDeliverys];
        deliveryType[0] = delivery;
        deliveryType[0].transform.SetParent(gameObject.transform, true);
        for (int i = 1; i < numDeliverys; i++)
        {
            deliveryType[i] = Instantiate(delivery, delivery.transform.position, delivery.transform.rotation);
            deliveryType[i].transform.SetParent(gameObject.transform, true);
        }
    }
    private void Burst(Collision collision = null)
    {
        hasExploded = true;
        for (int i = 0; i < deliveryType.Length; i++)
        {
            if (deliveryType[i] != null)
            {
                deliveryType[i].GetComponent<DeliveryMethod>().Activate(potionType, i);
            }
        }
        if (collision != null) //automatically triggers the effect on whatever it hit
        {
            PotionTarget target = collision.gameObject.GetComponent<PotionTarget>();
            if (target != null)
            {
                target.ContactEffect(this);
            }
        }
        SpawnParticles();
        Destroy(gameObject);
    }
    protected override void SetColor() //sets the color based on the potion child's color
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = potionType.GetComponent<InventoryItemPotion>().GetPotionColor();
    }
    private void SpawnParticles()
    {
        GameObject particles = Instantiate(particleEffect, transform.position, Quaternion.identity);
        particles.GetComponent<ParticleEffectScript>().Initialize(potionType.GetComponent<InventoryItemPotion>().GetPotionColor(), true);
    }


    protected override void PlayerDeath(bool yes = true) //should despawn on player respawn
    {
        if(!hasExploded)
        {
            Destroy(gameObject);
        }
    }
}