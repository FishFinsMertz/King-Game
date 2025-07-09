using System.Collections;
using UnityEngine;

public class KingRushState : KingState
{
    public KingRushState(KingController king) : base(king) { }

    public override void Enter()
    {
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;

        king.StartCoroutine(StartRush());
    }

    private IEnumerator StartRush()
    {
        yield return new WaitForSeconds(king.rushChargeTime);

        // Attempt Teleport
        //yield return king.enemyTeleporter.TryTeleport(5.5f, king.teleportProbability);
        //king.Flip();

        king.rush1Hitbox.enabled = true;

        Vector2 direction = (king.player.transform.position - king.transform.position).normalized;
        Vector2 xDirection = new Vector2(Mathf.Sign(direction.x), 0);
        float distance = Vector2.Distance(king.player.transform.position, king.transform.position);
        float targetDistance = distance - 1f;

        Vector2 startPosition = king.transform.position;
        float traveled = 0f;
        bool hasHit = false;

        // Apply velocity for rush
        king.rb.linearVelocity = xDirection * king.rushSpeed;

        while (traveled < targetDistance)
        {
            traveled = Mathf.Abs(king.transform.position.x - startPosition.x);

            if (!hasHit)
            {
                int rushResult = king.DealDamageToPlayer(king.rushDmg, xDirection, 8f, king.rush1Hitbox);
                if (rushResult == 0)
                {
                    hasHit = true;
                    king.hitstop.Freeze(king.rushFreezeDuration);
                    king.cameraController.StartShake(CameraController.ShakeLevel.medium);
                }
            }

            yield return null;
        }

        // Stop active movement but preserve momentum for natural slide
        king.rb.linearVelocity = king.rb.linearVelocity * 0.2f;

        king.rush1Hitbox.enabled = false;

        king.nonParryWarning.SetActive(true);

        yield return new WaitForSeconds(king.rushSlashChargeTime);

        king.rush2Hitbox.enabled = true;
        int rushSlashResult = king.DealDamageToPlayer(king.rushSlashDmg, Vector2.up, 20f, king.rush2Hitbox);
        if (rushSlashResult == 0)
        {
            king.hitstop.Freeze(king.rushSlashFreezeDuration);
            king.cameraController.StartShake(CameraController.ShakeLevel.heavy);
        }
        else
        { 
            king.cameraController.StartShake(CameraController.ShakeLevel.medium);
        }

        yield return new WaitForSeconds(king.rushSlashDuration);

        king.nonParryWarning.SetActive(false);

        king.rush2Hitbox.enabled = false;
        king.StartCoroutine(king.StartRushCoolDown());
        king.ChangeState(new KingWalkState(king));
    }

    public override void Exit()
    {
        king.isAttacking = false;
    }
}
