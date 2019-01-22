/*  This script handles all of the player graphic components, including sprite changes
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour {

    [Header("Component References")]
    public SpriteRenderer playerSprite;

    [Header("Sprite List")]
    public Sprite player_idle;
    public Sprite player_move;
    public Sprite player_sliding;
    public Sprite player_ducking;
    public Sprite player_inhale;
    public Sprite player_stuffed;
    public Sprite player_exhale;
    public Sprite player_jump;
    public Sprite player_fly;
    public Sprite player_airpuffed;
    public Sprite player_airborn;
    public Sprite player_land;
    public Sprite player_damaged;
    public Sprite player_dead;

    // Changes the sprite to whatever action is passed into here.
    public void ChangeSprite(string stateName)
    {
        switch(stateName)
        {
            case "isIdle":  // Idle on the ground
                playerSprite.sprite = player_idle;
                break;
            case "isDucking":    // Ducking on the ground
                playerSprite.sprite = player_ducking;
                break;
            case "isMoving":    // Moving on the ground
                playerSprite.sprite = player_move;
                break;
            case "isSliding": // Sliding on the ground
                playerSprite.sprite = player_sliding;
                break;
            case "isInhaling":  // When the player is inhaling
                playerSprite.sprite = player_inhale;
                break;
            case "isStuffed":   // When the player has something in their mouth
                playerSprite.sprite = player_stuffed;
                break;
            case "isExhaling":  // When the player is exhaling
                playerSprite.sprite = player_exhale;
                break;
            case "isJumping":   // When the player is jumping
                playerSprite.sprite = player_jump;
                break;
            case "isFlying":   // When the player is flying
                playerSprite.sprite = player_fly;
                break;
            case "isAirPuffing":   // When the player is exhaling an airpuff
                playerSprite.sprite = player_airpuffed;
                break;
            case "isAirborn":   // When the player is in the air not jumping or flying
                playerSprite.sprite = player_airborn;
                break;
            case "isLanding":  // landed on the ground
                playerSprite.sprite = player_land;
                break;
            case "isDamaged":  // When the player is hit
                playerSprite.sprite = player_damaged;
                break;
            case "isDead":  // When the player is dead, aka, lost all health
                playerSprite.sprite = player_dead;
                break;
            default:    // Prints out error if a state is misspelled or it doesn't exist
                print("I don't know what " + stateName + " is!");
                break;
        }
    }
}
