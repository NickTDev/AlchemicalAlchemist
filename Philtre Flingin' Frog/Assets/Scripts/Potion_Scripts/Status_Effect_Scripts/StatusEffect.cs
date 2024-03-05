using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] GameObject particlePrefab; //the particle effect that will follow this effect
    ParticleEffectScript particleScript;
    [SerializeField] bool isGood; //whether this is a beneficial effect
    [SerializeField] string effectName; //name of the effect; used to prevent duplicates
    [SerializeField] bool canStack; //whether this effect can apply multiple times. If not, additional instances reset the timer
    float secondsForTick = 1.0f; //how long between applications of ticking status effects
    [SerializeField] float durationSeconds = 10; //how long the effect lasts
    float nextTick; //when the next application will occur

    float elapsedTime = 0; //how long since the effect began USED FOR TICK APPLICATION
    float elapsedDuration = 0; //how long since the effect began/was reset USED FOR DECAY SELF DESTRUCT


    protected GameObject parent; //the enemy this status effect is applied to
    protected PotionTarget targetScript;

    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        nextTick = secondsForTick;
        targetScript = parent.GetComponent<PotionTarget>();
        ApplyEffect();
        GameEvents.current.onPlayerRespawn += PlayerRespawn;
    }

    public void SpawnParticleEffect(GameObject potionPrefab)
    {
        GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity) as GameObject;
        particleScript = particles.GetComponent<ParticleEffectScript>();
        particleScript.Initialize(potionPrefab.GetComponent<InventoryItemPotion>().GetPotionColor(),false);
        particleScript.transform.SetParent(transform, true);
    }

    void Update()
    {
        Timer();
    }

    void Timer() //keeps track of how long the effect has lasted, ends when no longer applicable
    {
        if(!GameManager.GetPaused()) //does not happen while paused
        {
            elapsedTime += 1 * Time.deltaTime;
            elapsedDuration += 1 * Time.deltaTime;
            if (elapsedDuration > durationSeconds)
            {
                RemoveEffect();
                Destroy(gameObject);
            }
            if (elapsedTime >= nextTick) //if it's time, applys the effect
            {
                nextTick += secondsForTick;
                TickEffect();
            }
        }
    }

    public void ResetTimer() //restarts the countdown
    {
        elapsedDuration = 0;
    }

    public bool GetCanStack()
    {
        return canStack;
    }
    public bool GetIsGood()
    {
        return isGood;
    }
    public string GetEffectName()
    {
        return effectName;
    }

    public void PlayerRespawn(bool yes = true)
    {
        Destroy(gameObject);
    }

    protected virtual void TickEffect() { } //causes any 'ticking' effects, such as damage
    protected virtual void ApplyEffect() { } //applys any constant effects, such as speed debuff
    protected virtual void RemoveEffect() { } //removes any constant effects, such as speed debuff
}
