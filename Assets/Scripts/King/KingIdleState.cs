using UnityEngine;

public class KingIdleState : KingState
{
    public KingIdleState(KingController king) : base(king) { }

    public override void Enter()
    {
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
    }

    public override void Update()
    {
        if (king.GetPlayerDistance() <= king.detectionRange)
        {
            king.ChangeState(new KingWalkState(king));
        }

        if (king.GetPlayerDistance() <= king.closeRange && king.IsPlayerInFront())
        {
            king.ChangeState(new KingThrustState(king));
        }
    }
}
