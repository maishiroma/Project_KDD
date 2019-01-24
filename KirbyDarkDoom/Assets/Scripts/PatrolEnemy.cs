/*  Derives from BaseEnemy. This enemy simple patrols around the area they are around.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
    [Header("General Sub Variables")]
    public bool moveSetDisitance = false;
    public float turnTime = 5f;

    [Header("Flying Enemy")]
    public bool canFly = false;
    public float flySpeed = 10f;
    [Range(0.1f,1f)]
    public float flyGravity = 0.5f;
    public float verticalTimer = 1f;

    // private variables
    private bool isMovingUp = false;
    private float origGravity;

    // This enemy will constantly move
    public override void Move()
    {
        if(isFacingRight == true)
        {
            enemyRB.AddForce(Vector2.right * moveSpeed);
        }
        else
        {
            enemyRB.AddForce(-Vector2.right * moveSpeed);
        }

        // If the enemy is allowed to fly, they will move upwards
        if(canFly == true && isMovingUp == true)
        {
            enemyRB.AddForce(Vector2.up * flySpeed);
        }
    }

    // Initializes the TurnAroundRepeater, which will make the enemy turn around automatically after X seconds
	private void Start()
	{
        origGravity = enemyRB.gravityScale;
        if(moveSetDisitance == true)
        {
            InvokeRepeating("TurnAround", turnTime, turnTime);
        }

        if(canFly == true)
        {
            InvokeRepeating("ToggleFlying", verticalTimer, verticalTimer);
        }
	}

    // This method is used to verify that the values in the inspector are valid
	[ExecuteInEditMode]
    private void OnValidate()
    {
        turnTime = Mathf.Clamp(turnTime, 1f, 100f);
        verticalTimer = Mathf.Abs(verticalTimer);
        flySpeed = Mathf.Abs(flySpeed);
    }

	// Calls the move function that was overriden
	private void FixedUpdate()
	{
        Move();
	}

	// Behavior when the enemy hits something
	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.layer == LayerMask.NameToLayer("Indestructable") || collision.gameObject.layer == LayerMask.NameToLayer("Destructable"))
        {
            // If the enemy collides into a wall, block, or enemy, they will change directions
            TurnAround();
        }
        else if(collision.gameObject.tag == "Player")
        {
            // Enemy is defeated, but player also takes damage, which is handled in PlayerController
            gameObject.SetActive(false);
        }
	}

    // Used in an Invoke to make the enemy turn around.
    private void TurnAround()
    {
        isFacingRight = !isFacingRight;
    }

    // Used in an Invoke to stop the enemy from ascending/descending
    private void ToggleFlying()
    {
        if(isMovingUp == true)
        {
            isMovingUp = false;
            enemyRB.gravityScale = origGravity;
        }
        else
        {
            isMovingUp = true;
            enemyRB.gravityScale = flyGravity;
        }
    }
}
