using UnityEngine;

public class ParryHitbox : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Parrying");
        //Debug.Log("Current State: " + player.currentState.GetType());
        //Debug.Log("Player Layer: " + LayerMask.LayerToName(player.gameObject.layer));
        //Debug.Log("Other Tag: " + other.tag);
        if (player.currentState is PlayerParryState && player.gameObject.layer == LayerMask.NameToLayer("Invulnerable") &&
        other.CompareTag("ParryableAttack"))
        {
            Debug.Log("Parry Successful!");
            player.energyManager.ChargeEnergy(player.parryChargeAmt);
        }
    }
}
