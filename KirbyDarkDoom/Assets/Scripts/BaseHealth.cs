/*  This is the base health mechanic that all entities in the game will inherit from.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHealth : MonoBehaviour {

	[Header("Base Variables")]
    public float maxHealth;
    public bool isInvincible = false;
    public float invincibilityTime = 2f;

    [Header("Base Components")]
    public SpriteRenderer entitySpriteRender;

    // Private Variables
    private Vector2 spawnLocation;
    private float currentHealth;

    // Getters
    public float CurrentHealth {
        get {return currentHealth;}
    }

    // Makes sure the healths are at a valid amount
	private void OnValidate()
	{
        maxHealth = Mathf.Clamp(maxHealth, currentHealth, currentHealth + maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
	}

	// Sets up the current health to be equal to the max health
	private void Start()
	{
        currentHealth = maxHealth;
        spawnLocation = gameObject.transform.position;
	}

	// Call this method to decrease health
	public void TakeDamage(float damagePower)
    {
        if(isInvincible == false)
        {
            currentHealth -= damagePower;
            if(currentHealth <= 0)
            {
                DyingAction();
            }
        }
    }

    // Call this method to increase health
    public void RestoreHealth(float recoverAmount)
    {
        currentHealth += recoverAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // This method is called to prevent the entity from taking damage for X seconds
    public void ActivateInvincibility()
    {
        if(isInvincible == false && currentHealth > 0)
        {
            isInvincible = true;
            Invoke("ResetInvincibility", invincibilityTime);
            InvokeRepeating("FlashSprite",0f, 0.1f);
        }
    }

    // Respawns the entity with full health
    // Can be overriden
    public virtual void Respawn()
    {
        if(gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
        currentHealth = maxHealth;
        gameObject.transform.position = spawnLocation;
    }

    // Called in an Invoke to reset the entity's invincibility
    private void ResetInvincibility()
    {
        isInvincible = false;
        CancelInvoke("FlashSprite");
    }

    // Called in an Invoke Repeating to simulate an entity's invincibility
    private void FlashSprite()
    {
        Color entitySprite = entitySpriteRender.color;
        if(Mathf.Abs(entitySprite.a - 1f) < 0.00001f)
        {
            entitySpriteRender.color = new Color(entitySprite.r, entitySprite.g, entitySprite.b, 0.5f);
        }
        else
        {
            entitySpriteRender.color = new Color(entitySprite.r, entitySprite.g, entitySprite.b, 1f);
        }
    }

    // Required by all scripts who inherit from this to implement.
    public abstract void DyingAction();
}
