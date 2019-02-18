/*  This script handles player movement and all collisions with the player
 */

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Public Variables
    [Header("General Variables")]
    public float groundMoveSpeed = 20f;
    public float airMoveSpeed = 10f;
    public float jumpPower = 60f;
    public float flyPower = 30f;
    public float flyingGravity = 0.5f;
    public float duckOffset = -0.22f;
    public float duckHeight = 0.5f;
    public float highFallPower = 20f;
    [Range(0.1f, 1f)]
    public float slideSpeed = 0.1f;
    [Range(0.1f, 3f)]
    public float timeToHighFall = 1.5f;

    [Header("Invoke Timers")]
    [Range(0.1f,1f)]
    public float exhaleResetTimer = 0.3f;
    [Range(0.1f,1f)]
    public float resetDamageLookTimer = 0.5f;
    [Range(0.1f,1f)]
    public float jumpGainTimer = 0.3f; 
    [Range(0.1f,1f)]
    public float flyGainTimer = 0.2f;
    [Range(0.1f, 1f)]
    public float highFallBounceTimer = 0.2f;
    [Range(0.1f, 1f)]
    public float slideTimer = 0.2f;

    [Header("States")]
    public bool isFacingRight = true;
    public bool isDucking = false;
    public bool isSliding = false;
    public bool isInAir = false;
    public bool isHighFall = false;
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
    private Vector2 origVelocity = Vector2.zero;
    private float origGravity = 0f;
    private float horizInput = 0f;
    private float inhaleHitboxXPos = 0f;
    private float slideHitboxXPos = 0f;
    private float origPlayerHeight = 0f;
    private float fallAirTime = 0f;
    private bool canExhale = true;
    private bool isMovingUpwards = false;

    // Resets the player movement so that they are in their initial state
    public void ResetPlayerMovement(bool faceRight)
    {
        // Reorientate player based on passed in boolean
        if(faceRight == false)
        {
            isFacingRight = false;
            playerGraphics.playerSprite.flipX = true;
            playerRB.MoveRotation(180f);
            inhaleHitboxChild.transform.localPosition = new Vector2(-inhaleHitboxXPos,inhaleHitboxChild.transform.localPosition.y);
            slideHitboxChild.transform.localPosition = new Vector2(-slideHitboxXPos,slideHitboxChild.transform.localPosition.y);
        }
        else
        {
            isFacingRight = true;
            playerGraphics.playerSprite.flipX = false;
            playerRB.MoveRotation(-180f);
            inhaleHitboxChild.transform.localPosition = new Vector2(inhaleHitboxXPos,inhaleHitboxChild.transform.localPosition.y);
            slideHitboxChild.transform.localPosition = new Vector2(slideHitboxXPos,slideHitboxChild.transform.localPosition.y);
        }

        // Depending on some states, we need to reset certain values
        if(isDucking == true)
        {
            playerCollider.size = new Vector2(1,origPlayerHeight);
            playerCollider.offset = new Vector2(0,0);
            isDucking = false;
        }
        if(isInhaling == true)
        {
            inhaleHitboxChild.SetActive(false);
            isInhaling = false;
        }

        // Calls all of the invoke methods to make sure all of the states are reset
        ResetExhaleState();
        EnableExhale();
        StopVerticalIncrease();
        StopFlying();
        StopLandingAnimation();
        StopDamageLook();
        StopSliding();
        ResetHighFall();

        // We will presume the player is in the air
        isInAir = true;

        // And nothing is in the player's mouth
        isStuffed = false;

        // And reset their speed to be 0
        playerRB.velocity = Vector2.zero;
    }

    // This stops the player from moving, saving its original velocity
    public void StopPlayer()
    {
        origVelocity = playerRB.velocity;
        this.enabled = false;
        playerRB.isKinematic = true;
        playerRB.velocity = Vector2.zero;
    }

    // This resumes player movement, with an option to use its orignal velocity
    public void ResumePlayer(bool useOrigVelocity)
    {
        if(useOrigVelocity)
        {
            playerRB.velocity = origVelocity;
        }
        this.enabled = true;
        playerRB.isKinematic = false;
    }

    // Saves some of the private variables using the passed in GameObjects
    private void Start()
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
        if(isExhaling == false && playerHealth.IsDying == false && isTakingDamage == false)
        {
            // If the player is inhaling or ducking, they cannot move or jump
            if(isInhaling == false && isDucking == false)
            {
                JumpMovement();
                HorizontalMovement();
                Falling();
            }

            // If the player is ducking, they cannot inhale or exhale
            if(isDucking == false)
            {
                InhaleExhaleAction();
            }

            // If the player is not in the air and already inhaling, they can duck
            if(isInAir == false && isInhaling == false)
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
                        playerRB.AddForce(Vector2.ClampMagnitude(transform.up * flyPower, flyPower));
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
                if(isFacingRight == true)
                {
                    playerRB.MovePosition(playerRB.position + new Vector2(slideSpeed,0));
                }
                else
                {
                    playerRB.MovePosition(playerRB.position + new Vector2(-slideSpeed,0));
                }
            }
        }
    }

    // Handles if the player hits an enemy or any specific objects
	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.tag == "Enemy")
        {
            if(isHighFall == true && playerRB.position.y > collision.gameObject.GetComponent<Rigidbody2D>().position.y)
            {
                // The player damages the enemy only if they are in a high fall and are above the enemy
                collision.gameObject.GetComponent<NormalEnemyHealth>().TakeDamage(highFallPower);
                ResetHighFall();
                isJumping = true;
                isMovingUpwards = true;
                Invoke("StopVerticalIncrease", highFallBounceTimer);
            }
            else
            {
                // The player takes damage accordingly
                PlayerHurt(collision.gameObject.GetComponent<BaseEnemy>().attackPower);
            }
        }
	}

	// Checks if the player is grounded or if they are constantly touching an enemy
	private void OnCollisionStay2D(Collision2D collision)
	{
        if(CheckGrounded() == true  && isInAir == true)
        {
            if(isHighFall == true)
            {
                // If the player lands on the ground while in a high fall, they will do a short hop
                ResetHighFall();
                isJumping = true;
                isMovingUpwards = true;
                Invoke("StopVerticalIncrease", highFallBounceTimer);
            }
            else
            {
                isJumping = false;
                isInAir = false;

                // If the player is flying, they automatically do an airpuff
                if(isFlying == true && isExhaling == false)
                {
                    GameObject spawned = Instantiate(airPuffPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                    playerGraphics.ChangeSprite("isAirPuffing");
                    isExhaling = true;
                    if(isFacingRight == false)
                    {
                        spawned.GetComponent<SpriteRenderer>().flipX = true;
                    }
                    Invoke("StopFlying", exhaleResetTimer);
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
        else if(CheckGrounded() == true)
        {
            // If the player is grounded and is ducking on a PassBothGround platform, they will go through it
            if(collision.gameObject.tag == "PassBothGround" && isDucking == true)
            {
                StartCoroutine("ResetPassBothPlatform", collision.gameObject.GetComponent<PlatformEffector2D>());
            }
        }

        if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "MiniBoss")
        {
            // The player takes damage accordingly
            PlayerHurt(collision.gameObject.GetComponent<BaseEnemy>().attackPower);
        }
	}

    // Checks to see if the player is airborn
	private void OnCollisionExit2D(Collision2D collision)
	{
        if(CheckGrounded() == false && isInAir == false)
        {
            isInAir = true;

            if(isSliding == true)
            {
                // If the player slides off the edge, their slide is cancelled
                CancelInvoke("StopSliding");
                StopSliding();
                isDucking = false;
            }
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
                // Landing takes priority over all other graphics
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
                // Is the player in a long fall?
                if(isHighFall == true)
                {
                    playerGraphics.ChangeSprite("isHighFall");
                }
                // Is the player puffing?
                else if(isFlying == true)
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
            horizInput = Input.GetAxis("Horizontal") * airMoveSpeed;
        }
        else
        {
            horizInput = Input.GetAxis("Horizontal") * groundMoveSpeed;
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
                    Invoke("StopSliding", slideTimer);
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
                    Invoke("StopVerticalIncrease", jumpGainTimer);
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
                        Invoke("StopVerticalIncrease", flyGainTimer);
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
            if(canExhale == true && isExhaling == false)
            {
                GameObject spawned = null;
                // Exhales out the enemy that the player has in their mouth
                if(isStuffed == true)
                {
                    spawned = Instantiate(exhaleStarPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                    playerGraphics.ChangeSprite("isExhaling");
                    isStuffed = false;
                    isExhaling = true;
                    Invoke("ResetExhaleState", exhaleResetTimer);
                }
                // Exhales out an airpuff if the player is flying
                else if(isFlying == true)
                {
                    spawned = Instantiate(airPuffPrefab, inhaleHitboxChild.transform.position, Quaternion.identity, gameObject.transform);
                    playerGraphics.ChangeSprite("isAirPuffing");
                    isExhaling = true;
                    Invoke("StopFlying", exhaleResetTimer);
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
                Invoke("EnableExhale", exhaleResetTimer);
                return;
            }
            else if(isInhaling == false)
            {
                // Activates the inhale
                ResetHighFall();
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

    // Handles the logic for falling long disitances
    private void Falling()
    {
        if(isInAir == true && isStuffed == false)
        {
            // If the player is falling, they will eventually go into a falling animation
            if(isJumping == false && isFlying == false)
            {
                fallAirTime += Time.deltaTime;
                if(fallAirTime > timeToHighFall && isHighFall == false)
                {
                    isHighFall = true;
                }
            }
            else
            {
                // When the player is flying or jumping, the meter resets
                ResetHighFall();
            }
        }
        else
        {
            // When the player is on the ground or stuffed, the meter resets
            ResetHighFall();
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

    // Resets the high falling status if the fallTimer hasn't already been reset
    private void ResetHighFall()
    {
        if(FloatEquality(fallAirTime, 0) == true)
        {
            isHighFall = false;
            fallAirTime = 0f;
        }
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

    // Tells the pass both platform that the player is on to reset itself
    IEnumerator ResetPassBothPlatform(PlatformEffector2D platform)
    {
        // We first tell the platform to detect everything but the player
        platform.colliderMask = ~(1 << LayerMask.NameToLayer("Player"));
        isInAir = true;
        isDucking = false;
        yield return new WaitForSeconds(0.5f);

        // After that, we tell the mask to detect everything again
        platform.colliderMask = -1;
        yield return null;
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
                    // If the player is passing through a permeable ground vertically, we do not consider them grounded
                    if((hit.collider.gameObject.tag == "PassGround" || hit.collider.gameObject.tag == "PassBothGround") && playerRB.velocity.y > 0)
                    {
                        return false;
                    }
                    else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Indestructable") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Destructable"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }

    // Helper method that handles all of the necessary logic when the player gets hurt
    public void PlayerHurt(float attackPower)
    {
        if(isTakingDamage == false && playerHealth.isInvincible == false)
        {
            // We first stop specific states if they are valid
            if(isDucking == true)
            {
                playerCollider.size = new Vector2(1,origPlayerHeight);
                playerCollider.offset = new Vector2(0,0);
                isDucking = false;
            }
            if(isInhaling == true)
            {
                inhaleHitboxChild.SetActive(false);
                isInhaling = false;
            }

            // The player then takes damage and are briefly invincible
            playerHealth.TakeDamage(attackPower);
            playerHealth.ActivateInvincibility();
            isTakingDamage = true;
            playerGraphics.ChangeSprite("isDamaged");
            Invoke("StopDamageLook", resetDamageLookTimer);
        }
    }
}
