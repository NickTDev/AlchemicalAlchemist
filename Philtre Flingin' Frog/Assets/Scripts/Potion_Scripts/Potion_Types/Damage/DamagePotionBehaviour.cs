using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePotionBehaviour : PotionBehaviour
{
    public override void ContactEffect(GameObject other)
    {
        targetScript = other.GetComponent<PotionTarget>();
        if (targetScript != null)
        {
            targetScript.TakeDamage(damage);
        }
    }
}
