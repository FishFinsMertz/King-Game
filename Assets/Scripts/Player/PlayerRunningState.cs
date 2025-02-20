using UnityEngine;

public class PlayerRunningState : PlayerState
{
    public PlayerRunningState(PlayerController player) : base(player) { }

    public override void Update()
    {
        if (player.InputX == 0)
            player.ChangeState(new PlayerIdleState(player));

        if (Input.GetButtonDown("Jump"))
            player.ChangeState(new PlayerJumpingState(player));
    }

    public override void FixedUpdate()
    {
        player.rb.velocity = new Vector2(player.InputX * player.speed, player.rb.velocity.y);
    }
}
