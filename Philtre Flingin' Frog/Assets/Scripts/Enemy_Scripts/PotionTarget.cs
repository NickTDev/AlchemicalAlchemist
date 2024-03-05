using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PotionTarget : MonoBehaviour
{
    [SerializeField] public float baseMoveSpeed;
    public float currentMoveSpeed;
    EffectCauser hitBy;
    [SerializeField] protected int baseHealth;
    protected int health;// = 1; //must start positive or else player insta dies
    protected float minMoveSpeed = 0;
    [SerializeField] protected Image healthBar;
    //AOE potions only fire their effects once every second
    float potionPulseTime = 1; //should be the same for all entities, don't serialize
    float timeSinceLastPulse = 1;
    [SerializeField] bool colorHealthBar = true; //whether to change the color of the health bar as you lose health. yes for enemies, no for player (unique health bar)

    protected void InitHealthBar()
    {
        health = baseHealth; //first sets health
        AddHealth(0); //ensures the helath bar respawns at full
    }
    /*
    void OnCollisionEnter(Collision collision)//projectile/AOE triggers the potion effect on initial contact
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Potions"))
        {
            hitBy = collision.gameObject.GetComponent(typeof(EffectCauser)) as EffectCauser;
            ContactEffect(hitBy);
        }
    }
    */
    void OnTriggerEnter(Collider collision)//AOE triggers the potion effect on a 1 second pulse
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Potions"))
        {
            hitBy = collision.gameObject.GetComponent(typeof(EffectCauser)) as EffectCauser;
            ContactEffect(hitBy);
        }
    }
    void OnTriggerStay(Collider collision)//AOE triggers the potion effect on a 1 second pulse
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Potions"))
        {
            hitBy = collision.gameObject.GetComponent(typeof(EffectCauser)) as EffectCauser;
            ContactEffect(hitBy);
        }
    }
    public void ContactEffect(EffectCauser effectCause)
    {
        if (timeSinceLastPulse > potionPulseTime)
        {
            timeSinceLastPulse = 0.0f;
            PotionBehaviour effect = effectCause.GetPotionTypeScript();
            effect.ContactEffect(gameObject);
        }
    }
    public void AddHealth(int amount)
    {
        health += amount;
        healthBar.fillAmount = (float)health / baseHealth;
        if(colorHealthBar) //enemy bars get redder as they die, player has unique sprite so no
        {
            Color healthColor = healthBar.color;
            healthColor.g = (float)health / baseHealth;
            healthColor.r = 1 - (float)health / baseHealth;
            healthBar.color = healthColor;
        }
    }
    public void TakeDamage(int amount)
    {
        AddHealth(amount * -1);
        healthBar.fillAmount = (float)health / baseHealth;
        GameEvents.current.hitByPotion(gameObject.transform.position);
    }
    public void ModifySpeedPercentage(float percent, bool capAtZero = true) //increases/decreases speed by the specified percentage of base speed
    {
        currentMoveSpeed += baseMoveSpeed * percent; //modifies it
        if (capAtZero && currentMoveSpeed < 0)
        {
            currentMoveSpeed = 0;
        }
    }
    protected void PotionPulseTimer()
    {
        /*
        if(timeSinceLastPulse > potionPulseTime)
        {
            timeSinceLastPulse = 0.0f;
        }
        Debug.Log("" + timeSinceLastPulse);
        */
        timeSinceLastPulse += Time.deltaTime; //important to increment after check so that onCollisionStay has a frame where elapsed time is greater than max
    }
}