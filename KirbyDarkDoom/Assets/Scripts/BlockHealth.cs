/* Determines what actions destructable blocks have.
 * Due to the similarities these have with live entitires (besides not hurting the player), this inherits from BaseHealth
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHealth : BaseHealth
{
    // For now, we just deactivate this block when it loses all of its health
    public override void DyingAction()
    {
        gameObject.SetActive(false);
    }
}
