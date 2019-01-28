/*  This is the definition for enemies and their health. WIP
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyHealth : BaseHealth
{
    // For now, we just deactivate the enemy when they lose all of their health
    public override void DyingAction()
    {
        gameObject.SetActive(false);
    }
}
