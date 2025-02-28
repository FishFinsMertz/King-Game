using UnityEngine;
public class BasicEnemyChaseState : BasicEnemyState
{
    private int moveDirection;
    public BasicEnemyChaseState(BasicEnemyController enemy) : base(enemy) { }

    public override void Update()
    {
        if (enemy.distanceFromPlayer <= enemy.slashRange && enemy.IsPlayerInFront()) {
            enemy.ChangeState(new BasicEnemySlashState(enemy));
        }

        if (enemy.distanceFromPlayer > enemy.detectionRange) {
            enemy.ChangeState(new BasicEnemyIdleState(enemy));
        }
    }

    public override void FixedUpdate()
    {
        // If the enemy is further than the minimum threshold, update direction
        if (enemy.distanceFromPlayer > enemy.minThreshold) {
            moveDirection = enemy.vectorFromPlayer.x < 0 ? -1 : 1;
        }

        // Move the enemy in the stored direction
        enemy.rb.linearVelocity = new Vector2(enemy.chaseSpeed * moveDirection, enemy.rb.linearVelocity.y);
    }
}
