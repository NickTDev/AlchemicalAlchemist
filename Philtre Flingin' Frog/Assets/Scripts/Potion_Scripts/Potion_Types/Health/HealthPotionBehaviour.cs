using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionBehaviour: PotionBehaviour
{
    [SerializeField] int healing;
    public override void ContactEffect(GameObject other)
    {
        targetScript = other.GetComponent<PotionTarget>();
        if (targetScript != null)
        {
            targetScript.AddHealth(healing);
        }
    }
}
