/*  This is used for changing the graphics of enemies.
 *  Identical to PlayerGraphics's functionalities, except less sprites
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGraphics : MonoBehaviour {

    [Header("External References")]
    public SpriteRenderer enemySprite;

	[Header("Graphic List")]
    public Sprite enemy_normal;
    public Sprite enemy_attack;
    public Sprite enemy_defeat;

    // When called, depending on the given state, changes the enemy's sprite
    public void SwitchSprite(string state)
    {
        switch(state)
        {
            case "normal":
                enemySprite.sprite = enemy_normal;
                break;
            case "attack":
                enemySprite.sprite = enemy_attack;
                break;
            case "defeat":
                enemySprite.sprite = enemy_defeat;
                break;
        }
    }
}
