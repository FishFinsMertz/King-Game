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
        
    }
}
