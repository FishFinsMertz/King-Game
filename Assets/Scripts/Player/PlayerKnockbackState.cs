using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    private Vector2 knockbackForce;
    private float knockbackTimer;
    private float slideTimer;
    private float stunTimer;
    private bool isVerticalKnockback;
    private bool waitingForGround;

    private float originalDamping;

    private enum KnockbackPhase { Knockback, Slide, Stun, WaitForGround }
    private KnockbackPhase phase;

    public PlayerKnockbackState(PlayerController player, Vector2 knockbackForce) : base(player)
    {
        this.knockbackForce = knockbackForce;
        isVerticalKnockback = Mathf.Abs(knockbackForce.y) > Mathf.Abs(knockbackForce.x);
        phase = KnockbackPhase.Knockback;
    }

    public override void Enter()
    {
        originalDamping = player.rb.linearDamping;
        player.rb.linearDamping = 0f; // Remove drag

        player.rb.linearVelocity = knockbackForce;

        // Animation
        player.animator.SetBool("isDamaged", true);
        player.animator.SetTrigger("Knockback");
        player.audioEmitter.PlaySFX(player.damagedSFX, 0.3f, 0.1f);
    }

    public override void Exit()
    {
        // Restore damping
        player.rb.linearDamping = originalDamping;
        player.animator.SetBool("isDamaged", false);
    }

    public override void Update()
    {
        switch (phase)
        {
            case KnockbackPhase.Knockback:
                knockbackTimer += Time.deltaTime;
                if (knockbackTimer >= player.knockbackDuration)
                {
                    if (isVerticalKnockback)
                    {
                        phase = KnockbackPhase.WaitForGround;
                        waitingForGround = true;
                    }
                    else
                    {
                        phase = KnockbackPhase.Slide;
                        slideTimer = 0f;
                    }
                }
                break;

            case KnockbackPhase.Slide:
                slideTimer += Time.deltaTime;
                if (slideTimer >= player.slideDuration)
                {
                    phase = KnockbackPhase.Stun;
                    stunTimer = 0f;
                    player.rb.linearVelocity = Vector2.zero;
                    // Animation
                    player.animator.SetTrigger("Recover");
                }
                break;

            case KnockbackPhase.Stun:
                stunTimer += Time.deltaTime;
                if (stunTimer >= player.stunDuration)
                {
                    player.ChangeState(new PlayerIdleState(player));
                }
                break;

            case KnockbackPhase.WaitForGround:
                if (player.IsGrounded())
                {
                    phase = KnockbackPhase.Stun;
                    stunTimer = 0f;
                    player.rb.linearVelocity = Vector2.zero;
                    
                    // Animation
                    player.animator.SetTrigger("Recover");
                }
                break;
        }
    }

    public override void FixedUpdate()
    {
        switch (phase)
        {
            case KnockbackPhase.Slide:
                player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x * 0.9f, player.rb.linearVelocity.y);
                break;

            case KnockbackPhase.Stun:
                player.rb.linearVelocity = Vector2.zero;
                break;

            case KnockbackPhase.Knockback:
            case KnockbackPhase.WaitForGround:
                // Do nothing; let natural fall happen
                break;
        }
    }
}
