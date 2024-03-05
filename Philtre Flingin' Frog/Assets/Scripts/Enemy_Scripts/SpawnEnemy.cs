using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public enum EnemyType
    {
        TRACKER,
        CHARGER,
        SWARMER,
        NUM_ENEMIES
    }

    public float spawnNum; //Number of enemies to spawn
    public float spawnRadius; //Radius in which enemies can spawn
    public EnemyType enemyToSpawn;

    public GameObject trackerPrefab;
    public GameObject chargerPrefab;
    public GameObject swarmerPrefab;

    public GameObject aCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onPlayerDeath += playerDied;

        spawnEnemies();
    }

    //Spawn enemies based on selected type and selected number
    public void spawnEnemies()
    {
        if (!aCheckpoint.GetComponent<Checkpoint>().hasBeenCrossed)
        {
            //Randomly determines a place to spawn in the designated radius for the designated number of enemies and spawns them
            for (int i = 0; i < spawnNum; i++)
            {
                float randX = Random.Range(-spawnRadius, spawnRadius);
                float randZ = Random.Range(-spawnRadius, spawnRadius);

                if (enemyToSpawn == EnemyType.TRACKER)
                    Instantiate(trackerPrefab, new Vector3(gameObject.transform.position.x + randX, gameObject.transform.position.y + 2.0f, gameObject.transform.position.z + randZ), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                else if (enemyToSpawn == EnemyType.CHARGER)
                    Instantiate(chargerPrefab, new Vector3(gameObject.transform.position.x + randX, gameObject.transform.position.y + 2.0f, gameObject.transform.position.z + randZ), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                else if (enemyToSpawn == EnemyType.SWARMER)
                    Instantiate(swarmerPrefab, new Vector3(gameObject.transform.position.x + randX, gameObject.transform.position.y + 2.0f, gameObject.transform.position.z + randZ), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
            }
        }
    }

    //Checks to see if player has died, then respawns enemies if true
    private void playerDied(Vector3 pos)
    {
        spawnEnemies();
    }
}
