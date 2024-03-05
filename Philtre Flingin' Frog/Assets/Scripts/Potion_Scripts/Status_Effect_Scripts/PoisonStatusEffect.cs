using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatusEffect : StatusEffect
{
    [SerializeField] int damage;
    protected override void TickEffect() //deals damage every second
    {
        targetScript.TakeDamage(damage);
    }
}
