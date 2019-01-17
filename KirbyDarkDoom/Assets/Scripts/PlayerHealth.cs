/*  This controls the player's health
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth
{
    [Header("Sub Variables")]
    public int numbOfLives = 3;

    [Header("Sub Components")]
    public Rigidbody2D playerRB;
    public BoxCollider2D playerHitBox;

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
            playerHitBox.enabled = false;
            playerRB.freezeRotation = false;

            Invoke("Respawn", 2f);
        }
        else if(isDying == true)
        {
            playerRB.MoveRotation(playerRB.rotation + 10f);
        }
    }

    // Does what the base Respawn does, except this one resets the variables in this method
    public override void Respawn()
    {
        if(numbOfLives > 0)
        {
            base.Respawn();

            playerRB.rotation = 0f;
            isDying = false;
            playerHitBox.enabled = true;
            playerRB.freezeRotation = true;
            numbOfLives--;
        }
        else
        {
            // GAMEOVER SCREEN
            GameManager.Instance.GoToGameOver();
        }
    }

    // Constantly checks if the player is dead
	private void Update()
	{
        DyingAction();
	}
}
