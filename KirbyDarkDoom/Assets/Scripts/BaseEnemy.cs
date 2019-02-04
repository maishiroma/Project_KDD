/*  This defines the basic enemy AI movement. Other Enemy Classes will be derived from this, since this will define all of the 
 *  standard enemy movement.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour {

	[Header("Base Variables")]
    public float moveSpeed = 5f;
    public float attackPower = 20f;
    public bool startFacingLeft = false;        // A variable that is only used at the start of an enemy's life

    [Header("Base Component References")]
    public Rigidbody2D enemyRB;
    public Transform frontOfEnemy;

    // Private Variables
    private bool isFacingRight = true;
    private Vector2 origVelocity;

    // Getters
    public bool IsFacingRight {
        get { return isFacingRight;}
    }

    // This stops the enemy from moving, saving its original velocity
    public void StopEnemy()
    {
        origVelocity = enemyRB.velocity;
        enemyRB.velocity = Vector2.zero;
        enemyRB.isKinematic = true;
        this.enabled = false;
    }

    // This resumes enemy movement, with the option to use its saved velocity
    public void ResumeEnemy(bool useOrigVelocity)
    {
        if(useOrigVelocity == true)
        {
            enemyRB.velocity = origVelocity;
        }
        enemyRB.isKinematic = false;
        this.enabled = true;
    }

    // Does a check to see if there's anything in front of the enemy
    public bool CheckFrontOfEnemy()
    {
        if(isFacingRight == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(frontOfEnemy.position, Vector2.right, 0.1f);
            if(hit == true)
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Indestructable") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Destructable"))
                {
                    return true;
                }
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(frontOfEnemy.position, -Vector2.right, 0.1f);
            if(hit == true)
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Indestructable") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Destructable"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Turns the enemy around, reorientating its front
    public void TurnAround()
    {
        isFacingRight = !isFacingRight;
        frontOfEnemy.transform.localPosition = new Vector2(-frontOfEnemy.transform.localPosition.x,frontOfEnemy.transform.localPosition.y);
    }

    /* Absract Methods */
    public abstract void Move();
}
