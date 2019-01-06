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

    [Range(0.1f,1f)]
    public float lerpValue = 0.1f;

    [Header("Component References")]
    public Rigidbody2D playerRB;
    public PlayerGraphics playerGraphics;

    [Header("Outside References")]
    public GameObject inhaleHitboxChild;
    public GameObject exhaleStarPrefab;

    // Private variables
    private float currHorizSpeed = 0f;
    private float horizInput = 0f;
    private float inhaleHitboxXPos = 0f;

    // Saves some of the private variables using the passed in GameObjects
    void Start()
    {
        inhaleHitboxXPos = inhaleHitboxChild.transform.position.x;
    }

    // Receives the input from the player here
    private void Update()
    {
        // State Check to make sure player is in the air
        playerGraphics.playerAnimator.SetBool("inAir", VerifyIfAirborn());

        // If the player is inhaling, they cannot move or jump
        if(playerGraphics.playerAnimator.GetBool("isInhaling") == false)
        {
            JumpMovement();
            HorizontalMovement();
        }

        InhaleExhaleAction();
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
        if(playerGraphics.playerAnimator.GetBool("inAir") == false)
        {
            currHorizSpeed = horizInput * horizAcceletation;
        }
        else
        {
            currHorizSpeed = (horizInput * horizAcceletation) / 2f;
        }

        playerRB.AddForce(transform.right * currHorizSpeed);
    }

    // Handles the logic of how the player can jump and 'puff' in the air
    private void JumpMovement()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(playerGraphics.playerAnimator.GetBool("inAir") == false)
            {
                // If the player is grounded, they do a standard jump
                playerRB.AddForce(transform.up * jumpPower);
                playerGraphics.ChangeSpriteAnimatorState("inAir_jump");
            }
            else
            {
                // The player cannot float if they inhaled something 
                if(playerGraphics.playerAnimator.GetBool("isStuffed") == false)
                {
                    // If the player is in the air, they can do "mini" jumps
                    playerRB.AddForce(Vector2.ClampMagnitude(transform.up * jumpPower, jumpPower / 1.5f));

                    // This throttles the height the player gets if they spam the jump button
                    if(playerRB.velocity.y > 0)
                    {
                        playerRB.AddForce(transform.up * -jumpPower / 2f);
                    }
                    playerGraphics.ChangeSpriteAnimatorState("inAir_fly");
                }
            }
        }
    }

    // Handles the logic for inhaling and exhaling
    private void InhaleExhaleAction()
    {
        if(Input.GetKeyDown(KeyCode.H))     // Exhale out a projectile if the player inhaled an object
        {
            if(playerGraphics.playerAnimator.GetBool("isStuffed") == true)
            {
                Instantiate(exhaleStarPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                playerGraphics.ChangeSpriteAnimatorState("normal");
            }
        }
        else if(Input.GetKey(KeyCode.H))        // Inhale
        {
            if(playerGraphics.playerAnimator.GetBool("isStuffed") == true)
            {
                return;
            }
            else if(playerGraphics.playerAnimator.GetBool("isInhaling") == false)
            {
                playerGraphics.ChangeSpriteAnimatorState("isInhaling");
                inhaleHitboxChild.SetActive(true);
            }
        }
        else  // Stop inhaling
        {
            if(playerGraphics.playerAnimator.GetBool("isInhaling") == true)
            {
                playerGraphics.ChangeSpriteAnimatorState("normal");
                inhaleHitboxChild.SetActive(false);
            }
        }
    }
}
