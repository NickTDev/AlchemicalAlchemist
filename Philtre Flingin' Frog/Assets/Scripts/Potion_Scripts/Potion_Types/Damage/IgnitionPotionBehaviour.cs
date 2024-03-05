using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnitionPotionBehaviour : StatusEffectPotionBehaviour
{
    public override void ContactEffect(GameObject other)
    {
        targetScript = other.GetComponent<PotionTarget>();
        if (targetScript != null)
        {
            targetScript.TakeDamage(damage);
            //If the target is flammable (has the FlammableScript component), sets on fire
            if (targetScript.GetComponent<FlammableScript>() != null)
            {
                ApplyStatusEffect(other);
            }
        }
    }
}
