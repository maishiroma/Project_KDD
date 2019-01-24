/*  This script handles traveling between areas
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorActions : MonoBehaviour {

    [Header("General Variables")]
    public bool makePlayerFaceRight = true;
    [Range(1f,2f)]
    public float fadeTime = 1f;
    [Range(0.01f, 1f)]
    public float lerpValue = 0.1f;

	[Header("Outside References")]
    public Transform travelSpot;
    public Image fadeOverlay;

    // Private Variables
    private float alpha = 0f;
    private bool isTraveling = false;
    private bool isFading = false;

	// Handles the core logic of the door cutscene
	private void Update()
	{
        if(isTraveling == true)
        {
            // Fading to black
            if(isFading == true)
            {
                alpha = Mathf.Lerp(alpha, 1, lerpValue);
            }
            // Fading to transparent
            else
            {
                alpha = Mathf.Lerp(alpha, 0, lerpValue);
            }
            // Updating the alpha of the fadeout
            Color newColor = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, alpha);
            fadeOverlay.color = newColor;
        }
	}

	// While the player is hovering over this and if they hit W, they will enter the door
	private void OnTriggerStay2D(Collider2D collision)
	{
        if(isTraveling == false && collision.gameObject.tag == "Player" && Input.GetKey(KeyCode.W))
        {
            isTraveling = true;
            StartCoroutine("TravelCutscene", collision.gameObject);
        }
	}

    // This is used to time the transition
    IEnumerator TravelCutscene(GameObject player)
    {
        // Getting the proper components
        Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
        PlayerController playerController = player.GetComponent<PlayerController>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        // We stop the player from moving and fade to black
        playerRB.isKinematic = true;
        playerRB.velocity = Vector2.zero;
        playerHealth.isInvincible = true;
        isFading = true;
        yield return new WaitForSeconds(fadeTime);

        // We then teleport the player
        playerRB.position = travelSpot.position;
        playerController.ResetPlayerMovement(makePlayerFaceRight);
        yield return new WaitForFixedUpdate();

        // Then we enable the player to move and start fading back in
        playerRB.isKinematic = false;
        playerHealth.isInvincible = false;
        isFading = false;
        yield return new WaitForSeconds(fadeTime);

        // We tell this invoke we are done!
        isTraveling = false;
        yield return null;
    }
}
