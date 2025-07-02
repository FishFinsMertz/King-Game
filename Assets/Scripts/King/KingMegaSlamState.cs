using System.Collections;
using UnityEngine;

public class KingMegaSlamState : KingState
{
    public KingMegaSlamState(KingController king) : base(king) { }

    public override void Enter()
    {
        // Perform attack
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;

        king.StartCoroutine(PerformMegaSlam());
    }
    private IEnumerator PerformMegaSlam()
    {
        // Attempt Teleport
        yield return king.enemyTeleporter.TryTeleport(4f, king.teleportProbability); //Teleport distance and probability of teleporting
        king.Flip();

        king.nonParryWarning.SetActive(true);
        king.animator.SetTrigger("MegaSlam");

        yield return new WaitForSeconds(king.megaSlamChargeTime);

        king.megaSlamHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        int success = king.DealDamageToPlayer(king.megaSlamDmg, Vector2.right, 15f, king.megaSlamHitbox);

        if (success == 0)
        {
            king.hitstop.Freeze(king.megaSlamFreezeDuration);
        }

        king.cameraController.StartShake(CameraController.ShakeLevel.medium);


        king.StartCoroutine(king.StartMegaSlamCoolDown());

        yield return new WaitForSeconds(king.megaSlamDuration); // Wait for the attack animation to finish

        king.nonParryWarning.SetActive(false);

        // Change states
        king.ChangeState(new KingWalkState(king));

    }

    public override void Exit()
    {
        king.isAttacking = false;
        king.megaSlamHitbox.enabled = false;
    }
}
