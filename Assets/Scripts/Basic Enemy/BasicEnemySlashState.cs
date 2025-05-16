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

        //Debug.Log("Charging up my slash, delay: " + delay);
        yield return new WaitForSeconds(enemy.slashChargeTime); // Delay before attack hitbox activates
        
        // Enabling hitbox and attack
        enemy.slashHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        enemy.DealDamageToPlayer(30, Vector2.right, 10f, enemy.slashHitbox);

        enemy.cameraController.StartShake(CameraController.ShakeLevel.light);
        yield return new WaitForSeconds(enemy.slashTimer); // Wait for the attack animation to finish
        enemy.isAttacking = false;

        // next states
        if (enemy.distanceFromPlayer <= enemy.slashRange && enemy.IsPlayerInFront())
        {
            // SCUFFED FIX FOR WHEN ENEMY ATTACKS 2 TIMES IN A ROW AND THE THRUST ANIMATION TRIGGER DOESNT RESET
            enemy.isAttacking = false;
            enemy.ChangeState(new BasicEnemyChaseState(enemy));
        }

        if (enemy.distanceFromPlayer <= enemy.backRange && !enemy.IsPlayerInFront() && enemy.ShouldBackAtk()) {
            enemy.ChangeState(new BasicEnemyBackState(enemy));
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
        enemy.slashHitbox.enabled = false;
    }
}
