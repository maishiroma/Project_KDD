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
    public float horizAcceletation = 1f;
    public float horizMaxSpeed = 10f;
    public float jumpPower = 700f;
    [Range(0.1f, 2f)]
    public float flyModifer = 1.5f;
    [Range(0.1f,1f)]
    public float lerpValue = 0.1f;

    [Header("States")]
    public bool isInAir = false;
    public bool isJumping = false;
    public bool isFlying = false;
    public bool isInhaling = false;
    public bool isStuffed = false;
    public bool isExhaling = false;

    [Header("Component References")]
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
    private bool canExhale = true;

    // Saves some of the private variables using the passed in GameObjects
    void Start()
    {
        inhaleHitboxXPos = inhaleHitboxChild.transform.position.x;

        // If the player starts out in the air, we set the state of jumping to be true
        if(VerifyIfAirborn())
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
        if(isInAir == true && VerifyIfAirborn() == false)
        {
            isJumping = false;
            isFlying = false;
        }
        isInAir = VerifyIfAirborn();

        // If the player is exhaling, they cannot do any actions
        if(isExhaling == false)
        {
            // If the player is inhaling, they cannot move or jump
            if(isInhaling == false)
            {
                JumpMovement();
                HorizontalMovement();
            }
            InhaleExhaleAction();
        }
    }

    // Handles the movement for the player
    private void FixedUpdate()
    {
        // The player will only move if they are neither exhaling or inhaling
        if(isExhaling == false && isInhaling == false)
        {
            // Horizontal movement
            playerRB.AddForce(transform.right * currHorizSpeed);

            // Jumping and Flying
            if(isFlying == true)
            {
                // If the player is in the air, they can do "mini" jumps
                // This throttles the height the player gets if they spam the jump button
                if(playerRB.velocity.y < 0)
                {
                    playerRB.AddForce(Vector2.ClampMagnitude(transform.up * (jumpPower * jumpInput), jumpPower / flyModifer));
                }
            }
            else if(isJumping == true)
            {
                // Normal jumping
                playerRB.AddForce(Vector2.ClampMagnitude(transform.up * (jumpPower * jumpInput), jumpPower));
            }
        }
    }

	// Updates the player's graphic according
	private void GraphicUpdate()
    {
        // If the player is stuffed, exhaling, or inhaling, their sprite will not be updated
        if(isStuffed == false && isExhaling == false && isInhaling == false)
        {
            // Checks if the player just landed on the ground
            if(isInAir == true && VerifyIfAirborn() == false)
            {
                playerGraphics.ChangeSprite("isLanding");
            }
            else if(isInhaling == false)
            {
                if(isInAir == false)
                {
                    // Is the player moving?
                    if(horizInput < -0.1f || horizInput > 0.1f)
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
                        playerGraphics.ChangeSprite("isJumping");
                    }
                }
            }
        }
    }

    // Check if player is in the air
    private bool VerifyIfAirborn()
    {
        // We needed to do this check since doubles are a bit finicky with equality checks
        if(Mathf.Abs(playerRB.velocity.y - 0) < 0.00001f)
        {
            return false;
        }
        else
        {
            return true;   
        }

    }

    // Handles moving left and right for the player
    private void HorizontalMovement()
    {
        if(Input.GetKey(KeyCode.D))
        {
            // If the player is moving right
            horizInput += Mathf.Lerp(0, horizMaxSpeed, lerpValue);
            horizInput = Mathf.Clamp(horizInput, 0f, horizMaxSpeed);

            // Rotates the player to face right
            if(playerRB.rotation >= 0)
            {
                playerGraphics.playerSprite.flipX = false;
                playerRB.MoveRotation(-180f);
                inhaleHitboxChild.transform.localPosition = new Vector2(inhaleHitboxXPos,0f);
            }
        }
        else if(Input.GetKey(KeyCode.A))
        {
            // If the player is moving left
            horizInput += Mathf.Lerp(0, -horizMaxSpeed, lerpValue);
            horizInput = Mathf.Clamp(horizInput, -horizMaxSpeed, 0f);

            // Rotates the player to face left
            if(playerRB.rotation <= 0)
            {
                playerGraphics.playerSprite.flipX = true;
                playerRB.MoveRotation(180f);
                inhaleHitboxChild.transform.localPosition = new Vector2(-inhaleHitboxXPos,0f);
            }
        }
        else
        {
            // If the player is standing still
            if(horizInput < -0.1f || horizInput > 0.1f)
            {
                horizInput = Mathf.Lerp(horizInput, 0f, lerpValue);
            }
            else
            {
                horizInput = 0f;
            }
        }

        // If the player is in the air, horizontal movement is halved
        if(isInAir == false)
        {
            currHorizSpeed = horizInput * horizAcceletation;
        }
        else
        {
            currHorizSpeed = (horizInput * horizAcceletation) / 2f;
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
                jumpInput = 1f;
            }
            else
            {
                // The player cannot fly if they inhaled something 
                if(isStuffed == false)
                {
                    isJumping = false;
                    isFlying = true;
                    jumpInput = 1f;
                }
                else
                {
                    jumpInput = 0f;
                }
            }
        }
        else
        {
            jumpInput = 0f;
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
                Invoke("ResetExhaleState", 0.5f);
            }
        }
        else if(Input.GetKey(KeyCode.H))
        {
            // This occurs immediatly as soon as the player inhaled an enemy
            if(isStuffed == true)
            {
                // We prevent the player from immediatly activating the exhale
                canExhale = false;
                Invoke("EnableExhale", 0.5f);
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
}
