/*  This script moves anything that is colliding with this in the direction of the player
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleHitbox : MonoBehaviour {

    [Header("Outside References")]
    public Rigidbody2D playerRB;
    public PlayerController playerController;

    //Private Variables
    private Rigidbody2D objectToBeInhaled;

    // If an enemy is in the inhale range, they will start to be dragged toward the player
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.tag == "Enemy")
        {
            objectToBeInhaled = collision.gameObject.GetComponent<Rigidbody2D>();
        }
	}

    // Pulls the enemy toward the player
	private void OnTriggerStay2D(Collider2D collision)
	{
        if(collision.tag == "Enemy" && objectToBeInhaled != null)
        {
            // Calculates the position that the enemy will be pulled in
            Vector2 newPos = Vector2.MoveTowards(objectToBeInhaled.position, playerRB.position, 0.1f);

            // If the enemy is close to the player while they are being inhaled, they will make the player stuffed
            if(Vector2.Distance(playerRB.position, newPos) <= 1f)
            {
                playerController.playerGraphics.ChangeSpriteAnimatorState("isStuffed");
                objectToBeInhaled.gameObject.SetActive(false);
                objectToBeInhaled = null;
                gameObject.SetActive(false);
            }
            else
            {
                objectToBeInhaled.MovePosition(newPos);
            }
        }
	}
}
