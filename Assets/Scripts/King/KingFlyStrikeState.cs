using System.Collections;
using UnityEngine;

public class KingFlyStrikeState : KingState
{
    public KingFlyStrikeState(KingController king) : base(king) { }

    private float originalGravity;
    private float originalDrag;
    public override void Enter()
    {
        king.canFlyStrike = false;
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;
        originalGravity = king.rb.gravityScale;
        originalDrag = king.rb.linearDamping;

        king.StartCoroutine(FlyStrike());
    }

    private IEnumerator FlyStrike()
    {
        // Phase 1: Rise into the air
        king.animator.SetTrigger("Hover");
        king.audioEmitter.PlaySFX(king.hoverSFX, 0.7f, 0.1f);

        Vector2 targetHoverPosition = new Vector2(king.transform.position.x, king.transform.position.y + king.flyHeight);

        // Turn off some physics stuff
        king.rb.gravityScale = 0f;
        king.rb.linearDamping = 0f;
        // Smooth ascent 
        while (king.transform.position.y < targetHoverPosition.y - 0.1f)
        {
            float distance = targetHoverPosition.y - king.transform.position.y;
            float smoothedSpeed = Mathf.Lerp(0, king.ascentSpeed, distance / king.flyHeight); // Eases to 0 as it approaches target
            king.rb.linearVelocity = new Vector2(0, smoothedSpeed);
            yield return null;
        }

        // Pause at the top
        king.rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(king.flyHoverTime);

        // Phase 2: Lock onto player’s position to slam
        king.Flip();
        Vector2 slamDirection = (king.player.transform.position - king.transform.position).normalized;
        king.rb.linearVelocity = slamDirection * king.flyStrikeSpeed;
    
        // Wait until boss lands
        yield return new WaitUntil(() => king.IsGrounded());
        king.animator.SetTrigger("Land");
        king.audioEmitter.PlaySFX(king.flyStrikeSFX, 0.7f, 0.1f);
        // Slam landed animation

        // Enable hitbox
        king.flyStrikeHitbox.enabled = true;

        int result = king.DealDamageToPlayer(king.flyStrikeDmg, Vector2.right, 20f, king.flyStrikeHitbox);
        if (result == 0)
        {
            king.hitstop.Freeze(king.flyStrikeFreezeDuration);
        }

        //Reset Gravity
        king.rb.gravityScale = originalGravity;
        king.rb.linearDamping = originalDrag;
        king.StartCoroutine(king.StartFlyStrikeCoolDown());

        king.cameraController.StartShake(CameraController.ShakeLevel.heavy);
        yield return new WaitForSeconds(king.flyStrikeDuration);

        // End
        king.ChangeState(new KingWalkState(king));
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        king.flyStrikeHitbox.enabled = false;
        king.isAttacking = false;
        king.megaSlamHitbox.enabled = false;
    }
}
