using UnityEngine;
using System.Collections;


public class BasicEnemyBackState : BasicEnemyState
{
    public BasicEnemyBackState(BasicEnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Perform attack
        enemy.rb.linearVelocity = new Vector2(0, enemy.rb.linearVelocity.y);
        enemy.isAttacking = true;
        enemy.StartCoroutine(BackAtk());
    }

    private IEnumerator BackAtk() {
        //Debug.Log("Charging up my slash, delay: " + delay);

        //Animation
        enemy.animator.SetTrigger("Back");
        enemy.audioEmitter.PlaySFX(enemy.backSFX, 0.5f, 0.1f);

        yield return new WaitForSeconds(enemy.backAtkDelay); // Delay before attack hitbox activates
        
        // Enabling hitbox and attack
        enemy.backHitbox.enabled = true;

        // Check for player collision after enabling hitbox
        float success = enemy.DealDamageToPlayer(enemy.backDmg, Vector2.right, 10f, enemy.backHitbox);

        if (success == 0)
        {
            // HitStop
            enemy.hitstop.Freeze(enemy.backFreezeDuration);
        }

        enemy.cameraController.StartShake(CameraController.ShakeLevel.light);
        yield return new WaitForSeconds(enemy.backAtkTimer); // Wait for the attack animation to finish

        // next states
        enemy.ChangeState(new BasicEnemyChaseState(enemy));
    }

    public override void Exit() {
        enemy.isAttacking = false;
        enemy.backHitbox.enabled = false;
    }
}
