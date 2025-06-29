using UnityEngine;

public class KingFlyStrikeState : KingState
{
    public KingFlyStrikeState(KingController king) : base(king) { }

    public override void Enter()
    {
        // Perform attack
        king.Flip();
        king.rb.linearVelocity = new Vector2(0, king.rb.linearVelocity.y);
        king.isAttacking = true;
    }
}
