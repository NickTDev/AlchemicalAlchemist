using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum ChargerState
{
    IDLE,
    AGGRO,
    WAITING,
    CHARGING,
    STOPPING,
    NUM_STATES
}

public class chargerEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player; //Position of the player character
    Enemy enemyData; //Data including current speed

    public LayerMask projectile;
    public float objectHeight;

    public float playerAggroRadius; //Radius in which becomes aggro if player enters
    public float deAggroRadius; //Radius in which becomes idle if player leaves
    public float enemyAggroRadius; //Radius in which becomes aggro if another enemy within becomes aggro
    public float playerChargeRadius; //Radius in which the player must be for the enemy to charge
    public float playerWaitRadius; //Radius in which the enemy will stop chasing and wait to use its charge

    public float chargeAttackCooldown; //How long between uses of the charge attack
    public float chargeAttackDuration; //How long the charge attack occurs for
    public float waitDuration; //How long the player waits before charging once it can charge again
    float currentCooldownTimer = 0;
    float currentChargeTimer = 0;
    public float chargeSpeed; //How fast the enemy moves during a charge attack
    Vector3 chargePosition;

    Vector3 forwardView; //Vector of the direction the enemy is facing
    public float viewAngle; //Half the size of the view cone, checks this many degrees either side of the middle of the cone

    ChargerState currentState;
    Vector3 targetPosition; //Position the nav mesh is tracking for the enemy to move towards
    float idleTimer;
    public float idleWaitTime; //How long between each idle movement

    [SerializeField] FMODUnity.EventReference AggroSFX;


    // Start is called before the first frame update
    void Start()
    {
        currentState = ChargerState.IDLE;
        GameEvents.current.onBecameAggro += checkAggro;
        GameEvents.current.onHitByPotion += hitByPotionAlert;
        forwardView = transform.forward;
        enemyData = gameObject.GetComponent<Enemy>();
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

        if (currentState == ChargerState.AGGRO)
        {
            Aggro();
        }
        else if (currentState == ChargerState.IDLE)
        {
            Idle();
        }
        else if (currentState == ChargerState.CHARGING)
        {
            Charge();
        }
        else if (currentState == ChargerState.STOPPING)
        {
            Stop();
        }
        else if (currentState == ChargerState.WAITING)
        {
            Wait();
        }

        //Counts down until enemy can charge attack again
        if (currentCooldownTimer > 0.0f && !GameManager.GetPaused())
        {
            currentCooldownTimer -= Time.deltaTime;
        }

        //Counts down until enemy can charge attack again
        if (idleTimer > 0.0f && !GameManager.GetPaused() && currentState == ChargerState.IDLE)
        {
            idleTimer -= Time.deltaTime;
        }

        agent.SetDestination(targetPosition);
    }

    //Carries out the enemy's aggro state
    private void Aggro()
    {
        //Check if player is outside of the aggro radius
        if (Vector3.Distance(player.position, gameObject.transform.position) > deAggroRadius)
            setIdle();
        else if (player.position != null && !GameManager.GetPaused())
        {
            //Check if the player is inside the radius to be able to charge
            if (Vector3.Distance(player.position, gameObject.transform.position) > playerChargeRadius)
            {
                gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                targetPosition = player.position;
                forwardView = new Vector3(player.position.x - gameObject.transform.position.x, forwardView.y, player.position.z - gameObject.transform.position.z);
            }
            else if (Vector3.Distance(player.position, gameObject.transform.position) > playerWaitRadius) //Check if the player is in the waiting radius
            {
                gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                //Check to see if can charge and calls the ability if it can
                if (currentCooldownTimer <= 0.0f)
                {
                    chargePosition = Vector3.Normalize(player.position - gameObject.transform.position);
                    setWaiting();
                }
                else //Otherwise just follows player
                {
                    targetPosition = player.position;
                    forwardView = new Vector3(player.position.x - gameObject.transform.position.x, forwardView.y, player.position.z - gameObject.transform.position.z);
                }
            }
            else
                setStopping();
        }
        else //Otherwise keeps enemy in place
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
        {
            currentCooldownTimer = chargeAttackCooldown;
            setAggro();
        }
        else if (targetPosition.x == gameObject.transform.position.x && targetPosition.z == gameObject.transform.position.z && idleTimer <= 0)
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            targetPosition = new Vector3(gameObject.transform.position.x + Random.Range(-3.0f, 3.0f), gameObject.transform.position.y, gameObject.transform.position.z + Random.Range(-3.0f, 3.0f));
            forwardView = targetPosition - transform.position;
            idleTimer = idleWaitTime;
        }
        else //Otherwise keeps enemy in place
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    //Carries out the enemy's charging state
    private void Charge()
    {
        //Checks if can still keep charging
        if (currentChargeTimer > 0.0f && !GameManager.GetPaused())
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            currentChargeTimer -= Time.deltaTime;

            //Change Player's speed to faster speed
            enemyData.currentMoveSpeed = chargeSpeed;

            //Set destination in the direction of the saved player position
            targetPosition = (gameObject.transform.position + chargePosition);
        }
        else if (!GameManager.GetPaused()) //Resets enemy speed to stop charging and follows player as normal
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            enemyData.currentMoveSpeed = enemyData.baseMoveSpeed;
            currentCooldownTimer = chargeAttackCooldown;
            setAggro();
        }
        else //Otherwise stays in place
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    //Carries out the enemy's stopping state
    private void Stop()
    {
        //Checks if player is too close to follow
        if (Vector3.Distance(player.position, gameObject.transform.position) > playerWaitRadius)
            setAggro();
        else if (currentCooldownTimer <= 0.0f) //Checks if enemy can use its charge attack and then uses it if it can
            setWaiting();
        else //Otherwise stays in place
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    //Carries out the enemy's waiting state
    private void Wait()
    {
        //Keeps the enemy in place until charge can start
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    //Sets the enemy into an idle state
    void setIdle()
    {
        currentState = ChargerState.IDLE;
    }

    //Sets the enemy into an attacking state and sets off a game event that it has done so
    void setAggro()
    {
        if (currentState != ChargerState.AGGRO)
            AudioEngineManager.PlaySound(AggroSFX, 1f, this.transform.position);
        currentState = ChargerState.AGGRO;
        GameEvents.current.becameAggro(gameObject.transform.position);
    }

    //Forces the enemy to stop moving and sets the charge attack to begin in a set amount of time
    void setWaiting()
    {
        currentState = ChargerState.WAITING;
        Invoke("setCharging", waitDuration);
    }

    //Sets the enemy to begin its charge attack
    void setCharging()
    {
        currentChargeTimer = chargeAttackDuration;
        currentState = ChargerState.CHARGING;
    }

    //Sets the enemy to stop moving until otherwise directed
    void setStopping()
    {
        currentState = ChargerState.STOPPING;
    }

    //Checks if near a recently aggro-ed enemy
    private void checkAggro(Vector3 pos)
    {
        if (currentState != ChargerState.AGGRO)
        {
            if (Vector3.Distance(pos, gameObject.transform.position) <= enemyAggroRadius && pos != gameObject.transform.position)
                setAggro();
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
        System.Type enemyType = typeof(chargerEnemy);
        GameEvents.current.enemyDies(enemyType);
    }

}
