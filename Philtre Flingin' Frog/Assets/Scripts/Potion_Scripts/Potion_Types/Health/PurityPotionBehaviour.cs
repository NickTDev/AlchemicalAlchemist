using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurityPotionBehaviour : PotionBehaviour
{
    [SerializeField] int healing;
    protected GameObject target;
    public override void ContactEffect(GameObject other)
    {
        targetScript = other.GetComponent<PotionTarget>();
        if (targetScript != null)
        {
            targetScript.AddHealth(healing);
            //cycles through the target's status effects & removes all negative ones
            foreach (Transform child in transform)
            {
                if(child != null)
                {
                    StatusEffect statusEffectScript = child.GetComponent<StatusEffect>();
                    if (statusEffectScript != null && !statusEffectScript.GetIsGood())
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            //allows the invulnerability potion to apply a status effect + call this code without writing it twice
            target = other.gameObject;
            Additional();
        }
    }

}
