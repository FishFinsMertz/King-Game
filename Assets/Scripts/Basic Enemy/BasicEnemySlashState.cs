using UnityEngine;
using System.Collections;


public class BasicEnemySlashState : BasicEnemyState
{
    public BasicEnemySlashState(BasicEnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Perform attack
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
        enemy.isAttacking = true;
        enemy.StartCoroutine(PerformSlash());
    }

    private IEnumerator PerformSlash() {
        // Animation
        enemy.animator.SetTrigger("Thrust");
        enemy.audioEmitter.PlaySFX(enemy.thrustSFX, 0.5f, 0.1f);

        //Debug.Log("Charging up my slash, delay: " + delay);
        yield return new WaitForSeconds(enemy.slashChargeTime); // Delay before attack hitbox activates

        // Enabling hitbox and attack
        enemy.slashHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        int success = enemy.DealDamageToPlayer(enemy.slashDmg, Vector2.right, 10f, enemy.slashHitbox);

        if (success == 0)
        {
            enemy.hitstop.Freeze(enemy.slashFreezeDuration);
            enemy.cameraController.StartShake(CameraController.ShakeLevel.light);
        }
        
        yield return new WaitForSeconds(enemy.slashTimer); // Wait for the attack animation to finish
        enemy.isAttacking = false;

        // Switch state
        enemy.ChangeState(new BasicEnemyChaseState(enemy));
    }

    public override void Exit() {
        enemy.isAttacking = false;
        enemy.slashHitbox.enabled = false;
    }
}
