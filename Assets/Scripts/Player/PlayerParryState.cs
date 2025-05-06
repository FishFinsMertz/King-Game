using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerParryState : PlayerState
{
    private bool inParry = false;
    public PlayerParryState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.parryHitbox.enabled = true;
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y); // Stop movement
        inParry = true;

        //Animation
        player.animator.SetTrigger("Parry");

        player.StartCoroutine(PerformParry());  
    }

    private IEnumerator PerformParry() {
        yield return new WaitForSeconds(player.parryChargeTime);
        //Debug.Log("Invincible!!!");
        // Stamina cost
        player.staminaManager.DecreaseStamina(player.parryCost);
        player.gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        yield return new WaitForSeconds(player.parryInvulnerableTime);
        // Begin Cool Down
        inParry = false;
        player.StartCoroutine(player.StartParryCoolDown());
        player.ChangeState(new PlayerIdleState(player)); // Default to idle
    }

    public override void Update()
    {
        if (inParry) return; 


        if (Input.GetButtonDown("Jump") && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerJumpingState(player));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerDashState(player));
        }

        if (player.InputX != 0 && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerRunningState(player));
        }

        if (Input.GetKeyDown(KeyCode.E) && player.IsGrounded() && player.staminaManager.staminaAmount > 0) {
            player.ChangeState(new PlayerShootState(player));
        }
    }

    public override void Exit()
    {
        //Debug.Log("Weak again");
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        player.parryHitbox.enabled = false;
    }
}
