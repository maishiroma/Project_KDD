/*  This defines all of the parameters for the mini boss's behavior
 *  Extends from BaseEnemy, in order to simplify the proccess.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMiniBossActions : BaseEnemy {

    [Header("Sub General Vars")]
    public float dashSpeed;
    public float bombThrowSpeed;
    public float jumpPower;

    [Header("Sub Timers")]
    public float behaviorChangeTime;
    public float paceTime;
    public float dashTime;
    public float jumpHeightTime;
    public float cooldownTime;

    [Header("Sub External Refs")]
    public GameObject bombPrefab;
    public Transform bombSpawnPoint;
    public MiniBossHealth bossHealth;
    public EnemyGraphics bossGraphics;

    // Private variables
    private EnemyStates currentState;       // The current state of the miniboss, which determines its specific actions
    private bool hasActivatedCooldown;
    private bool isMovingForward;
    private float origSpeed;
    private GameObject spawnedBombInstance;

    public EnemyStates CurrentState {
        get {return currentState;}
        set {
            // When you switch states, its graphics will change accordingly
            switch(value)
            {
                case EnemyStates.IDLE:
                    bossGraphics.SwitchSprite("normal");
                    break;
                case EnemyStates.ATTACK1:
                case EnemyStates.ATTACK2:
                    bossGraphics.SwitchSprite("attack");
                    break;
                case EnemyStates.DEFEAT:
                    bossGraphics.SwitchSprite("defeat");
                    break;
            }
            currentState = value;
        }
    }

    // Determines how the boss will move around
    public override void Move()
    {
        switch(CurrentState)
        {
            case EnemyStates.IDLE:
                // Miniboss simply paces around
                if(isMovingForward == true)
                {
                    enemyRB.AddForce(Vector2.right * moveSpeed);
                }
                else
                {
                    enemyRB.AddForce(-Vector2.right * moveSpeed);
                }

                if(!IsInvoking("ChangePaceDirec"))
                {
                    Invoke("ChangePaceDirec", paceTime);
                }
                break;
            case EnemyStates.ATTACK1:
                // MiniBoss performs a dash forward
                if(IsFacingRight == true)
                {
                    enemyRB.AddForce(Vector2.right * dashSpeed);
                }
                else
                {
                    enemyRB.AddForce(-Vector2.right * dashSpeed);
                }

                if(hasActivatedCooldown == false)
                {
                    StartCoroutine(StopDashAttack());
                }
                break;
            case EnemyStates.ATTACK2:
                // Miniboss jumps and performs a bomb throw
                enemyRB.AddForce(transform.up * jumpPower);

                if(spawnedBombInstance == null)
                {
                    spawnedBombInstance = Instantiate(bombPrefab, bombSpawnPoint.position, Quaternion.identity, gameObject.transform);
                    spawnedBombInstance.GetComponent<BoxCollider2D>().isTrigger = true;
                    spawnedBombInstance.GetComponent<Rigidbody2D>().isKinematic = true;
                }
                if(hasActivatedCooldown == false)
                {
                    StartCoroutine(BombAttack());
                }
                break;
        }
    }

    // When starting up, we reset all of the enemy statuses
    private void OnEnable()
    {
        enemyRB.velocity = Vector2.zero;

        if(startFacingLeft == true && IsFacingRight == true)
        {
            // If we make the enemy start facing left, we only turn them around if they are facing right
            TurnAround();
        }
    }

    // When the enemy is defeated, all invokes are canceled
    private void OnDisable()
    {
        CancelInvoke();
    }

    // Makes sure all of the timers are positive values
	private void OnValidate()
	{
        dashSpeed = Mathf.Clamp(dashSpeed, 0, Mathf.Infinity);
        bombThrowSpeed = Mathf.Clamp(bombThrowSpeed, 0, Mathf.Infinity);
        jumpPower = Mathf.Clamp(jumpPower, 0, Mathf.Infinity);

        behaviorChangeTime = Mathf.Clamp(behaviorChangeTime, 0, 100);
        paceTime = Mathf.Clamp(paceTime, 0, 100);
        dashTime = Mathf.Clamp(dashTime, 0, 100);
        jumpHeightTime = Mathf.Clamp(jumpHeightTime, 0, 100);
        cooldownTime =  Mathf.Clamp(cooldownTime, 0, 100);
	}

	// Saves the initial speef of the player
	private void Awake()
	{
        origSpeed = moveSpeed;
	}

    // Initializes the behavior of the enemy for the entire lifespan
	private void Start()
	{
        InvokeRepeating("RandomBehavior", behaviorChangeTime, behaviorChangeTime);
	}

    // All physics stuff happens here (like moving)
	private void FixedUpdate()
	{
        // As long as the boss is still alive, they will keep on moving
        if(bossHealth.CurrentHealth > 0)
        {
            Move();
        }
	}

    // Sets up the bahavior of the EnemyAI
    private void RandomBehavior()
    {
        if(CurrentState == EnemyStates.IDLE)
        {
            /*  
             * If the RANS is either a 1 or a 2, the boss will do something, IF they are in the IDLE state
             * 
             */
            int rand = (int)Random.Range(1,3);
            if(rand == 1)
            {
                // DASH ATTACK
                CurrentState = EnemyStates.ATTACK1;
            }
            else if(rand == 2)
            {
                // BOMB THROW
                CurrentState = EnemyStates.ATTACK2;
            }
        }
    }

    // Called in an Invoke to move the opposite way
    private void ChangePaceDirec()
    {
        isMovingForward = !isMovingForward;
    }

    // Makes the miniboss resume back to its idle state after attacking
    IEnumerator StopDashAttack()
    {
        // We perform the dash
        hasActivatedCooldown = true;
        yield return new WaitForSeconds(dashTime);

        // We enter cooldown period
        CurrentState = EnemyStates.COOLDOWN;
        yield return new WaitForSeconds(cooldownTime);

        // We then reset the state back
        TurnAround();
        CurrentState = EnemyStates.IDLE;
        hasActivatedCooldown = false;
        yield return null;
    }

    // Handles the bomb attacks, and resumes back to its idle position
    IEnumerator BombAttack()
    {
        // We jump up to the max height
        hasActivatedCooldown = true;
        yield return new WaitForSeconds(jumpHeightTime);

        // We then throw the bomb forward
        spawnedBombInstance.GetComponent<Rigidbody2D>().isKinematic = false;
        spawnedBombInstance.GetComponent<BoxCollider2D>().isTrigger = false;
        spawnedBombInstance.GetComponent<PatrolEnemy>().moveSpeed = bombThrowSpeed;
        yield return new WaitForFixedUpdate();

        // We enter cooldown
        CurrentState = EnemyStates.COOLDOWN;
        yield return new WaitForSeconds(cooldownTime);

        // And then we reset the state
        CurrentState = EnemyStates.IDLE;
        hasActivatedCooldown = false;
        spawnedBombInstance = null;
        yield return null;
    }
}
