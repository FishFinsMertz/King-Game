using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashTime = 0.1f; // Duration of the dash
    private float dashTimer;
    private Vector2 slideForce;
    private float afterImageTimer;

    private float lessGravityAmt = 0.5f;

    public PlayerDashState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.audioEmitter.PlaySFX(player.dashSFX, 0.65f, 0.1f);

        // After image
        afterImageTimer = player.afterImageCooldown;
        CreateAfterImage();

        // Decrease Stamina
        player.staminaManager.DecreaseStamina(player.dashCost);

        dashTimer = dashTime;
        
        // Set dash velocity
        player.rb.linearVelocity = new Vector2(player.isFacingRight * player.dashPower, player.rb.linearVelocity.y * lessGravityAmt);

        // Calculate slide force for later use
        slideForce = new Vector2(player.isFacingRight * player.dashPower * 0.5f, player.rb.linearVelocity.y * lessGravityAmt);
    }

    public override void Update()
    {
        dashTimer -= Time.deltaTime;
        afterImageTimer -= Time.deltaTime;

        if (afterImageTimer <= 0f)
        {
            CreateAfterImage();
            afterImageTimer = player.afterImageCooldown;
        }

        if (dashTimer <= 0)
        {
                player.ChangeState(new PlayerSlidingState(player, slideForce));
        }

        if (Input.GetButtonDown("Jump")) {
            player.ChangeState(new PlayerJumpingState(player));
        }
    }

    public void CreateAfterImage() {
        GameObject afterImage = Object.Instantiate(player.afterImagePrefab, player.transform.position, Quaternion.identity);

        SpriteRenderer afterImageSR = afterImage.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer playerSR = player.GetComponentInChildren<SpriteRenderer>();
        
        afterImageSR.sprite = playerSR.sprite;
        afterImageSR.flipX = player.isFacingRight == -1;
    }
}
