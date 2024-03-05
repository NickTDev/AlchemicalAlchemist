using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeliveryMethod : EffectCauser
{
    //Most of the stuff in this parent is only used in some of the deliver methods, but theres neough overlap to only write it once
    [SerializeField] bool AOE; //whether this delivery type has an AOE
    [SerializeField] float duration; //how long the effect lasts
    [SerializeField] float scale; //how large the AOE is once finished growing
    [SerializeField] float growthSpeed; //how much the AOE grows each second
    [SerializeField] float transparency; //how transparent the AOE is
    [SerializeField] FMODUnity.EventReference breakSound;
    [SerializeField] bool useGround = true; //whether this deliveyr should stick to the ground (false for cling)
    bool onGround = false;
    bool underGround = false;
    float verticalSpeed; //how fast the delvieyr method climbs & falls to stick on the ground
    bool active = false;
    [SerializeField] int numSpawned = 1; //how many of the delivery method get spawned
    protected int orderSpawned; //the order this was spawned compared to others from the same bottle

    protected override void StartupInit()
    {
        transform.localScale = new Vector3(0, 0, 0); //the AOE should take up no size until potion bursts
        Color transparentColor = GetComponent<Renderer>().material.color;
        transparentColor.a = 0.1f;
        GetComponent<Renderer>().material.SetColor("_Color", transparentColor);
    }
    public virtual void Activate(GameObject potion, int order = 0) //called from Projectile when it bursts
    {
        if (AOE) //creates AOE, if exists
        {
            transform.SetParent(null); //de-coubples the AOE from the projectile
            potionType = potion;
            potionType.transform.SetParent(gameObject.transform, true); //transfers the primary potion object to the AOE cloud
            SetColor();
        }
        orderSpawned = order;
        UniqueStart();
        active = true;
        //sound
        FMOD.Studio.EventInstance aInstance = AudioEngineManager.CreateSound(breakSound, 1f, gameObject.transform.position);
        aInstance.start();
    }
    protected virtual void Update()
    {
        if (active)
        {
            DurationTimer();
            Grow();
            UniqueUpdate();
            if(useGround)
            {
                StayOnGround();
            }
        }
    }
    protected virtual void DurationTimer()//will count down to self destruction
    {
        if (!GameManager.GetPaused()) //does not happen while paused
        {
            duration -= Time.deltaTime;
            if (duration < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    protected virtual void Grow() //will increase scale until fully grown
    {
        if (!GameManager.GetPaused()) //does not happen while paused
        {
            float currentScale = transform.localScale.x;
            if (currentScale < scale)
            {
                currentScale += growthSpeed * Time.deltaTime;
                currentScale = Mathf.Clamp(currentScale, 0.0f, scale);
                transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }
        }
    }
    protected override void SetColor()
    {
        Color newColor = potionType.GetComponent<InventoryItemPotion>().GetPotionColor();
        newColor.a = transparency;
        GetComponent<Renderer>().material.color = newColor;
    }
    public int GetNumSpawned()
    {
        return numSpawned;
    }
    protected override void PlayerDeath(bool yes = true) //should despawn on player respawn
    {
        if(active)
        {
            duration = -1;
        }
    }

    public void SetVerticalSpeed(float vertSpeed)
    {
        verticalSpeed = vertSpeed;
    }

    public void SetOnGround(bool grounded)
    {
        onGround = grounded;
    }
    public void SetUnderGround(bool under)
    {
        underGround = under;
    }
    void StayOnGround()
    {
        //Debug.Log("on: " + onGround + "  under: " + underGround);
        if(!GameManager.GetPaused())
        {
            if(underGround)
            {
                float yPos = transform.position.y + verticalSpeed;
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }
            else if (!onGround)
            {
                float yPos = transform.position.y - verticalSpeed;
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }
        }
    }

    protected virtual void UniqueStart() { } //for any unique actions delivery methods take on start
    protected virtual void UniqueUpdate() { } //for any unique actions delivery methods take on update
}