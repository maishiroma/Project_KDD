/*  By inderiting from BaseHealth, this adds additional features to the player's Health mechanic
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : BaseHealth
{
    [Header("Sub Variables")]
    public int numbOfLives = 3;

    [Header("Sub Components")]
    public PlayerController playerController;
    public Rigidbody2D playerRB;
    public BoxCollider2D playerHitBox;

    [Header("UI References")]
    public Slider health_UI;
    public TextMeshProUGUI life_UI;

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
        if(numbOfLives > 1)
        {
            base.Respawn();

            // We respawn all of the enemies and blocks
            GameManager.Instance.RespawnAssociatedEnemies();
            GameManager.Instance.RespawnAssociatedBlocks();

            // we then the player's variables back to normal
            // This is done so that the player is in front of everything
            playerController.ResetPlayerMovement(playerController.isFacingRight);
            playerRB.transform.position += new Vector3(0,0,-5f);
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

        // Also updates the UI accordingly
        health_UI.value = CurrentHealth;
        life_UI.text = "x " + numbOfLives;
	}
}
