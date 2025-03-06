using UnityEngine;
using System.Collections;

public class BasicEnemyLeapState : BasicEnemyState
{
    private Vector2 leapTarget;
    private float jumpHeight = 6f; // Controlled jump height
    private float gravityScale = 2f; // Stronger gravity for better arc
    private float leapTimeout = 3f; // Fail-safe to prevent infinite loops
    private bool isLeaping = false;

    public BasicEnemyLeapState(BasicEnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.nonParryWarning.SetActive(true);
        if (enemy.player == null) return;
        // Determine leap target position
        float targetDistance = (enemy.minLeapRange + enemy.maxLeapRange) / 3f;
        float leapX = enemy.transform.position.x + Mathf.Sign(enemy.vectorFromPlayer.x) * targetDistance;
        leapTarget = new Vector2(leapX, enemy.player.transform.position.y);

        // Ensure the enemy doesn't jump too high
        float heightDifference = enemy.player.transform.position.y - enemy.transform.position.y;
        float adjustedJumpHeight = Mathf.Max(jumpHeight, heightDifference + 2f);

        // Set gravity scale for controlled jump
        enemy.rb.gravityScale = gravityScale;

        // Start leap sequence
        enemy.StartCoroutine(LeapAttack(adjustedJumpHeight));
    }

    private IEnumerator LeapAttack(float adjustedJumpHeight)
    {
        isLeaping = true;
        yield return new WaitForSeconds(0.5f); // Charging animation

        // Calculate leap velocity
        float timeToTarget = 0.5f;
        float horizontalSpeed = (leapTarget.x - enemy.transform.position.x) / timeToTarget;
        float verticalSpeed = Mathf.Sqrt(2 * adjustedJumpHeight * Mathf.Abs(Physics.gravity.y) * enemy.rb.gravityScale);

        // Apply leap force
        enemy.rb.linearVelocity = new Vector2(horizontalSpeed, verticalSpeed);

        // Enable hitbox at peak jump
        yield return new WaitUntil(() => enemy.rb.linearVelocity.y <= 0);
        enemy.leapHitbox.enabled = true;

        // Timeout failsafe
        float timer = 0f;
        while (!enemy.IsGrounded() && timer < leapTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure enemy has landed
        enemy.leapHitbox.enabled = true;
        enemy.cameraController.StartShake(CameraController.ShakeLevel.medium);
        enemy.DealDamageToPlayer(40, Vector2.right, 15f, enemy.leapHitbox);

        yield return new WaitForSeconds(0.4f); // Damage window
        enemy.leapHitbox.enabled = false;

        // Pause on landing
        enemy.rb.linearVelocity = Vector2.zero; // Stop movement
        enemy.rb.gravityScale = 1f;
        yield return new WaitForSeconds(0.5f); // Pause before state change

        isLeaping = false;
    }


    public override void Update() {
        if (isLeaping == false) {
            if (enemy.distanceFromPlayer <= enemy.slashRange && enemy.IsPlayerInFront()) {
                enemy.ChangeState(new BasicEnemySlashState(enemy)); // Attack if close
            }
            if (enemy.distanceFromPlayer <= enemy.backRange && !enemy.IsPlayerInFront() && enemy.ShouldBackAtk()) {
                enemy.ChangeState(new BasicEnemyBackState(enemy));
            }
            if (enemy.distanceFromPlayer <= enemy.detectionRange) {
                enemy.ChangeState(new BasicEnemyChaseState(enemy)); // Chase if in range
            }
            else {
                enemy.ChangeState(new BasicEnemyIdleState(enemy)); // Go idle
            }
        }
    }

    public override void Exit()
    {
        enemy.nonParryWarning.SetActive(false);
        enemy.leapHitbox.enabled = false; // Ensure hitbox is off
    }
}
