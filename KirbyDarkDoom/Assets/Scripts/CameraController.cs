/* Controls how the camera moves with the player. 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header("Positioning Variables")]
    public Vector2 cameraOffset;
    public float minXPos;
    public float maxXPos;
    public float minYPos;
    public float maxYPos;

    // Private variables
    private Transform player;

    // Upon Activation, we look for the player to follow, if no player is set
    private void Awake()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    // We update the position of the camera based on the player's movement
	private void LateUpdate()
	{
        // We apply the initial position to be where the player is and add the offset from the player
        Vector2 newPos = (Vector2)player.transform.position + cameraOffset;

        // We then clamp the position to the min/max values we gathered
        gameObject.transform.position = new Vector2(Mathf.Clamp(newPos.x, minXPos, maxXPos), Mathf.Clamp(newPos.y, minYPos, maxYPos));
	}
}
