using UnityEngine;

public class BasicEnemyChaseState : BasicEnemyState
{
    private int moveDirection;
    private float repulsionDistance = 5f;  // Distance at which repulsion force will be applied
    private float repulsionStrength = 50f;  // Strength of the repulsive force
    public BasicEnemyChaseState(BasicEnemyController enemy) : base(enemy) { }

    public override void Update()
    {
        if (enemy.distanceFromPlayer <= enemy.slashRange && enemy.IsPlayerInFront()) {
            enemy.ChangeState(new BasicEnemySlashState(enemy));
        }

        if (enemy.distanceFromPlayer > enemy.detectionRange) {
            enemy.ChangeState(new BasicEnemyIdleState(enemy));
        }

        // Add a probability thing later so the enemy doesn't always jump!!!!!!!
        if (enemy.playerInLeapRange() && enemy.ShouldLeap()) {
            enemy.ChangeState(new BasicEnemyLeapState(enemy));
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

        // Apply repulsive force if there are other enemies too close
        ApplyRepulsiveForce();
    }

    private void ApplyRepulsiveForce()
    {
        // Find all colliders within a specified range (could be a group of enemies)
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemy.transform.position, repulsionDistance, LayerMask.GetMask("Enemy"));

        foreach (Collider2D otherEnemyCollider in nearbyEnemies)
        {
            if (otherEnemyCollider != enemy.GetComponent<Collider2D>())  // Don't push itself
            {
                // Calculate the vector pointing from this enemy to the other one
                Vector2 directionToOtherEnemy = enemy.transform.position - otherEnemyCollider.transform.position;
                float distanceToOtherEnemy = directionToOtherEnemy.magnitude;

                // If they are too close, apply a repulsive force
                if (distanceToOtherEnemy < repulsionDistance)
                {
                    // Normalize direction to avoid increasing velocity too much
                    directionToOtherEnemy.Normalize();
                    
                    // Set the y component of the direction to 0 to apply force only in the x direction
                    directionToOtherEnemy.y = 0;

                    // Apply the repulsive force (only in the x direction)
                    enemy.rb.AddForce(directionToOtherEnemy * repulsionStrength);
                }
            }
        }
    }
}
