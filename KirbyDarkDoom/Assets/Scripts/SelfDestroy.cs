/*  This script, when attatched to an object, will self destroy itself after X seconds.
 *  If toggled, this can also create a damaging hitbox upon destroying.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	[Header("General Variables")]
    public bool canCreateHitbox = false;
    public float timeToDestroy;
    public float hitboxTimer;

    [Header("Outside References")]
    public GameObject hitboxRef;
    public SpriteRenderer objectSprite;

    // Reconfirmes the variables being valid timers
	private void OnValidate()
	{
        timeToDestroy = Mathf.Clamp(timeToDestroy, 0, Mathf.Infinity);
        hitboxTimer = Mathf.Clamp(hitboxTimer, 0, Mathf.Infinity);
	}

	// Starts up the self destruct proccess
	private void Start()
	{
        StartCoroutine(DestroyLogic());
	}

    // Here, depending on what canCreateHitbox does, we either immedatly remove the gameobject, or create the hitbox
    IEnumerator DestroyLogic()
    {
        // We wait for the time to destroy the GameObject
        yield return new WaitForSeconds(timeToDestroy);

        if(canCreateHitbox == true)
        {
            // If we can create a hitbox, we set it active here
            objectSprite.enabled = false;
            hitboxRef.SetActive(true);
            yield return new WaitForSeconds(hitboxTimer);
        }

        gameObject.SetActive(false);
        Invoke("RemoveMe", 1f);
        yield return null;
    }

    // Removes the GameObject from the scene from an Invoke call.
    private void RemoveMe()
    {
        Destroy(gameObject);
    }

}
