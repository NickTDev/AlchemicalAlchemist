using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : PotionTarget
{
    public int damage;
    private NavMeshAgent navMeshAgent;
    [SerializeField] GameObject[] drops; //the potential collectibles this enemy can drop
    [SerializeField] float[] dropChances; //the chance that the enemy will drop each item 
    [SerializeField] float dropRadius; //the distance in which they can drop them
    [SerializeField] FMODUnity.EventReference DeathSFX;

    void Start()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = baseMoveSpeed;
        currentMoveSpeed = baseMoveSpeed;
        InitHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
        UpdateSpeed();
        PotionPulseTimer();
    }


    void UpdateSpeed() //keeps actual speed in line with intended
    {
        if (currentMoveSpeed > minMoveSpeed)
        {
            navMeshAgent.speed = currentMoveSpeed;
        }
        else
        {
            navMeshAgent.speed = minMoveSpeed;
        }
    }

    void Die()
    {
        AudioEngineManager.PlaySound(DeathSFX, 1.0f, gameObject.transform.position);
        DropLoot();
        Destroy(gameObject);
    }

    void DropLoot() //random chance of dropping each loot item
    {
        for (int i = 0; i < drops.Length; i++)
        {
            float rand = Random.Range(0f, 1f);
            if (dropChances.Length <= i || rand <= dropChances[i]) //random chance of dropping; first check ensures droppage if the drop chance list is too short
            {
                float xPos = transform.position.x + Random.Range(-dropRadius, dropRadius);
                float yPos = transform.position.y + 1;
                float zPos = transform.position.z + Random.Range(-dropRadius, dropRadius);
                Vector3 droppedPos = new Vector3(xPos, yPos, zPos);
                GameObject dropped = Instantiate(drops[i], droppedPos, new Quaternion(0, 0, 0, 0)) as GameObject;
                dropped.GetComponent<Collectible>().SetRespawnable(false);
            }
        }
    }

}
