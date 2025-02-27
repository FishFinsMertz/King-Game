using UnityEngine;

public class PlayerSlidingState : PlayerState
{
    private float slideTimer = 0.4f; // Duration of the slide
    private float slideFriction = 0.85f; // Multiplier applied per frame for damping
    private Vector2 slideForce; // Initial force applied to the player

    public PlayerSlidingState(PlayerController player, Vector2 force) : base(player)
    {
        slideForce = force;
    }

    public override void Enter()
    {
        player.rb.linearVelocity = slideForce; // Apply initial force
    }

    public override void FixedUpdate()
    {
        slideTimer -= Time.fixedDeltaTime;

        // Apply exponential friction
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x * slideFriction, player.rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump")) {
            player.ChangeState(new PlayerJumpingState(player));
        }

        // Transition to idle or running when slide is over or speed is very low
        if (slideTimer <= 0 || Mathf.Abs(player.rb.linearVelocity.x) < 0.5f)
        {
            player.ChangeState(player.InputX != 0 ? new PlayerRunningState(player) : new PlayerIdleState(player));
        }
    }
}
