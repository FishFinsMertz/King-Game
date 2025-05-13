using System.Collections;
using UnityEngine;

public class PlayerShootState : PlayerState
{
    private bool attackFinished = false;

    public PlayerShootState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y); // Stop movement
        attackFinished = false;

        // Animation
        player.animator.SetTrigger("Shoot");

        player.StartCoroutine(Blast());
    }

    private IEnumerator Blast()
    {
        yield return new WaitForSeconds(player.shootChargeTime);

        // Shoot
        player.shootHitbox.gameObject.SetActive(true);

        // Stamina cost
        player.staminaManager.DecreaseStamina(player.shootCost);

        // Camera shake
        player.cameraController.StartShake(CameraController.ShakeLevel.light);

        float damage = player.energyManager.energyAmount;
        player.DealDamageToEnemy(damage, player.shootHitbox);
        player.energyManager.DecreaseEnergy(damage);
        player.healthManager.Heal(damage / 2f);
        
        yield return new WaitForSeconds(player.shootSpeed);
        attackFinished = true;
    }

    public override void Update()
    {
        if (!attackFinished) return; // Ensure attack finishes before checking inputs

        if (Input.GetButtonDown("Jump") && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerJumpingState(player));
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerDashState(player));
            return;
        }

        if (player.InputX != 0 && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerRunningState(player));
            return;
        }

        if (Input.GetMouseButtonDown(0) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerAttackState(player));
        }

        if (Input.GetMouseButtonDown(1) && player.IsGrounded() && player.staminaManager.staminaAmount > 0 && player.CanParry()) {
            player.ChangeState(new PlayerParryState(player));
        }

        player.ChangeState(new PlayerIdleState(player)); // Default to idle
    }

    public override void Exit()
    {
        player.shootHitbox.gameObject.SetActive(false);
    }
}
