using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class KingSword : PooledObjects
{
    [Header("Sword Stats")]
    public float launchSpeed;
    public float hoverTime;
    public float chargeSpeed;
    public float chargeTime;
    public float LandingTime;

    [Header("Misc")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject projectileHitbox; 

    // Private and Hidden Variables
    private ObjPool pool;
    private Rigidbody2D rb;

    public override void SetPool(ObjPool objectPool)
    {
        pool = objectPool;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void OnEnable()
    {
        if (projectileHitbox != null)
            projectileHitbox.SetActive(true);

        StartCoroutine(Launch());
    }

    private IEnumerator Launch()
    {
        rb.linearVelocity = Vector2.zero;

        // Hover phase
        yield return new WaitForSeconds(hoverTime);

        // Rise phase
        rb.linearVelocity = Vector2.up * chargeSpeed;
        yield return new WaitForSeconds(chargeTime);

        // Fall phase
        rb.linearVelocity = Vector2.down * launchSpeed;

        // Wait until grounded
        yield return new WaitUntil(() => Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer));

        // Disable the hitbox when landed
        if (projectileHitbox != null)
            projectileHitbox.SetActive(false);

        // Delay before recycling
        yield return new WaitForSeconds(LandingTime);

        pool.RecycleObject(gameObject);
    }
}
