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
        // Attempt Teleport
        yield return king.enemyTeleporter.TryTeleport(3f, king.teleportProbability); //Teleport distance and probability of teleporting
        king.Flip();

        king.animator.SetTrigger("Thrust");
        king.audioEmitter.PlaySFX(king.thrustSFX, 0.5f, 0.1f);

        yield return new WaitForSeconds(king.thrustChargeTime); // Delay before attack hitbox activates

        // Enabling hitbox and attack
        king.thrustHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        int success = king.DealDamageToPlayer(king.thrustDmg, Vector2.right, 9f, king.thrustHitbox);

        if (success == 0)
        {
            king.hitstop.Freeze(king.thrustFreezeDuration);
            king.cameraController.StartShake(CameraController.ShakeLevel.light);
        }

        yield return new WaitForSeconds(king.thrustDuration); // Wait for the attack animation to finish

        // Change state
        king.ChangeState(new KingWalkState(king));

    }

    public override void Exit()
    {
        king.isAttacking = false;
        king.thrustHitbox.enabled = false;
    }
}
