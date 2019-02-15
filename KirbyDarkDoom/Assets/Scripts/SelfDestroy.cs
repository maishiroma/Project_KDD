/*  This script, when attatched to an object, will self destroy itself after X seconds.
 *  If toggled, this can also create a damaging hitbox upon destroying.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	[Header("General Variables")]
    public bool canExplode = false;
    public float timeToDestroy;

    [Header("Explosion Variables")]
    public Sprite explosionSprite;
    public float explosionPower;
    public float hitboxXSize;
    public float hitboxYSize;
    public float hitboxTimer;

    [Header("Outside References")]
    public BoxCollider2D hitboxRef;
    public Rigidbody2D objectRB;
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

    // Harms whatever is in the blast zone, if applicable
	private void OnTriggerStay2D(Collider2D collision)
	{
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().PlayerHurt(explosionPower);
        }
	}

	// Here, depending on what canCreateHitbox does, we either immedatly remove the gameobject, or create the hitbox
	IEnumerator DestroyLogic()
    {
        // We wait for the time to destroy the GameObject
        yield return new WaitForSeconds(timeToDestroy);

        if(canExplode == true)
        {
            // If we can create a hitbox, we set it active here
            objectSprite.sprite = explosionSprite;
            hitboxRef.isTrigger = true;
            hitboxRef.size = new Vector2(hitboxXSize, hitboxYSize);
            objectRB.isKinematic = true;
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

    // When selectign this object, draws out the blast zone radius
	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(hitboxXSize, hitboxYSize));
	}
}
