using UnityEngine;

public class PlayerRunningState : PlayerState
{
    public PlayerRunningState(PlayerController player) : base(player) { }

    public override void Update()
    {
        if (player.InputX == 0) {
            player.ChangeState(new PlayerIdleState(player));
        }

        if (Input.GetButtonDown("Jump")) {
            player.ChangeState(new PlayerJumpingState(player));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ChangeState(new PlayerDashState(player));
        }
    }

    public override void FixedUpdate()
    {
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.rb.linearVelocity.y);
    }
}
