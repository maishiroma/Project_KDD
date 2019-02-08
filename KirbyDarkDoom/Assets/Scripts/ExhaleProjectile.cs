/*  This script handles projectile movement and how it interacts with various entities.
 *  Currently, there's only two types of projectiles: exhale puffs and star projectiles.
 *  Each one has their own attributes and powers, as depicted by their prefab
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaleProjectile : MonoBehaviour {

	[Header("General Variables")]
    public float moveSpeed;
    public float timeToLive;
    public float damagePower;

    [Header("External References")]
    public Rigidbody2D projectileRB;

    // Upon creating, the star will have a set TTL and a specified move direction
    private void Start()
    {
        Invoke("DestroyProjectile", timeToLive);
        moveSpeed *= Mathf.Sign(transform.localPosition.x);
    }

	// The star will continuously move in a given direction
	private void FixedUpdate()
    {
        projectileRB.MovePosition(projectileRB.position + new Vector2(moveSpeed, 0));
    }

    // Actions that will happen when the star collides with something
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.gameObject.layer == LayerMask.NameToLayer("Indestructable"))
        {
            // Projectile is destroyed if it runs into an indestructible object
            DestroyProjectile();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Destructable"))
        {
            //If the object inherits from BaseHealth, we deal damage to it
            if(collision.gameObject.GetComponent<BaseHealth>() != null)
            {
                collision.gameObject.GetComponent<BaseHealth>().TakeDamage(damagePower);
            }
            DestroyProjectile();
        }
	}

    // Invoked to destroy the star if it is still around
    private void DestroyProjectile()
    {
        if(gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
