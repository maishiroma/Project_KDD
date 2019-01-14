/*  Derives from BaseEnemy. This enemy simple patrols around the area they are around.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : BaseEnemy
{
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

    // Calls the move function that was overriden
    private void FixedUpdate()
	{
        Move();
	}

    // If the enemy collides into a wall, they will change directions
	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.tag == "Wall" && collision.gameObject.layer == LayerMask.NameToLayer("Indestructable"))
        {
            isFacingRight = !isFacingRight;
        }
	}
}
