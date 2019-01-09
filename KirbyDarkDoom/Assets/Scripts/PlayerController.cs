/*  This script handles the player movement. WIP
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Public Variables
    [Header("General Variables")]
    public float moveSpeed = 20f;
    public float jumpPower = 60f;
    public float duckOffset = -0.22f;
    public float duckHeight = 0.5f;
    [Range(0.1f, 2f)]
    public float flyModifer = 1.5f;
    [Range(0.1f,1f)]
    public float jumpFlyLength = 0.3f;

    [Header("States")]
    public bool isDucking = false;
    public bool isInAir = false;
    public bool isJumping = false;
    public bool isFlying = false;
    public bool isInhaling = false;
    public bool isStuffed = false;
    public bool isExhaling = false;

    [Header("Component References")]
    public BoxCollider2D playerCollider;
    public Rigidbody2D playerRB;
    public PlayerGraphics playerGraphics;

    [Header("Outside References")]
    public GameObject inhaleHitboxChild;
    public GameObject exhaleStarPrefab;

    // Private variables
    private float currHorizSpeed = 0f;
    private float horizInput = 0f;
    private float jumpInput = 0f;
    private float inhaleHitboxXPos = 0f;
    private float origPlayerHeight = 0f;
    private bool canExhale = true;

    // Saves some of the private variables using the passed in GameObjects
    void Start()
    {
        inhaleHitboxXPos = inhaleHitboxChild.transform.position.x;
        origPlayerHeight = playerCollider.size.y;

        // If the player starts out in the air, we set the state of jumping to be true
        if(floatEquality(playerRB.velocity.y, 0) == true)
        {
            isJumping = true;
        }
    }

    // Receives the input from the player here
    private void Update()
    {
        // Graphics Check
        GraphicUpdate();

        // Checks if the player just landed on the ground
        if(isInAir == true && floatEquality(playerRB.velocity.y, 0) == false)
        {
            isJumping = false;
            isFlying = false;
        }
        isInAir = floatEquality(playerRB.velocity.y, 0);

        // If the player is exhaling, they cannot do any actions
        if(isExhaling == false)
        {
            // If the player is inhaling or ducking, they cannot move or jump
            if(isInhaling == false)
            {
                if(isDucking == false)
                {
                    JumpMovement();
                    HorizontalMovement();
                }
                Ducking();
            }
            InhaleExhaleAction();
        }
    }

    // Handles the movement for the player
    private void FixedUpdate()
    {
        // The player will only move if they are neither exhaling or inhaling
        if(isExhaling == false && isInhaling == false && isDucking == false)
        {
            // Horizontal movement
            playerRB.AddForce(transform.right * horizInput);

            // Jumping and Flying
            if(isFlying == true)
            {
                // If the player is in the air, they can do "mini" jumps
                playerRB.AddForce(Vector2.ClampMagnitude(transform.up * jumpPower, jumpPower / flyModifer));
            }
            else if(isJumping == true)
            {
                // Normal jumping
                playerRB.AddForce(Vector2.ClampMagnitude(transform.up * jumpPower, jumpPower));
            }
        }
    }

	// Updates the player's graphic according
	private void GraphicUpdate()
    {
        // If the player is stuffed, exhaling, or inhaling, their sprite will not be updated
        if(isStuffed == false && isExhaling == false && isInhaling == false)
        {
            if(isInAir == false)
            {
                // Is the player ducking?
                if(isDucking == true)
                {
                    playerGraphics.ChangeSprite("isDucking");
                }
                // Is the player moving?
                else if(horizInput < -0.1f || horizInput > 0.1f)
                {
                    playerGraphics.ChangeSprite("isMoving");
                }
                else
                {
                    playerGraphics.ChangeSprite("isIdle");
                } 
            }
            else
            {
                if(isFlying == true)
                {
                    playerGraphics.ChangeSprite("isFlying");
                }
                else if(isJumping == true)
                {
                    playerGraphics.ChangeSprite("isJumping");
                }
                else
                {
                    playerGraphics.ChangeSprite("isAirborn");
                }
            }
        }
    }

    // Handles moving left and right for the player
    private void HorizontalMovement()
    {
        if(isInAir == true)
        {
            horizInput = Input.GetAxis("Horizontal") * (moveSpeed / 3f);
        }
        else
        {
            horizInput = Input.GetAxis("Horizontal") * moveSpeed;
        }

        if(horizInput < 0)
        {
            // Rotates the player to face right
            playerGraphics.playerSprite.flipX = true;
            playerRB.MoveRotation(180f);
            inhaleHitboxChild.transform.localPosition = new Vector2(-inhaleHitboxXPos,0f);
        }
        else if(horizInput > 0)
        {
            // Rotates the player to face left
            playerGraphics.playerSprite.flipX = false;
            playerRB.MoveRotation(-180f);
            inhaleHitboxChild.transform.localPosition = new Vector2(inhaleHitboxXPos,0f);
        }
    }

    // The player ducks. 
    // If they have something in their mouth, it dissapears (for now)
    private void Ducking()
    {
        if(Input.GetKey(KeyCode.S))
        {
            if(isDucking == false)
            {
                playerCollider.size = new Vector2(1,duckHeight);
                playerCollider.offset = new Vector2(0,duckOffset);
                isDucking = true;
                isStuffed = false;
                canExhale = true;
            }
        }
        else
        {
            if(isDucking == true)
            {
                playerCollider.size = new Vector2(1,origPlayerHeight);
                playerCollider.offset = new Vector2(0,0);
                isDucking = false;
            }
        }
    }

    // Handles the logic of how the player can jump and 'puff' in the air
    private void JumpMovement()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isInAir == false)
            {
                // If the player is grounded, they do a standard jump
                isJumping = true;

                // This is done so that the player will stop moving upward after X seconds
                if(IsInvoking("StopJumpFlyVertical") == false)
                {
                    Invoke("StopJumpFlyVertical", jumpFlyLength);
                }
            }
            else
            {
                // The player cannot fly if they inhaled something 
                if(isStuffed == false)
                {
                    isJumping = false;
                    isFlying = true;

                    // This is done so that the player will stop moving upward after X seconds
                    if(IsInvoking("StopJumpFlyVertical") == false)
                    {
                        Invoke("StopJumpFlyVertical", jumpFlyLength);
                    }
                }
            }
        }
    }

    // Handles the logic for inhaling and exhaling
    private void InhaleExhaleAction()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            // Exhale out a projectile if the player inhaled an object
            // This action is only allowed once canExhale is true
            if(isStuffed == true && canExhale == true)
            {
                Instantiate(exhaleStarPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                playerGraphics.ChangeSprite("isExhaling");
                isStuffed = false;
                isExhaling = true;
                Invoke("ResetExhaleState", 0.3f);
            }
        }
        else if(Input.GetKey(KeyCode.H))
        {
            // This occurs immediatly as soon as the player inhaled an enemy
            if(isStuffed == true)
            {
                // We prevent the player from immediatly activating the exhale
                canExhale = false;
                Invoke("EnableExhale", 0.1f);
                return;
            }
            else if(isInhaling == false)
            {
                // Activates the inhale
                playerGraphics.ChangeSprite("isInhaling");
                inhaleHitboxChild.SetActive(true);
                isInhaling = true;
            }
        }
        else
        {
            // Stop inhaling
            if(isInhaling == true)
            {
                playerGraphics.ChangeSprite("isIdle");
                inhaleHitboxChild.SetActive(false);
                isInhaling = false;
            }
        }
    }

    // Handles checking if two floats are equal
    // Returns false if they aren't equal
    private bool floatEquality(float f1, float f2)
    {
        if(Mathf.Abs(f1 - f2) < 0.00001f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Resets the graphic and state for exhaling
    private void ResetExhaleState()
    {
        isExhaling = false;
        playerGraphics.ChangeSprite("isIdle");
    }

    // Enables the exhale interaction
    private void EnableExhale()
    {
        canExhale = true;
    }

    // Called in an Invoke to reset the player from moving upward
    private void StopJumpFlyVertical()
    {
        isJumping = false;
        isFlying = false;
    }
}
