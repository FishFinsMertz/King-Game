using UnityEngine;
using System.Collections;


public class BasicEnemyBackState : BasicEnemyState
{
    public BasicEnemyBackState(BasicEnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Perform attack
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
        enemy.isAttacking = true;
        enemy.StartCoroutine(BackAtk());
    }

    private IEnumerator BackAtk() {
        //Debug.Log("Charging up my slash, delay: " + delay);

        //Animation
        enemy.animator.SetTrigger("Back");

        yield return new WaitForSeconds(enemy.backAtkDelay); // Delay before attack hitbox activates
        
        // Enabling hitbox and attack
        enemy.backHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        float success = enemy.DealDamageToPlayer(enemy.backDmg, Vector2.right, 10f, enemy.backHitbox);

        if (success == 0)
        {
            // HitStop
            enemy.hitstop.Freeze(enemy.backFreezeDuration);
        }

        enemy.cameraController.StartShake(CameraController.ShakeLevel.light);
        yield return new WaitForSeconds(enemy.backAtkTimer); // Wait for the attack animation to finish
        enemy.isAttacking = false;

        // next states
        if (enemy.distanceFromPlayer <= enemy.slashRange && enemy.IsPlayerInFront()) {
            enemy.ChangeState(new BasicEnemySlashState(enemy));
        }

        if (enemy.distanceFromPlayer > enemy.slashRange && enemy.distanceFromPlayer <= enemy.detectionRange || !enemy.IsPlayerInFront()) {
            enemy.isAttacking = false;
            enemy.ChangeState(new BasicEnemyChaseState(enemy));
        }

        if (enemy.distanceFromPlayer > enemy.detectionRange) {
            enemy.isAttacking = false;
            enemy.ChangeState(new BasicEnemyIdleState(enemy));
        }
    }

    public override void Exit() {
        enemy.isAttacking = false;
        enemy.backHitbox.enabled = false;
    }
}
