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

        if (Input.GetButtonDown("Jump") && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerJumpingState(player));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerDashState(player));
        }

        if (Input.GetMouseButtonDown(0) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerAttackState(player));
        }

        if (Input.GetMouseButtonDown(1) && player.IsGrounded() && player.staminaManager.staminaAmount > 0 && player.CanParry()) {
            player.ChangeState(new PlayerParryState(player));
        }
    }
}
