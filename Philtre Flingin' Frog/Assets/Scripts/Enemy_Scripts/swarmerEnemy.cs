using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum SwarmerState
{
    IDLE,
    AGGRO,
    NUM_STATES
}

public class swarmerEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player; //Position of the player character

    public LayerMask projectile;
    public float objectHeight;

    public float playerAggroRadius; //Radius in which becomes aggro if player enters
    public float deAggroRadius; //Radius in which becomes idle if player leaves
    public float enemyAggroRadius; //Radius in which becomes aggro if another enemy within becomes aggro

    Vector3 forwardView; //Vector of the direction the enemy is facing
    public float viewAngle; //Half the size of the view cone, checks this many degrees either side of the middle of the cone

    SwarmerState currentState;
    Vector3 targetPosition; //Position the nav mesh is tracking for the enemy to move towards
    float idleTimer;
    public float idleWaitTime; //How long between each idle movement

    // Start is called before the first frame update
    void Start()
    {
        currentState = SwarmerState.IDLE;
        GameEvents.current.onBecameAggro += checkAggro;
        GameEvents.current.onBecameUnAggro += checkUnAggro;
        GameEvents.current.onHitByPotion += hitByPotionAlert;
        forwardView = transform.forward;
        targetPosition = transform.position;
        idleTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Sets the player character if not already set or was reset
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        if (currentState == SwarmerState.AGGRO)
        {
            Aggro();
        }
        else if (currentState == SwarmerState.IDLE)
        {
            Idle();
        }

        //Counts down until enemy can charge attack again
        if (idleTimer > 0.0f && !GameManager.GetPaused() && currentState == SwarmerState.IDLE)
        {
            idleTimer -= Time.deltaTime;
        }

        agent.SetDestination(targetPosition);
    }

    //Carries out the enemy's aggro state
    private void Aggro()
    {
        //Determines if the player is far enough away to no longer chase
        if (Vector3.Distance(player.position, gameObject.transform.position) > deAggroRadius)
            setIdle();
        else if (player.position != null && !GameManager.GetPaused()) //Moves towards the player if the game is not paused
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            targetPosition = player.position;
            forwardView = new Vector3(player.position.x - gameObject.transform.position.x, forwardView.y, player.position.z - gameObject.transform.position.z);
        }
        else //Stops in place if the game is paused
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    //Carries out the enemy's idle state
    private void Idle()
    {
        //Determinesthe angle between the player's position and the enemy's current forward vector
        Vector3 playerDir = new Vector3(player.position.x - gameObject.transform.position.x, forwardView.y, player.position.z - gameObject.transform.position.z);
        float d = Vector3.Angle(playerDir, forwardView);

        //Determines if the player is close enough to chase and within the enemy's view cone, then sets it to start chasing
        if (Vector3.Distance(player.position, gameObject.transform.position) < playerAggroRadius && d < viewAngle)
            setAggro();
        else if (targetPosition.x == gameObject.transform.position.x && targetPosition.z == gameObject.transform.position.z && idleTimer <= 0)
        {
            targetPosition = new Vector3(gameObject.transform.position.x + Random.Range(-3.0f, 3.0f), gameObject.transform.position.y, gameObject.transform.position.z + Random.Range(-3.0f, 3.0f));
            forwardView = targetPosition - transform.position;
            idleTimer = idleWaitTime;
        }
    }

    //Sets the enemy into an idle state
    void setIdle()
    {
        currentState = SwarmerState.IDLE;
        GameEvents.current.becameUnAggro(gameObject.transform.position);
    }

    //Sets the enemy into an attacking state and sets off a game event that it has done so
    void setAggro()
    {
        currentState = SwarmerState.AGGRO;
        GameEvents.current.becameAggro(gameObject.transform.position);
    }

    //Checks if near a recently aggro-ed enemy
    private void checkAggro(Vector3 pos)
    {
        if (currentState != SwarmerState.AGGRO)
        {
            if (Vector3.Distance(pos, gameObject.transform.position) <= enemyAggroRadius && pos != gameObject.transform.position)
                setAggro();
        }
    }

    //Checks if near a recently idled swarmer enemy
    private void checkUnAggro(Vector3 pos)
    {
        if (currentState == SwarmerState.AGGRO)
        {
            if (Vector3.Distance(pos, gameObject.transform.position) <= enemyAggroRadius && pos != gameObject.transform.position)
                setIdle();
        }
    }

    //Checks to see if was hit by a potion, then becomes aggro if was
    private void hitByPotionAlert(Vector3 pos)
    {
        if (pos != gameObject.transform.position)
            setAggro();
    }

    void OnDestroy()
    {
        System.Type enemyType = typeof(swarmerEnemy);
        GameEvents.current.enemyDies(enemyType);
    }


}
