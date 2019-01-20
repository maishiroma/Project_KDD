/*  This script handles player movement and all collisions with the player
 */
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Public Variables
    [Header("General Variables")]
    public float moveSpeed = 20f;
    public float jumpPower = 60f;
    public float flyingGravity = 0.5f;
    public float duckOffset = -0.22f;
    public float duckHeight = 0.5f;
    [Range(0.1f, 1f)]
    public float slideRange = 0.1f;
    [Range(0.5f, 5f)]
    public float flyHeightModifier = 2f;
    [Range(0.1f,1f)]
    public float verticalGainDuration = 0.3f;

    [Header("States")]
    public bool isFacingRight = true;
    public bool isDucking = false;
    public bool isSliding = false;
    public bool isInAir = false;
    public bool isJumping = false;
    public bool isFlying = false;
    public bool isInhaling = false;
    public bool isStuffed = false;
    public bool isExhaling = false;
    public bool isLanding = false;
    public bool isTakingDamage = false;

    [Header("Component References")]
    public BoxCollider2D playerCollider;
    public Rigidbody2D playerRB;
    public PlayerGraphics playerGraphics;
    public PlayerHealth playerHealth;

    [Header("Outside References")]
    public GameObject inhaleHitboxChild;
    public GameObject slideHitboxChild;
    public GameObject exhaleStarPrefab;
    public GameObject airPuffPrefab;
    public Transform[] groundCheckers = new Transform[3];

    // Private variables
    private float origGravity = 0f;
    private float currHorizSpeed = 0f;
    private float horizInput = 0f;
    private float jumpInput = 0f;
    private float inhaleHitboxXPos = 0f;
    private float slideHitboxXPos = 0f;
    private float origPlayerHeight = 0f;
    private bool canExhale = true;
    private bool isMovingUpwards = false;

    // Saves some of the private variables using the passed in GameObjects
    void Start()
    {
        inhaleHitboxXPos = inhaleHitboxChild.transform.localPosition.x;
        slideHitboxXPos = slideHitboxChild.transform.localPosition.x;
        origPlayerHeight = playerCollider.size.y;
        origGravity = playerRB.gravityScale;

        // If the player starts out in the air, we set the state of jumping to be true
        if(CheckGrounded() == false)
        {
            isInAir = true;
        }
    }

    // Receives the input from the player
    private void Update()
    {
        // Graphics Check
        GraphicUpdate();

        // If the player is exhaling or dying, they cannot do any actions
        if(isExhaling == false && playerHealth.IsDying == false)
        {
            // If the player is inhaling or ducking, they cannot move or jump
            if(isInhaling == false && isDucking == false)
            {
                JumpMovement();
                HorizontalMovement();
            }

            // If the player is ducking, they cannot inhale or exhale
            if(isDucking == false)
            {
                InhaleExhaleAction();
            }

            // If the player is not in the air, they can duck
            if(isInAir == false)
            {
                Ducking();
            }
        }
    }

    // Handles the movement for the player
    private void FixedUpdate()
    {
        // The player will only move if they are neither exhaling or inhaling (Or if they are dying/got hit)
        if(isExhaling == false && isInhaling == false  && isTakingDamage == false && playerHealth.IsDying == false)
        {
            if(isDucking == false)
            {
                // Horizontal movement
                playerRB.AddForce(transform.right * horizInput);

                // Jumping and Flying
                if(isMovingUpwards == true)
                {
                    if(isFlying == true)
                    {
                        // If the player is in the air, they can do "mini" jumps
                        playerRB.AddForce(Vector2.ClampMagnitude(transform.up * jumpPower, jumpPower / 3f));
                    }
                    else if(isJumping == true)
                    {
                        // Normal jumping
                        playerRB.AddForce(Vector2.ClampMagnitude(transform.up * jumpPower, jumpPower));
                    }
                }
            }
            else if(isSliding == true)
            {
                // Sliding movement
                if(isInAir == false)
                {
                    if(isFacingRight == true)
                    {
                        playerRB.MovePosition(playerRB.position + new Vector2(slideRange,0));
                    }
                    else
                    {
                        playerRB.MovePosition(playerRB.position + new Vector2(-slideRange,0));
                    }
                }
                else
                {
                    // If the player slides off the edge, their slide is cancelled
                    CancelInvoke("StopSliding");
                    StopSliding();
                    isDucking = false;
                }
            }
        }
    }

    // Handles if the player hits an enemy or any specific objects
	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.tag == "Enemy" && isTakingDamage == false)
        {
            playerHealth.TakeDamage(collision.gameObject.GetComponent<BaseEnemy>().attackPower);
            playerHealth.ActivateInvincibility();
            isTakingDamage = true;
            playerGraphics.ChangeSprite("isDamaged");
            Invoke("StopDamageLook", 0.5f);
        }
	}

	// Checks if the player is grounded
	private void OnCollisionStay2D(Collision2D collision)
	{
        if(CheckGrounded() == true  && isInAir == true)
        {
            isJumping = false;
            isInAir = false;

            // If the player is flying, they automatically do an airpuff
            if(isFlying == true)
            {
                GameObject spawned = Instantiate(airPuffPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                playerGraphics.ChangeSprite("isAirPuffing");
                isExhaling = true;
                if(isFacingRight == false)
                {
                    spawned.GetComponent<SpriteRenderer>().flipX = true;
                }
                Invoke("StopFlying", 0.1f);
            }
            else if(isInhaling == false)
            {
                // Unless the player is inhaling, they will do a quick landing animation
                isLanding = true;
                if(IsInvoking("StopLandingAnimation") == false)
                {
                    Invoke("StopLandingAnimation", 0.1f);
                }
            }
        }
	}

    // Checks to see if the player is airborn
	private void OnCollisionExit2D(Collision2D collision)
	{
        if(CheckGrounded() == false && isInAir == false)
        {
            isInAir = true;
        }
	}

	// Updates the player's graphic according
	private void GraphicUpdate()
    {
        // If the player is dying, they will only show that animation
        if(playerHealth.IsDying)
        {
            playerGraphics.ChangeSprite("isDead");
        }
        // If the player is exhaling, or inhaling, their sprite will not be updated
        else if(isExhaling == false && isInhaling == false && isTakingDamage == false)
        {
            if(isStuffed == true)
            {
                playerGraphics.ChangeSprite("isStuffed");
            }
            else if(isInAir == false)
            {
                // This takes priority over all other graphics
                if(isLanding == true)
                {
                    playerGraphics.ChangeSprite("isLanding");
                }
                // is the player sliding?
                else if(isSliding == true)
                {
                    playerGraphics.ChangeSprite("isSliding");
                }
                // Is the player ducking?
                else if(isDucking == true)
                {
                    playerGraphics.ChangeSprite("isDucking");
                }
                // Is the player moving?
                else if(playerRB.velocity.x < -0.1f || playerRB.velocity.x > 0.1f)
                {
                    playerGraphics.ChangeSprite("isMoving");
                }
                else
                {
                    playerGraphics.ChangeSprite("isIdle");
                }
            }
            else
            {
                // Is the player puffing?
                if(isFlying == true)
                {
                    playerGraphics.ChangeSprite("isFlying");
                }
                // Is the player jumping?
                else if(isJumping == true)
                {
                    playerGraphics.ChangeSprite("isJumping");
                }
                else
                {
                    playerGraphics.ChangeSprite("isAirborn");
                }
            }
        }
    }

    // Handles moving left and right for the player
    private void HorizontalMovement()
    {
        if(isInAir == true)
        {
            horizInput = Input.GetAxis("Horizontal") * (moveSpeed / 4f);
        }
        else
        {
            horizInput = Input.GetAxis("Horizontal") * moveSpeed;
        }

        // Rotates the player to face left
        if(horizInput < 0)
        {
            isFacingRight = false;
            playerGraphics.playerSprite.flipX = true;
            playerRB.MoveRotation(180f);
            inhaleHitboxChild.transform.localPosition = new Vector2(-inhaleHitboxXPos,inhaleHitboxChild.transform.localPosition.y);
            slideHitboxChild.transform.localPosition = new Vector2(-slideHitboxXPos,slideHitboxChild.transform.localPosition.y);
        }
        // Rotates the player to face right
        else if(horizInput > 0)
        {
            isFacingRight = true;
            playerGraphics.playerSprite.flipX = false;
            playerRB.MoveRotation(-180f);
            inhaleHitboxChild.transform.localPosition = new Vector2(inhaleHitboxXPos,inhaleHitboxChild.transform.localPosition.y);
            slideHitboxChild.transform.localPosition = new Vector2(slideHitboxXPos,slideHitboxChild.transform.localPosition.y);
        }
    }

    // The player ducks.
    private void Ducking()
    {
        if(Input.GetKey(KeyCode.S))
        {
            // If the player has something in their mouths, they will swallow it
            if(isDucking == false)
            {
                playerCollider.size = new Vector2(1,duckHeight);
                playerCollider.offset = new Vector2(0,duckOffset);
                isDucking = true;
                isStuffed = false;
                canExhale = true;
            }
            else
            {
                // If the player is ducking and hits inhale, they will slide
                if(Input.GetKeyDown(KeyCode.H) && !IsInvoking("StopSliding"))
                {
                    isSliding = true;
                    slideHitboxChild.SetActive(true);
                    Invoke("StopSliding", 0.2f);
                }
            }
        }
        else
        {
            if(isDucking == true)
            {
                playerCollider.size = new Vector2(1,origPlayerHeight);
                playerCollider.offset = new Vector2(0,0);
                isDucking = false;
            }
        }
    }

    // Handles the logic of how the player can jump and 'puff' in the air
    private void JumpMovement()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isInAir == false)
            {
                // If the player is grounded, they do a standard jump
                isJumping = true;
                isMovingUpwards = true;

                // This is done so that the player will stop moving upward after X seconds
                if(IsInvoking("StopVerticalIncrease") == false)
                {
                    Invoke("StopVerticalIncrease", verticalGainDuration);
                }
            }
            else
            {
                // The player cannot fly if they inhaled something
                if(isStuffed == false)
                {
                    isJumping = false;
                    isFlying = true;
                    isMovingUpwards = true;
                    playerRB.gravityScale = flyingGravity;

                    // This is done so that the player will stop moving upward after X seconds
                    if(IsInvoking("StopVerticalIncrease") == false)
                    {
                        Invoke("StopVerticalIncrease", verticalGainDuration);
                    }
                }
            }
        }
    }

    // Handles the logic for inhaling and exhaling
    private void InhaleExhaleAction()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            // These actions are only allowed once canExhale is true
            if(canExhale == true)
            {
                GameObject spawned = null;
                // Exhales out the enemy that the player has in their mouth
                if(isStuffed == true)
                {
                    spawned = Instantiate(exhaleStarPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                    playerGraphics.ChangeSprite("isExhaling");
                    isStuffed = false;
                    isExhaling = true;
                    Invoke("ResetExhaleState", 0.3f);
                }
                // Exhales out an airpuff if the player is flying
                else if(isFlying == true)
                {
                    spawned = Instantiate(airPuffPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                    playerGraphics.ChangeSprite("isAirPuffing");
                    isExhaling = true;
                    Invoke("StopFlying", 0.1f);
                }

                // Makes sure the projectile is facing in the direction the player is facing
                if(spawned != null)
                {
                    if(isFacingRight == false)
                    {
                        spawned.GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
            }
        }
        else if(Input.GetKey(KeyCode.H))
        {
            // This occurs immediatly as soon as the player inhaled an enemy
            if(isStuffed == true)
            {
                // We prevent the player from immediatly activating the exhale
                canExhale = false;
                Invoke("EnableExhale", 0.1f);
                return;
            }
            else if(isInhaling == false)
            {
                // Activates the inhale
                playerGraphics.ChangeSprite("isInhaling");
                inhaleHitboxChild.SetActive(true);
                isInhaling = true;
            }
        }
        else
        {
            // Stop inhaling
            if(isInhaling == true)
            {
                playerGraphics.ChangeSprite("isIdle");
                inhaleHitboxChild.SetActive(false);
                isInhaling = false;
            }
        }
    }

    // Resets the graphic and state for exhaling in an Invoke
    private void ResetExhaleState()
    {
        isExhaling = false;
        playerGraphics.ChangeSprite("isIdle");
    }

    // Enables the exhale interaction from an Invoke
    private void EnableExhale()
    {
        canExhale = true;
    }

    // Called in an Invoke to reset the player from moving upward
    private void StopVerticalIncrease()
    {
        isJumping = false;
        isMovingUpwards = false;
    }

    // Called in an Invoke to make the player fall down
    private void StopFlying()
    {
        isFlying = false;
        isExhaling = false;
        playerRB.gravityScale = origGravity;
    }

    // Called in an Invoke to stop the animation for landing to happen
    private void StopLandingAnimation()
    {
        isLanding = false;
    }

    // Called in an Invoke to stop the player animation of taking damage
    private void StopDamageLook()
    {
        isTakingDamage = false;
    }

    // Stops the slide from an Invoke call
    private void StopSliding()
    {
        isSliding = false;
        slideHitboxChild.SetActive(false);
    }

    // Handles checking if two floats are equal. Returns false if they aren't equal
    private bool FloatEquality(float f1, float f2)
    {
        if(Mathf.Abs(f1 - f2) < 0.00001f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Checks if the player is grounded properly
    private bool CheckGrounded()
    {
        // If the player is jumping, we are not going to check if they are grounded because they WILL NOT be grounded during that
        if(isJumping == false)
        {
             // We just need to check if at least one of these checks are valid.
            for(int i = 0; i < 3; ++i)
            {
                RaycastHit2D hit = Physics2D.Raycast(groundCheckers[i].position, -Vector2.up, 0.1f);
                if(hit == true)
                {
                    if(hit.collider.gameObject.tag == "Ground" && hit.collider.gameObject.layer == LayerMask.NameToLayer("Indestructable"))
                    {
                        return true;
                    }
                    else if(hit.collider.gameObject.tag == "Block")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }
}
