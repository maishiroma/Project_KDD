/*  This defines all of the parameters for the mini boss's health to be displayed on the screen
 *  Extends from BaseHealth, in order to simplify the proccess.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniBossHealth : BaseHealth
{
    [Header("Sub Outside References")]
    public BombMiniBossActions enemyActions;
    public EnemyGraphics enemyGraphics;
    public SelfDestroy explodeComponent;

    [Header("Sub UI Refs")]
    public TextMeshProUGUI bossName_UI;
    public Slider bossHealthBar_UI;

    // Shows the name and health bar of the boss when it is first spawned.
	private void OnEnable()
	{
        bossName_UI.gameObject.SetActive(true);
        bossHealthBar_UI.gameObject.SetActive(true);
        bossHealthBar_UI.maxValue = maxHealth;
	}

	//Updates the boss's health bar to the screen
	private void Update()
	{
        bossHealthBar_UI.value = CurrentHealth;
	}

    // Does the same behavior, except it resets the boss's positioning and behavior
	public override void Respawn()
	{
        base.Respawn();
        enemyActions.ResetBehavior();
	}

	// Once the boss is dead, it will stop moving. After X seconds it will then explode.
	public override void DyingAction()
    {
        if(!IsInvoking("Dissapear"))
        {
            bossName_UI.gameObject.SetActive(false);
            bossHealthBar_UI.gameObject.SetActive(false);
            enemyActions.CurrentState = EnemyStates.DEFEAT;
            canBeInhaled = true;
            explodeComponent.enabled = true;
        }
    }
}
