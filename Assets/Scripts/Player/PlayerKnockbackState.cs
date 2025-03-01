using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    private Vector2 knockbackForce;
    private float knockbackTimer;
    private float slideTimer;
    private float stunTimer;

    private float knockbackDuration = 0.3f; // Time spent in knockback phase
    private float slideDuration = 0.5f;     // Time spent sliding before stopping
    private float stunDuration = 0.7f;      // Time spent stunned before regaining control

    private bool isVerticalKnockback;
    private bool waitingForGround;

    private enum KnockbackPhase { Knockback, Slide, Stun, WaitForGround }
    private KnockbackPhase phase;

    public PlayerKnockbackState(PlayerController player, Vector2 knockbackForce) : base(player)
    {
        this.knockbackForce = knockbackForce;
        isVerticalKnockback = Mathf.Abs(knockbackForce.y) > Mathf.Abs(knockbackForce.x);

        knockbackTimer = 0;
        slideTimer = 0;
        stunTimer = 0;
        waitingForGround = false;

        phase = KnockbackPhase.Knockback;
    }

    public override void Enter()
    {
        player.rb.linearVelocity = knockbackForce;
        player.rb.linearDamping = 0; // No damping during knockback
    }

    public override void Update()
    {
        switch (phase)
        {
            case KnockbackPhase.Knockback:
                knockbackTimer += Time.deltaTime;
                if (knockbackTimer >= knockbackDuration)
                {
                    if (isVerticalKnockback)
                    {
                        phase = KnockbackPhase.WaitForGround; // Wait until player lands
                        waitingForGround = true;
                    }
                    else
                    {
                        phase = KnockbackPhase.Slide;
                        knockbackTimer = 0;
                    }
                }
                break;

            case KnockbackPhase.Slide:
                slideTimer += Time.deltaTime;
                if (slideTimer >= slideDuration)
                {
                    phase = KnockbackPhase.Stun;
                    slideTimer = 0;
                    player.rb.linearVelocity = Vector2.zero; // Stop completely
                }
                break;

            case KnockbackPhase.Stun:
                stunTimer += Time.deltaTime;
                if (stunTimer >= stunDuration)
                {
                    player.ChangeState(new PlayerIdleState(player)); // Allow movement again
                }
                break;

            case KnockbackPhase.WaitForGround:
                if (player.IsGrounded())
                {
                    phase = KnockbackPhase.Stun;
                    stunTimer = 0;
                }
                break;
        }
    }

    public override void FixedUpdate()
    {
        switch (phase)
        {
            case KnockbackPhase.Knockback:
                player.rb.linearVelocity = knockbackForce;
                break;

            case KnockbackPhase.Slide:
                player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x * 0.9f, player.rb.linearVelocity.y);
                break;

            case KnockbackPhase.Stun:
                player.rb.linearVelocity = Vector2.zero;
                break;

            case KnockbackPhase.WaitForGround:
                // Don't apply any extra force, just wait for the player to land
                break;
        }
    }
}
