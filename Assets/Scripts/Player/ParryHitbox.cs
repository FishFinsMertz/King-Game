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
        if (player.currentState is PlayerParryState && player.gameObject.layer == LayerMask.NameToLayer("Invulnerable") && 
        other.CompareTag("ParryableAttack"))
        {
            Debug.Log("Parry Successful!");
            player.energyManager.ChargeEnergy(20);
        }
    }
}
