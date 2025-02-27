using UnityEngine;

public abstract class BasicEnemyState
{
    protected BasicEnemyController enemy;

    public BasicEnemyState(BasicEnemyController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
