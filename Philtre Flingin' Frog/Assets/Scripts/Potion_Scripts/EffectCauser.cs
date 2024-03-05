using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCauser : MonoBehaviour
{
    protected GameObject potionType;

    void Start()
    {
        StartupInit();
        GameEvents.current.onPlayerRespawn += PlayerDeath;
    }

    protected virtual void StartupInit() { } //so that this can set up the event in start

    protected virtual void PlayerDeath(bool yes = true) //should despawn on player respawn
    {
    }

    public PotionBehaviour GetPotionTypeScript()
    {
        return potionType.GetComponent<PotionBehaviour>();
    }
    protected virtual void SetColor() { } //sets the color based on the potion child
}
