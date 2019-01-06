/*  This script handles all of the player graphic components, including sprite changes
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour {

    [Header("Component References")]
    public Animator playerAnimator;
    public SpriteRenderer playerSprite;

    [Header("Sprite List")]
    public Sprite player_normal;
    public Sprite player_inhale;
    public Sprite player_stuffed;
    public Sprite player_exhale;
    public Sprite player_jump;
    public Sprite player_fly;

    // Changes the sprite and animator to whatever action is passed into here.
    public void ChangeSpriteAnimatorState(string stateName)
    {
        switch(stateName)
        {
            case "normal":  // On the ground, no special action
                playerSprite.sprite = player_normal;
                playerAnimator.SetBool("isInhaling", false);
                playerAnimator.SetBool("isStuffed", false);
                break;
            case "landed":  // landed on the ground
                playerSprite.sprite = player_normal;
                break;
            case "isInhaling":  // When the player is inhaling
                playerSprite.sprite = player_inhale;
                playerAnimator.SetBool("isStuffed", false);
                playerAnimator.SetBool("isInhaling", true);
                break;
            case "inAir_jump":   // When the player is jumping
                playerSprite.sprite = player_jump;
                break;
            case "inAir_fly":   // When the player is flying
                playerSprite.sprite = player_fly;
                break;
            case "isStuffed":   // When the player has something in their mouth
                playerSprite.sprite = player_stuffed;
                playerAnimator.SetBool("isStuffed",true);
                playerAnimator.SetBool("isInhaling", false);
                break;
            default:    // Prints out error if a state is misspelled or it doesn't exist
                print("I don't know what " + stateName + " is!");
                break;
        }
    }
}
