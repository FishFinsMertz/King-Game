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
        
        else if (king.CompareAtkProbability(king.spikeProbability) && king.canSpike && king.GetPlayerDistance() <= king.midRange
        && !king.IsPlayerInFront() && !king.isAttacking)
        {
            king.ChangeState(new KingSpikeState(king));
        }

        else if (king.CompareAtkProbability(king.megaSlamProbability) && king.canMegaSlam && !king.isAttacking &&
        king.GetPlayerDistance() <= king.midRange && king.IsPlayerInFront())
        {
            king.ChangeState(new KingMegaSlamState(king));
        }

        else if (king.CompareAtkProbability(king.rumbleProbability) && king.canRumble && !king.isAttacking &&
        king.GetPlayerDistance() <= king.midRange)
        {
            king.ChangeState(new KingRumbleState(king));
        }

        else if (king.CompareAtkProbability(king.thrustProbability) && king.GetPlayerDistance() <= king.closeRange
        && king.IsPlayerInFront() && !king.isAttacking)
        {
            king.ChangeState(new KingThrustState(king));
        }

        else if (king.CompareAtkProbability(king.rushProbability) && king.canRush && !king.isAttacking && king.GetPlayerDistance() > king.closeRange &&
        king.GetPlayerDistance() <= king.longRange && king.IsPlayerInFront())
        {
            king.ChangeState(new KingRushState(king));
        }

        else if (king.CompareAtkProbability(king.flyStrikeProbability) && king.canFlyStrike && !king.isAttacking && king.GetPlayerDistance() > king.closeRange &&
        king.GetPlayerDistance() <= king.longRange)
        {
            king.ChangeState(new KingFlyStrikeState(king));
        }

        else if (king.CompareAtkProbability(king.swordBarrageProbability) && king.canSwordBarrage && !king.isAttacking && king.GetPlayerDistance() > king.midRange &&
        king.GetPlayerDistance() <= king.longRange)
        {
            king.ChangeState(new KingSwordBarrage(king));
        }
    }

    public override void Exit()
    {
        king.animator.SetBool("isWalking", false);
    }
}
