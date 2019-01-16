/*  This controls the player's health
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth
{
    // Private variables
    private bool isDying = false;

    // Getter for IsDying
    public bool IsDying {
        get {return isDying;}
    }

    // When the player loses all of their health, they will lost a life
    public override void DyingAction()
    {
        if(currentHealth <= 0 && isDying == false)
        {
            isDying = true;

            // TODO: Cutscene of player dying
            Invoke("RemovePlayer", 2f);
        }
    }

    // Constantly checks if the player is dead
	private void Update()
	{
        DyingAction();
	}

	// Called in an invoke to remove the player temporarily.
	private void RemovePlayer()
    {
        gameObject.SetActive(false);
    }
}
