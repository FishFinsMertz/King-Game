using System.Collections;
using UnityEngine;

public class KingThrustState : KingState
{
    public KingThrustState(KingController king) : base(king) { }

    public override void Enter()
    {
        // Perform attack
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;
        king.StartCoroutine(PerformThrust());
    }

    private IEnumerator PerformThrust()
    {
        king.animator.SetTrigger("Thrust");

        yield return new WaitForSeconds(king.thrustChargeTime); // Delay before attack hitbox activates

        // Enabling hitbox and attack
        king.thrustHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        int success = king.DealDamageToPlayer(king.thrustDmg, Vector2.right, 12f, king.thrustHitbox);

        if (success == 0)
        {
            king.hitstop.Freeze(king.thrustFreezeDuration);
            king.cameraController.StartShake(CameraController.ShakeLevel.light);
        }

        yield return new WaitForSeconds(king.thrustDuration); // Wait for the attack animation to finish

        // Change states
        if (king.GetPlayerDistance() > king.detectionRange)
        {
            king.ChangeState(new KingIdleState(king));
        }

        if (king.GetPlayerDistance() <= king.closeRange && king.IsPlayerInFront())
        {
            king.ChangeState(new KingWalkState(king));
        }
        else
        {
            king.ChangeState(new KingWalkState(king));
        }
    }

    public override void Exit()
    {
        king.isAttacking = false;
    }
}
