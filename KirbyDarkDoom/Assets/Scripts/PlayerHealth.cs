/*  By inderiting from BaseHealth, this adds additional features to the player's Health mechanic
 * 
 */
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
        if(CurrentHealth <= 0 && isDying == false)
        {
            // We start the dying animation
            isDying = true;
            playerHitBox.enabled = false;
            playerRB.freezeRotation = false;

            Invoke("Respawn", 2f);
        }
        else if(isDying == true)
        {
            // While the player is dying, we rotate them (graphic effect)
            playerRB.MoveRotation(playerRB.rotation + 10f);
        }
    }

    // Does what the base Respawn does, except this one resets the variables in this method
    public override void Respawn()
    {
        if(numbOfLives > 0)
        {
            base.Respawn();

            // we reset the player's variables back to normal
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
