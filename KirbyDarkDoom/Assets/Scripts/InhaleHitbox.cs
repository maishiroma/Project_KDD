/*  This script moves whatever is considered valid in the direction of the player.
 *  This is activated by pressing H from the Player Controller.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleHitbox : MonoBehaviour {

    [Header("General Variables")]
    [Range(0.1f,1f)]
    public float inhalePower = 0.1f;

    [Header("Outside References")]
    public Rigidbody2D playerRB;
    public PlayerController playerController;

    //Private Variables
    private Rigidbody2D rbToInhale;
    private bool isInhalingObject;

	// If a vaild object is in the inhale range, they will start to be dragged toward the player
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(CheckIfInhalable(collision.gameObject) == true && isInhalingObject == false)
        {
            isInhalingObject = true;
            if(collision.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                rbToInhale = collision.gameObject.GetComponent<Rigidbody2D>();
            }
        }
	}

    // Pulls the object toward the player
	private void OnTriggerStay2D(Collider2D collision)
	{
        if(CheckIfInhalable(collision.gameObject) == true && isInhalingObject == true)
        {
            Vector2 newPos;

            // Calculates the position that the object will be pulled in
            // Depending on whether or not the object has a RB, we determine how we check its position
            if(rbToInhale != null)
            {
                newPos = Vector2.MoveTowards(rbToInhale.position, playerRB.position, inhalePower);
            }
            else
            {
                newPos = Vector2.MoveTowards(collision.transform.position, playerRB.position, inhalePower);
            }

            // If the object is close to the player while they are being inhaled, they will make the player stuffed
            int sign = 1;
            int layerMasks = (1 << (LayerMask.NameToLayer("Indestructable"))) | (1 << LayerMask.NameToLayer("Destructable"));
            if(playerController.isFacingRight == false)
            {
                sign *= -1;
            }

            if(Physics2D.Raycast(playerRB.position, Vector2.right * sign, 1f, layerMasks))
            {
                if(collision.gameObject.tag == "Collectible")
                {
                    // If the player inhales a collectible, they will collect it.
                    collision.gameObject.GetComponent<Collectible>().CollectCollectible(playerController.playerHealth);
                }
                else
                {
                    playerController.playerGraphics.ChangeSprite("isStuffed");
                    playerController.isStuffed = true;
                    collision.gameObject.SetActive(false);
                }

                playerController.isInhaling = false;
                gameObject.SetActive(false);
                rbToInhale = null;
                isInhalingObject = false;
            }
            else
            {
                if(rbToInhale != null)
                {
                    rbToInhale.MovePosition(newPos);
                }
                else
                {
                    collision.transform.position = newPos;
                }
            }
        }
	}

    // Makes sure the Inhalled reference is gone when it leaves the hitbox
	private void OnTriggerExit2D(Collider2D collision)
	{
        rbToInhale = null;
        isInhalingObject = false;
	}

	// Checks if the given object is something the player can inhale
	private bool CheckIfInhalable(GameObject inhaledObject)
    {
        if(inhaledObject.GetComponent<BaseHealth>() != null)
        {
            // We first check if the object has a health component and if so, we check its property CanBeInhalled
            if(inhaledObject.GetComponent<BaseHealth>().canBeInhaled == true)
            {
                return true;
            }
        }
        else
        {
            // Otherwise we look at its tag
            switch(inhaledObject.tag)
            {
                case "Collectible":
                    return true;
            }
        }
        return false;
    }
}
