using UnityEngine;

public class PlayerRunningState : PlayerState
{
    public PlayerRunningState(PlayerController player) : base(player) { }

    public override void Update()
    {
        if (player.InputX == 0) {
            player.ChangeState(new PlayerIdleState(player));
        }

        if (Input.GetButtonDown("Jump") && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerJumpingState(player) );
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerDashState(player));
        }

        if (Input.GetMouseButtonDown(0) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerAttackState(player));
        }
    }

    public override void FixedUpdate()
    {
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.rb.linearVelocity.y);
    }
}
