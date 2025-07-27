using UnityEngine;

public class PlayerJumpingState : PlayerState
{
    private float groundedGracePeriod = 0.1f;
    private float timeSinceGrounded = 0f;

    public PlayerJumpingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        if (player.IsGrounded() || player.jumpCounter < (player.maxJumpCap ))
        {
            PerformJump();
        }
    }

    public override void Update()
    {
        // For Animation
        player.animator.SetBool("isJumping", !player.IsGrounded());

        // Coyote Time: Wait a short grace period before switching to idle
        if (player.IsGrounded())
        {
            timeSinceGrounded += Time.deltaTime;
            if (timeSinceGrounded >= groundedGracePeriod)
            {
                player.jumpCounter = 0;
                player.ChangeState(new PlayerIdleState(player));
                return;
            }
        }
        else
        {
            timeSinceGrounded = 0f;
        }

        // Allow double jump / multi-jump
        if (player.jumpCounter < player.maxJumpCap &&
            Input.GetButtonDown("Jump") &&
            player.staminaManager.staminaAmount > 0)
        {
            PerformJump();
        }

        // Animation
        player.animator.SetFloat("yVelocity", player.rb.linearVelocityY);
    }

    private void PerformJump()
{
        // Stamina cost
        player.staminaManager.DecreaseStamina(player.jumpCost);

        player.audioEmitter.PlaySFX(player.jumpSFX, 0.45f, 0.1f);
        
        // Animation
        if (player.jumpCounter > 0)
        {
            CreateJumpVFX();
        }

        // Physics
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.jumpingPower);
        player.jumpCounter++;
    }

    public override void FixedUpdate()
    {
        // Air movement
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.rb.linearVelocity.y);

        // Apply fall multiplier
        if (!player.IsGrounded() && player.rb.linearVelocity.y < 0)
        {
            player.rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (player.fallMultiplier - 1.5f) * Time.fixedDeltaTime;
        }
    }

    public override void Exit()
    {
        player.jumpCounter = 0;
        player.animator.SetBool("isJumping", false);
    }

    public void CreateJumpVFX() {
          // Spawn offset under player's feet
        Vector3 spawnOffset = new Vector3(0, -0.5f, 0); 
        Vector3 spawnPosition = player.transform.position + spawnOffset;

        // Instantiate VFX
        Object.Instantiate(player.jumpVFXPrefab, spawnPosition, Quaternion.identity);
    }
}
