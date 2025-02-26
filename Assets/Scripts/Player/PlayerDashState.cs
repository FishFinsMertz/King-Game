using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTime = 0.1f; // Duration of the dash
    private float dashTimer;
    private Vector2 slideForce;

    public PlayerDashState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        Debug.Log("DASH!");
        dashTimer = dashTime;
        
        // Set dash velocity

        player.rb.linearVelocity = new Vector2(player.isFacingRight * player.dashPower, player.rb.linearVelocity.y);

        // Calculate slide force for later use
        slideForce = new Vector2(player.isFacingRight * player.dashPower * 0.5f, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            if (Mathf.Abs(player.InputX) > 0) {
                player.ChangeState(new PlayerRunningState(player));
            }

            else {
                player.ChangeState(new PlayerSlidingState(player, slideForce));
            }
        }
    }
}
