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

    [Header("Base States")]
    public bool isFacingRight = true;

    [Header("Base Component References")]
    public Rigidbody2D enemyRB;

    // Private Variables
    private Vector2 origVelocity;

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

    /* Absract Methods */
    public abstract void Move();
}
