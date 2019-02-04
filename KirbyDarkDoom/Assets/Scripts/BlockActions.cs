/* Keeps a list of handy actions for destructable blocks
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockActions : MonoBehaviour {

    // Private Variables
    private Vector2 initialLocation;

    // Sets the inial location of the block
	private void Awake()
	{
        initialLocation = gameObject.transform.position;
	}

    // Reactivates and places the block back to its original location
    public void ResetBlock()
    {
        gameObject.transform.position = initialLocation;
        if(gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
    }
}
