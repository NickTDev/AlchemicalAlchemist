using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectPotionBehaviour : PotionBehaviour
{
    List<GameObject> hasDamaged = new List<GameObject>(); //wich entities this potion hsa already damage
    public override void ContactEffect(GameObject other)
    {
        ApplyStatusEffect(other);
        if(!hasDamaged.Contains(other)) //only applies damage once per entity
        {
           other.GetComponent<PotionTarget>().TakeDamage(damage);
            hasDamaged.Add(other);
        }
    }
}
