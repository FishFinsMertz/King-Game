using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        if (player.InputX != 0) {
            player.ChangeState(new PlayerRunningState(player));
        }

        if (Input.GetButtonDown("Jump")) {
            player.ChangeState(new PlayerJumpingState(player));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            player.ChangeState(new PlayerDashState(player));
        }
    }
}
