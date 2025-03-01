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
        float delay = Random.Range(0.3f, 1.2f);
        //Debug.Log("Charging up my slash, delay: " + delay);
        yield return new WaitForSeconds(delay); // Delay before attack hitbox activates
        
        // Enabling hitbox and attack
        enemy.slashHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        DealDamageToPlayer();

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
    
    private void DealDamageToPlayer() {
        // Checks all collider for player
        Collider2D[] hits = Physics2D.OverlapBoxAll(enemy.slashHitbox.bounds.center, enemy.slashHitbox.bounds.size, 0);
        
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                //Debug.Log("Player hit by slash!");
                PlayerHealthManager playerHealth = hit.GetComponent<PlayerHealthManager>();

                if (playerHealth != null)
                {
                    Vector2 hitDirection = (hit.transform.position - enemy.transform.position).normalized;
                    playerHealth.TakeDamage(30, hitDirection, 10f);
                }
            }
        }
    }

    public override void Exit() {
        enemy.slashHitbox.enabled = false;
    }
}
