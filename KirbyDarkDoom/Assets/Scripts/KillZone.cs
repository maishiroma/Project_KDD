/*  Anything that touches this area will be removed from the game/killed
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    // Anything that has health will take mortal damage
	private void OnTriggerStay2D(Collider2D collision)
	{
        if(collision.gameObject.GetComponent<BaseHealth>() != null)
        {
            BaseHealth health = collision.gameObject.GetComponent<BaseHealth>();

            health.isInvincible = false;
            health.TakeDamage(health.maxHealth);
        }
	}
}
