using UnityEngine;

public class BasicEnemyIdleState : BasicEnemyState
{
    public BasicEnemyIdleState(BasicEnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
    }

    public override void Update()
    {
        if (enemy.distanceFromPlayer <= enemy.detectionRange)
        {
            enemy.ChangeState(new BasicEnemyChaseState(enemy));
        }

        if (enemy.distanceFromPlayer <= enemy.slashRange && enemy.IsPlayerInFront())
        {
            enemy.ChangeState(new BasicEnemySlashState(enemy));
        }

        //if (enemy.distanceFromPlayer <= enemy.backRange && !enemy.IsPlayerInFront() && enemy.ShouldBackAtk()) {
        //    enemy.ChangeState(new BasicEnemyBackState(enemy));
        //}
    }
}
