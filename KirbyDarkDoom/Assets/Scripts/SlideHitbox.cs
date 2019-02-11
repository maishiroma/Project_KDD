/*  This detects whether the player runs into a specific object
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideHitbox : MonoBehaviour {

    [Header("General Variables")]
    public float slideAttackPower = 20f;

	[Header("External Components")]
    public PlayerController player;

	// If the player runs into a specific entiry, 
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.gameObject.layer == LayerMask.NameToLayer("Indestructable") || collision.gameObject.layer == LayerMask.NameToLayer("Destructable"))
        {
            if(collision.gameObject.GetComponent<BaseHealth>() != null)
            {
                if(collision.gameObject.tag == "Enemy")
                {
                    // If the player ran into an enemy, the player will stop its slide AND will danage the enemy
                    player.isSliding = false;
                    gameObject.SetActive(false);
                }
                else if(collision.gameObject.tag == "MiniBoss")
                {
                    // If the player ran into a miniboss, the player will stop its slide, BUT WONT damage the boss
                    player.isSliding = false;
                    gameObject.SetActive(false);
                    return;
                }

                collision.gameObject.GetComponent<BaseHealth>().TakeDamage(slideAttackPower);
            }
            else
            {
                // We stop the slide if the player is hitting a solid object
                player.isSliding = false;
                gameObject.SetActive(false);
            }
        }
	}
}
