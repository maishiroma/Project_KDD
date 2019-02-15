/*  This script spawns a specific GameObject when the player enters its area of range
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateEvent : MonoBehaviour {

    public GameObject[] listOfRelatedObjects;

    // Private variables
    private bool hasAlreadyActivated;

    // When the player enters this area, the object will be reactivated
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.gameObject.tag == "Player" && hasAlreadyActivated == false)
        {
            foreach (GameObject item in listOfRelatedObjects)
            {
                item.SetActive(true);
            }
            hasAlreadyActivated = true;
        }
	}

    // Resets this event when this method is called
    public void ResetEvent()
    {
        if(hasAlreadyActivated == true)
        {
            foreach (GameObject item in listOfRelatedObjects)
            {
                item.SetActive(false);
            }
            hasAlreadyActivated = false;
        }
    }
}
