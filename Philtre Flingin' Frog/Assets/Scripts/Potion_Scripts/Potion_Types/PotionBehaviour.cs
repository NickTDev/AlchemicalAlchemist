using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBehaviour : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] GameObject[] statusEffectPrefabs;

    protected PotionTarget targetScript;
    public virtual void ContactEffect(GameObject other) { } //called from the 'enemy' script when it makes contact with the potion/the AOE

    protected virtual void Additional() { } //just there for when you need it

    protected void ApplyStatusEffect(GameObject other) //all potions can acess this script, StatusEffectBehaviourScript exclusively calls this one
    {
        for (int i = 0; i < statusEffectPrefabs.Length; i++)
        {
            StatusEffect statusEffectScript = statusEffectPrefabs[i].GetComponent<StatusEffect>();
            //checks whether the entity is protected by a purity effect (only if this status effect is bad)
            bool blocked = false;
            if(!statusEffectScript.GetIsGood())
            {
                foreach (Transform child in transform)//cycles through the already applied effects, checking if they are this one
                {
                    if (child.gameObject.GetComponent<Purity_Status_Effect>() != null)
                    {
                        blocked = true;
                        break;
                    }
                }
            }
            //if its a good effect or there's no purity, continues
            if(!blocked)
            {
                //if this effect cannot stack, it checks whether its already applied to the target; if so, resets the timer
                bool newObject = true;
                if (!statusEffectScript.GetCanStack())
                {
                    foreach (Transform child in other.transform)//cycles through the already applied effects, checking if they are the same type as this one
                    {
                        StatusEffect tempEffectScript = child.gameObject.GetComponent<StatusEffect>();
                        if (tempEffectScript != null && tempEffectScript.GetEffectName() == statusEffectScript.GetEffectName())
                        {
                            child.GetComponent<StatusEffect>().ResetTimer();
                            newObject = false;
                            break;
                        }
                    }
                }
                //if this effect CAN stack/there weren't any on the target, adds one to the target
                if (newObject)
                {
                    GameObject statusEffect = Instantiate(statusEffectPrefabs[i], other.transform.position, other.transform.rotation) as GameObject;
                    statusEffect.transform.SetParent(other.transform, true);
                    statusEffect.GetComponent<StatusEffect>().SpawnParticleEffect(gameObject);
                }
            }
        }
    }
}
