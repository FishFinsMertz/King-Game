using System.Collections;
using UnityEngine;

public class KingRumbleState : KingState
{
    public KingRumbleState(KingController king) : base(king) { }

    public override void Enter()
    {
        king.canRumble = false;
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;
        king.StartCoroutine(PerformRumble());
    }

    private IEnumerator PerformRumble()
    {
        king.nonParryWarning.SetActive(true);
        king.animator.SetTrigger("Rumble");
        yield return new WaitForSeconds(king.rumbleChargeTime);
        king.rumbleHitbox.enabled = true;

        int success = king.DealDamageToPlayer(king.rumbleDmg, Vector2.right, 20f, king.rumbleHitbox);

        if (success == 0)
        {
            king.hitstop.Freeze(king.megaSlamFreezeDuration);
        }

        king.cameraController.StartShake(CameraController.ShakeLevel.medium);

        yield return new WaitForSeconds(king.rumbleDuration);
        king.StartCoroutine(king.StartRumbleCoolDown());
        king.nonParryWarning.SetActive(false);
        king.ChangeState(new KingWalkState(king));
    }

    public override void Exit()
    {
        king.rumbleHitbox.enabled = false;
        king.isAttacking = false;
    }
}
