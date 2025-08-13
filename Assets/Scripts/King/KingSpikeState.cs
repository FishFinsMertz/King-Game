using System.Collections;
using UnityEngine;

public class KingSpikeState : KingState
{
    public KingSpikeState(KingController king) : base(king) { }

    public override void Enter()
    {
        king.canSpike = false;
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;

        king.StartCoroutine(PerformSpike());
    }

    private IEnumerator PerformSpike()
    {
        king.animator.SetTrigger("Spike");
        king.audioEmitter.PlaySFX(king.spikeSFX, 0.75f, 0.1f);
        yield return new WaitForSeconds(king.spikeChargeTime);

        king.spikeHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        int success = king.DealDamageToPlayer(king.spikeDmg, Vector2.right, 9f, king.spikeHitbox);

        if (success == 0)
        {
            king.hitstop.Freeze(king.spikeFreezeDuration);
        }
        king.cameraController.StartShake(CameraController.ShakeLevel.light);

        yield return new WaitForSeconds(king.spikeDuration);
        king.StartCoroutine(king.StartSpikeCoolDown());
        king.ChangeState(new KingWalkState(king));
    }

    public override void Exit()
    {
        king.spikeHitbox.enabled = false;
        king.isAttacking = false;
    }
}
