/*  This script handles traveling between areas
 *  Genrrally, what happens is when the player looks up in front of a door, a transition will play and the player will then
 *  be warped to another position on the level, as depicted by the travelSpot
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorActions : MonoBehaviour {

    [Header("General Variables")]
    public bool isGoalDoor = false;
    public bool makePlayerFaceRight = true;
    [Range(1f,2f)]
    public float fadeTime = 1f;
    [Range(0.01f, 1f)]
    public float lerpValue = 0.1f;

    [Header("New Camera Modifiers")]
    public float cameraMinX;
    public float cameraMaxX;
    public float cameraMinY;
    public float cameraMaxY;

    [Header("Next Area References")]    // NOTE: These must be GameObjects that contain only the given GameObjects
    public GameObject nextAreaEnemies;
    public GameObject nextAreaBlocks;

	[Header("Outside References")]
    public CameraController mainCamera;
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
        GameManager.Instance.PauseGame();
        playerHealth.isInvincible = true;
        isFading = true;
        yield return new WaitForSeconds(fadeTime);

        // If we are a goal door, we move to a new cutscene
        if(isGoalDoor == true)
        {
            GameManager.Instance.GoToGoalScene();
            yield return null;
        }
        // Else, we proceed to the normal transition
        else
        {
            // We then teleport the player, change their respawn point
            playerRB.position = travelSpot.position;
            playerHealth.spawnLocation = travelSpot.position;
            playerController.ResetPlayerMovement(makePlayerFaceRight);
            yield return new WaitForFixedUpdate();

            // We set the camera to have new bounds so that it will properly show the player
            mainCamera.minXPos = cameraMinX;
            mainCamera.maxXPos = cameraMaxX;
            mainCamera.minYPos = cameraMinY;
            mainCamera.maxYPos = cameraMaxY;
            yield return new WaitForFixedUpdate();

            // We despawn all of the previous area's enemies and blocks
            GameManager.Instance.DefeatAssociatedEnemies();
            GameManager.Instance.DefeatAssociaedBlocks();
            yield return new WaitForFixedUpdate();

            // And spawn the new area's enemies and blocks
            GameManager.Instance.AssociateNewEnemyAndBlocks(nextAreaEnemies, nextAreaBlocks);
            GameManager.Instance.RespawnAssociatedEnemies();
            GameManager.Instance.RespawnAssociatedBlocks();
            yield return new WaitForFixedUpdate();

            // Then we enable the player to move and start fading back in
            GameManager.Instance.ResumeGame(false, false);
            playerHealth.isInvincible = false;
            isFading = false;
            yield return new WaitForSeconds(fadeTime);

            // We tell this invoke we are done!
            isTraveling = false;
            yield return null;
        }
    }

    
}
