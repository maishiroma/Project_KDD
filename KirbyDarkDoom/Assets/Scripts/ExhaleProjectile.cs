/*  This script handles the star projectile movement and how it destroys enemies.
 * 
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
        projectileRB.AddForce(Vector2.right * moveSpeed);
    }

    // If the star hits an enemy or wall, it will dissapear
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.gameObject.layer == LayerMask.NameToLayer("Indestructable"))
        {
            DestroyProjectile();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Destructable"))
        {
            if(collision.tag == "Enemy" || collision.tag == "Block")
            {
                // The target is defeated!
                collision.gameObject.SetActive(false);
                DestroyProjectile();
            }
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
