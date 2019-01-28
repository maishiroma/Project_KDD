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
        projectileRB.MovePosition(projectileRB.position + new Vector2(moveSpeed, 0));
    }

    // If the star hits an enemy or wall, it will dissapear
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.gameObject.layer == LayerMask.NameToLayer("Indestructable"))
        {
            // Projectile is destroyed if it runs into an indestructible object
            DestroyProjectile();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Destructable"))
        {
            if(collision.gameObject.tag == "Enemy")
            {
                // Enemy takes damage
                collision.gameObject.GetComponent<NormalEnemyHealth>().TakeDamage(damagePower);
            }
            else if(collision.gameObject.tag == "Block")
            {
                // The block is destroyed
                collision.gameObject.SetActive(false);
            }
            // Whatever the projectile runs into here, it will get destroyed
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
