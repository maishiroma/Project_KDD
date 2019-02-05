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
        if(collision.gameObject.GetComponent<BaseHealth>() != null)
        {
            // If the entiry we run into has healh, we damage said entity
            collision.gameObject.GetComponent<BaseHealth>().TakeDamage(slideAttackPower);

            if(collision.gameObject.tag == "Enemy")
            {
                // If the player ran into an enemy, the player will stop its slide
                player.isSliding = false;
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Anything else, the player will stop its slide
            player.isSliding = false;
            gameObject.SetActive(false);
        }
	}
}
