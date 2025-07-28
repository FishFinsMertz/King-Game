using UnityEngine;

public class PlayerRunningState : PlayerState
{
    public PlayerRunningState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetBool("isRunning", true);
        player.audioEmitter.PlaySFXLoop(player.runSFX, 0.65f, 0f);
    }

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

        if (Input.GetMouseButtonDown(1) && player.IsGrounded() && player.staminaManager.staminaAmount > 0 && player.CanParry()) {
            player.ChangeState(new PlayerParryState(player));
        }

        if (Input.GetKeyDown(KeyCode.E) && player.IsGrounded() && player.staminaManager.staminaAmount > 0 && player.energyManager.energyAmount >= 100) {
            player.ChangeState(new PlayerShootState(player));
        }
    }

    public override void FixedUpdate()
    {
        player.rb.linearVelocity = new Vector2(player.InputX * player.speed, player.rb.linearVelocity.y);
    }

    public override void Exit()
    {
        player.animator.SetBool("isRunning", false);
        player.audioEmitter.StopSFX();
    }
}
