using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private bool attackFinished = false;

    public PlayerAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.attackHitbox.enabled = true;
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y); // Stop movement
        attackFinished = false;
        player.StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(player.atkChargeTime);
        
        // Stamina cost
        player.staminaManager.DecreaseStamina(player.atkCost);

        player.DealDamageToEnemy();
        
        yield return new WaitForSeconds(player.atkSpeed);
        attackFinished = true;
    }

    public override void Update()
    {
        if (!attackFinished) return; // Ensure attack finishes before checking inputs

        if (Input.GetButtonDown("Jump") && player.staminaManager.staminaAmount > 0)
        {
            player.ChangeState(new PlayerJumpingState(player));
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded() && player.staminaManager.staminaAmount > 0)
        {
            player.ChangeState(new PlayerDashState(player));
            return;
        }

        if (player.InputX != 0 && player.staminaManager.staminaAmount > 0)
        {
            player.ChangeState(new PlayerRunningState(player));
            return;
        }

        player.ChangeState(new PlayerIdleState(player)); // Default to idle
    }

    public override void Exit()
    {
        player.attackHitbox.enabled = false;
    }
}
