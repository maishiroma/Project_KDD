/*  This defines the basic enemy AI movement. Other Enemy Classes will be derived from this, since this will define all of the 
 *  standard enemy movement.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour {

	[Header("Base Variables")]
    public float moveSpeed = 5f;

    [Header("Base States")]
    public bool isFacingRight = true;

    [Header("Base Component References")]
    public Rigidbody2D enemyRB;

    /* Absract Methods */
    public abstract void Move();
}
