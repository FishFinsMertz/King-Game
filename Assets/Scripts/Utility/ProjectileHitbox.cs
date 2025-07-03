using UnityEngine;

public class ProjectileHitbox : MonoBehaviour
{
    [Header("Projectile stats")]
    [SerializeField] private float damage;
    [SerializeField] private Vector2 hitDirection;
    [SerializeField] private float knockbackForce;

    private CameraController cameraController;
    
    private void Start()
    {
        cameraController = FindAnyObjectByType<CameraController>();

        if (cameraController == null)
        {
            Debug.LogWarning("CameraController not found in scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();

            playerHealth.TakeDamage(damage, hitDirection, knockbackForce);
            cameraController.StartShake(CameraController.ShakeLevel.light);
        }
    }
}
