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
        yield return new WaitForSeconds(enemy.backAtkDelay); // Delay before attack hitbox activates
        
        // Enabling hitbox and attack
        enemy.slashHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        enemy.DealDamageToPlayer(20, Vector2.right, 10f, enemy.backHitbox);

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
        enemy.backHitbox.enabled = false;
    }
}
