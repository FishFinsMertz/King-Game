using System.Collections;
using UnityEngine;

public class PlayerParryState : PlayerState
{
    private bool inParry = false;
    private Coroutine parryCoroutine;

    public PlayerParryState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.parryHitbox.enabled = true;
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y); // Stop movement
        inParry = true;

        // Animation
        player.animator.SetTrigger("Parry");

        parryCoroutine = player.StartCoroutine(PerformParry());
    }

    private IEnumerator PerformParry()
    {
        yield return new WaitForSeconds(player.parryChargeTime);
        player.gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        // Stamina cost
        player.staminaManager.DecreaseStamina(player.parryCost);

        yield return new WaitForSeconds(player.parryInvulnerableTime);

        yield return new WaitForSeconds(player.parryDuration - player.parryInvulnerableTime);

        // Only reset if still in this state
        if (player.currentState == this)
        {
            inParry = false;
            player.StartCoroutine(player.StartParryCoolDown());

            player.animator.SetBool("isDamaged", false); // Scuffed fix

            player.ChangeState(new PlayerIdleState(player));
        }
    }

    public override void Update()
    {
        if (inParry) return;

        if (Input.GetButtonDown("Jump") && player.staminaManager.staminaAmount > 0)
        {
            player.ChangeState(new PlayerJumpingState(player));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded() && player.staminaManager.staminaAmount > 0)
        {
            player.ChangeState(new PlayerDashState(player));
        }

        if (player.InputX != 0 && player.staminaManager.staminaAmount > 0)
        {
            player.ChangeState(new PlayerRunningState(player));
        }

        if (Input.GetKeyDown(KeyCode.E) && player.IsGrounded() && player.staminaManager.staminaAmount > 0 && player.energyManager.energyAmount >= 100)
        {
            player.ChangeState(new PlayerShootState(player));
        }
    }

    public override void Exit()
    {
        if (parryCoroutine != null)
        {
            player.StopCoroutine(parryCoroutine);
        }

        player.gameObject.layer = LayerMask.NameToLayer("Player");
        player.parryHitbox.enabled = false;
        player.animator.SetBool("isDamaged", false);
    }
}
