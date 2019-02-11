/*  This defines all of the parameters for the mini boss's behavior
 *  Extends from BaseEnemy, in order to simplify the proccess.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMiniBossActions : BaseEnemy {

    [Header("Sub States")]
    public bool isAttacking = false;

    [Header("Sub External Refs")]
    public GameObject bombPrefab;
    public MiniBossHealth bossHealth;
    public EnemyGraphics bossGraphics;

    // Determines how the boss will move around
    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    // All non physics stuff occurs here (like graphic changes)
    private void Update()
    {
        GraphicUpdate();
    }

    // All physics stuff happens here (like moving)
	private void FixedUpdate()
	{
        //Move();
	}

	// Changes the boss's graphics depending on its state
	private void GraphicUpdate()
    {
        if(bossHealth.CurrentHealth <= 0)
        {
            // Boss is defeated
            bossGraphics.SwitchSprite("defeat");
        }
        else if(isAttacking == true)
        {
            // Boss is currently attacking
            bossGraphics.SwitchSprite("attack");
        }
        else
        {
            // Default pose
            bossGraphics.SwitchSprite("normal");
        }
    }

    // This method is used to create a logical attack sequence
    IEnumerator Attack()
    {
        yield return null;
    }
}
