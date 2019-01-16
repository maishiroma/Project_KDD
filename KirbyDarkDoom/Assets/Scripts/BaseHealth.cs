/*  This is the base health mechanic that all entities in the game will inherit from.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHealth : MonoBehaviour {

	[Header("Base Variables")]
    public float maxHealth;
    public float currentHealth;
    public bool isInvincible = false;
    public float invincibilityTime = 2f;

    [Header("Base Components")]
    public SpriteRenderer entitySpriteRender;

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
	}

    // Returns true if the current health < 0
    public bool CheckIfAlive()
    {
        return currentHealth > 0;
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

    // This method is called to prevent the entity from taking damage for 
    public void ActivateInvincibility()
    {
        if(isInvincible == false)
        {
            isInvincible = true;
            Invoke("ResetInvincibility", invincibilityTime);
            InvokeRepeating("FlashSprite",0f, 0.1f);
        }
    }

    // Called in an Invoke to reset the entity's invincibility
    private void ResetInvincibility()
    {
        isInvincible = false;
        CancelInvoke("FlashSprite");
    }

    // Called in an Invoke Repeating to simulate player invincibility
    private void FlashSprite()
    {
        Color entitySprite = entitySpriteRender.color;
        if(entitySprite.a == 1f)
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
