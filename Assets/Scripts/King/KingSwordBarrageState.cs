using System.Collections;
using UnityEngine;

public class KingSwordBarrage : KingState
{
    public KingSwordBarrage(KingController king) : base(king) { }


    public override void Enter()
    {
        king.animator.SetTrigger("Barrage");
        king.isAttacking = true;
        king.StartCoroutine(BarrageRoutine());
    }

    private IEnumerator BarrageRoutine()
    {
        yield return new WaitForSeconds(king.swordBarrageChargeTime);
        //Debug.Log("Barrage incoming");
        king.StartCoroutine(king.PerformSwordBarrage());
        king.StartCoroutine(king.StartSwordbarrageCoolDown());
        king.ChangeState(new KingWalkState(king));
    }

    public override void Exit()
    {
        king.isAttacking = false;
    }
}
