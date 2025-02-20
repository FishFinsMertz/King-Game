using UnityEngine;

public class PlayerJumpingState : PlayerState
{
    private bool hasJumped;

    public PlayerJumpingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        if (player.IsGrounded() || player.jumpCounter < player.maxJumpCap - 1)
        {
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpingPower);
            player.jumpCounter++;
            hasJumped = true;
        }
    }

    public override void Update()
    {
        if (player.IsGrounded())
            player.ChangeState(new PlayerIdleState(player));
    }

    public override void FixedUpdate()
    {
        // Apply horizontal movement in the air
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.rb.linearVelocity.y);

        // Apply stronger gravity when falling
        if (player.rb.linearVelocity.y < 0)
        {
            player.rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (player.fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}
