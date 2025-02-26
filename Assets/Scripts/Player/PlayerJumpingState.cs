using UnityEngine;

public class PlayerJumpingState : PlayerState
{
    public int jumpCounter = 0;
    public PlayerJumpingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        if (player.IsGrounded() || jumpCounter < (player.maxJumpCap - 1))
        {
            player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.jumpingPower);
            jumpCounter++;
        }
    }

    public override void Update()
    {
        if (player.IsGrounded())
        {
            jumpCounter = 0;
            player.ChangeState(new PlayerIdleState(player));
        }

        else if (jumpCounter < (player.maxJumpCap - 1) && Input.GetButtonDown("Jump")) {
            player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.jumpingPower);
            jumpCounter++;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ChangeState(new PlayerDashState(player));
        }
    }

    public override void FixedUpdate()
    {
        // Apply horizontal movement in the air
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.rb.linearVelocity.y);

        // Faster fall
        if (!player.IsGrounded() && player.rb.linearVelocity.y < 0) {
            player.rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (player.fallMultiplier - 1.5f) * Time.fixedDeltaTime;
        }
    }
}
