/*  Derives from BaseEnemy. This enemy simple patrols around the area they are around.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
    [Header("Sub Variables")]
    public bool moveSetDisitance = false;
    public float turnTime = 5f;

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
    }

    // Initializes the TurnAroundRepeater, which will make the enemy turn around automatically after X seconds
	private void Start()
	{
        if(moveSetDisitance == true)
        {
            InvokeRepeating("TurnAround", turnTime, turnTime);
        }
	}

    // This method is used to verify that the values in the inspector are valid
	[ExecuteInEditMode]
    private void OnValidate()
    {
        turnTime = Mathf.Clamp(turnTime, 1f, 100f);
    }

	// Calls the move function that was overriden
	private void FixedUpdate()
	{
        Move();
	}

	// Behavior when the enemy hits something
	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Block" || collision.gameObject.tag == "Enemy")
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
}
