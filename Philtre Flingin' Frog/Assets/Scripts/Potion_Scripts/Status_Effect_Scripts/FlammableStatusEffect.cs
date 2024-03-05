using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableStatusEffect : StatusEffect
{
    [SerializeField] int flameDamage = 10;
    FlammableScript flameScript;
    //only used if the target was already flammable beforehand
    bool alreadyFlammable = false;
    FlammableScript existingFlameScript;

    protected override void ApplyEffect() //applies the flammable script
    {
        existingFlameScript = targetScript.GetComponent<FlammableScript>();
        if (existingFlameScript != null)
        {
            alreadyFlammable = true;
            //adds potion's flame damage to base flame damage
            existingFlameScript.AddFlameDamage(flameDamage);
        }
        else
        {
            flameScript = targetScript.gameObject.AddComponent<FlammableScript>() as FlammableScript;
            flameScript.SetFlameDamage(flameDamage);
        }
    }
    protected override void RemoveEffect() //removes the flammable script
    {
        if(alreadyFlammable) //subtracts the flame damage we added
        {
            existingFlameScript.AddFlameDamage(flameDamage * -1);
        }
        else
        {
            Destroy(flameScript);
        }
    }
}
