using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooStatusEffect : StatusEffect
{
    [SerializeField] float speedModification;
    protected override void ApplyEffect() //reduces speed
    {
        targetScript.ModifySpeedPercentage(speedModification);
    }
    protected override void RemoveEffect() //removes speed debuff
    {
        targetScript.ModifySpeedPercentage(speedModification * -1);
    }
}
