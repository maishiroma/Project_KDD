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
        if(collision.gameObject.tag == "Enemy")
        {
            // Defeat the enemy and stop the slide
            collision.gameObject.GetComponent<NormalEnemyHealth>().TakeDamage(slideAttackPower);
            player.isSliding = false;
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Block")
        {
            // Destroy the block and keep going
            collision.gameObject.SetActive(false);
        }
	}
}
