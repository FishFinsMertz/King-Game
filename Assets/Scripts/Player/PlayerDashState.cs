using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTime = 0.1f; // Duration of the dash
    private float dashTimer;
    private Vector2 slideForce;

    private float lessGravityAmt = 0.5f;

    public PlayerDashState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Decrease Stamina
        player.staminaManager.DecreaseStamina(player.dashCost);

        dashTimer = dashTime;
        
        // Set dash velocity
        player.rb.linearVelocity = new Vector2(player.isFacingRight * player.dashPower, player.rb.linearVelocity.y * lessGravityAmt);

        // Calculate slide force for later use
        slideForce = new Vector2(player.isFacingRight * player.dashPower * 0.5f, player.rb.linearVelocity.y * lessGravityAmt);
    }

    public override void Update()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
                player.ChangeState(new PlayerSlidingState(player, slideForce));
        }

        if (Input.GetButtonDown("Jump")) {
            player.ChangeState(new PlayerJumpingState(player));
        }
    }
}
