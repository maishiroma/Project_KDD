/*  Basis for collecting an item
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    HEALTH,
    LIVES
}

public class Collectible : MonoBehaviour {

	[Header("General Variables")]
    public CollectibleType typeOfCollectible;

    [Range(1f,100f)]
    public float amount;

    // Makes sure the amount is a resonable amount depending on the Collectible Type
	private void OnValidate()
	{
        if(typeOfCollectible == CollectibleType.LIVES)
        {
            // If the type if lives, we will always make sure the value of this will be 1
            amount = 1;
        }
	}

	// When the player runs into this, they will collect this
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.gameObject.tag == "Player")
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

            switch(typeOfCollectible)
            {
                case CollectibleType.HEALTH:
                    player.RestoreHealth(amount);
                    break;
                case CollectibleType.LIVES:
                    player.numbOfLives += (int)amount;
                    break;
            }
            gameObject.SetActive(false);
        }
	}
}
