using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireStatusEffect : StatusEffect
{
    protected override void TickEffect() //deals damage every second
    {
        FlammableScript flameScript = targetScript.GetComponent<FlammableScript>();
        if(flameScript == null ) //Oopsy, guess the fire's out
        {
            Destroy(gameObject);
        }
        else //deals damage based on the target's flammability
        {
            int damage = flameScript.GetFlameDamage();
            targetScript.TakeDamage(damage);
        }
    }
}
