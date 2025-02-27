using UnityEngine;
public class BasicEnemyChaseState : BasicEnemyState
{
    public BasicEnemyChaseState(BasicEnemyController enemy) : base(enemy) { }

    public override void Update()
    {
        if (enemy.distanceFromPlayer > 10) {
            enemy.ChangeState(new BasicEnemyIdleState(enemy));
        }
    }

    public override void FixedUpdate()
    {
        if (enemy.vectorFromPlayer.x < 0) {
            enemy.rb.linearVelocity = new Vector2(enemy.chaseSpeed * -1, enemy.rb.linearVelocity.y);
        }
        else {
            enemy.rb.linearVelocity = new Vector2(enemy.chaseSpeed, enemy.rb.linearVelocity.y);
        }
    }
}
