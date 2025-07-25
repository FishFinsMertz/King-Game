using UnityEngine;

public class ParryHitbox : MonoBehaviour
{
    private PlayerController player;
    private Camera mainCamera;
    private CameraController camController;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        mainCamera = Camera.main;
        camController = mainCamera.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Parrying");
        //Debug.Log(other.name);
        if (player.currentState is PlayerParryState && player.gameObject.layer == LayerMask.NameToLayer("Invulnerable") &&
        other.CompareTag("ParryableAttack"))
        {
            //Debug.Log("Parry Successful!");
            if (camController)
            {
                camController.StartShake(CameraController.ShakeLevel.light);
            }
            player.energyManager.ChargeEnergy(player.parryChargeAmt);
        }
    }
}
