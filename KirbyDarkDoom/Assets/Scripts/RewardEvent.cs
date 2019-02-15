/*  This script handles spawning a GameObject after certain conditions are met on the Gameobject this script lies on
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardEvent : MonoBehaviour {

    public GameObject[] listOfRelatedObjects;

    // Private variables
    private bool hasAlreadyActivated;

    // Depending on what kind of GameObject this is, we check for various things
	private void Update()
	{
        if(hasAlreadyActivated == false)
        {
            // If the GameObject this script resides on has no health left, we deem the reward to be activated
            if(gameObject.GetComponent<BaseHealth>() != null)
            {
                if(gameObject.GetComponent<BaseHealth>().CurrentHealth <= 0)
                {
                    foreach (GameObject item in listOfRelatedObjects)
                    {
                        item.SetActive(true);
                    }
                    hasAlreadyActivated = true;
                }
            }
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
