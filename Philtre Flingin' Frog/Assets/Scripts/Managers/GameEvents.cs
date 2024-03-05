using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    //Event for when the player respawns
    public event Action<bool> onPlayerRespawn;
    public void playerRespawn(bool yes)
    {
        if (onPlayerRespawn != null)
        {
            onPlayerRespawn(yes);
        }
    }
    //Event for when an enemy changes from idle to aggro
    public event Action<Vector3> onBecameAggro;
    public void becameAggro(Vector3 pos)
    {
        if (onBecameAggro != null)
        {
            onBecameAggro(pos);
        }
    }

    //Event for when a swarmer enemy deaggros
    public event Action<Vector3> onBecameUnAggro;
    public void becameUnAggro(Vector3 pos)
    {
        if (onBecameUnAggro != null)
        {
            onBecameUnAggro(pos);
        }
    }

    //Event for when an enemy gets hit with a potion
    public event Action<Vector3> onHitByPotion;

    public void hitByPotion(Vector3 pos)
    {
        if (onHitByPotion != null)
        {
            onHitByPotion(pos);
        }
    }

    //Event for when the player dies
    public event Action<Vector3> onPlayerDeath;
    public void playerDies(Vector3 pos)
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath(pos);
        }
    }

    //event for when enemy is killed
    public event Action<Type> onEnemyKill;
    public void enemyDies(Type enemyType)
    {
        if (onEnemyKill != null)
        {
            onEnemyKill(enemyType);
        }
    }

    //event for when ingredient is collected
    public event Action<Collectible> onCollect;
    public void itemCollected(Collectible collected)
    {
        if (onCollect != null)
        {
            onCollect(collected);
        }
    }

    //event for when dialogue ends
    public event Action<DialogueTrigger> onConvoEnd;
    public void speakerEnds(DialogueTrigger spoke)
    {
        if (onConvoEnd != null)
        {
            onConvoEnd(spoke);
        }
    }

}
