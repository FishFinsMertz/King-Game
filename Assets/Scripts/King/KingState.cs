using UnityEngine;

public abstract class KingState
{
    protected KingController king;

    public KingState(KingController king)
    {
        this.king = king;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
