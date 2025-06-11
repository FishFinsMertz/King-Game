using UnityEngine;

public class KingWalkState : KingState
{
    public KingWalkState(KingController king) : base(king) { }

    private int moveDirection;
    public override void Enter()
    {
        king.animator.SetBool("isWalking", true);
    }

    public override void FixedUpdate()
    {
        // If the boss is further than the minimum threshold, update direction
        if (king.GetPlayerDistance() > king.minFlipThreshold)
        {
            moveDirection = king.GetVectorToPlayer().x < 0 ? -1 : 1;
        }

        // Move the enemy in the stored direction
        king.rb.linearVelocity = new Vector2(king.chaseSpeed * moveDirection, king.rb.linearVelocity.y);
    }

    public override void Update()
    {
        if (king.GetPlayerDistance() > king.detectionRange)
        {
            king.ChangeState(new KingIdleState(king));
        }
    }

    public override void Exit()
    {
        king.animator.SetBool("isWalking", false);
    }
}
