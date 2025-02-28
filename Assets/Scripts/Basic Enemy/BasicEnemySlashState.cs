using UnityEngine;
using System.Collections;


public class BasicEnemySlashState : BasicEnemyState
{
    public BasicEnemySlashState(BasicEnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
        enemy.isAttacking = true;
        enemy.StartCoroutine(PerformSlash());
    }

    private IEnumerator PerformSlash() {
        float delay = Random.Range(0.3f, 1.2f);
        //Debug.Log("Charging up my slash, delay: " + delay);
        yield return new WaitForSeconds(delay); // Delay before attack hitbox activates
        
        /* ADD WHEN IMPLEMENTED
        if (enemy.IsPlayerInFront())
        {
            enemy.ActivateAttackHitbox();
        }
        */
        //Debug.Log("Slash!");
        enemy.cameraController.StartShake(CameraController.ShakeLevel.light);
        yield return new WaitForSeconds(enemy.slashTimer); // Wait for the attack animation to finish
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
    
}
