using System.Collections;
using UnityEngine;

public class KingSword : PooledObjects
{
    [Header("Sword Stats")]
    public float launchSpeed;
    public float hoverTime;
    public float chargeSpeed;
    public float chargeTime;
    public float LandingTime;
    public float maxLifeTime;

    [Header("Misc")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject projectileHitbox;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private GameObject spawnVFXPrefab;
    [SerializeField] private Animator animator;

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

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;
            StartCoroutine(FadeIn());
        }
        StartCoroutine(FailSafeRecycle());
        StartCoroutine(Launch());
    }

    private IEnumerator FailSafeRecycle() 
    {
        yield return new WaitForSeconds(maxLifeTime);
        Debug.Log("KingSword fail-safe triggered: Recycling due to timeout.");
        pool.RecycleObject(gameObject);
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

        // Land
        animator.SetTrigger("Land");

        // Disable the hitbox when landed
        if (projectileHitbox != null)
            projectileHitbox.SetActive(false);

        // Delay before recycling
        yield return new WaitForSeconds(LandingTime);

        pool.RecycleObject(gameObject);
    }

    private IEnumerator FadeIn()
    {
        // Spawn VFX as a detached object in the scene
        GameObject vfxInstance = Instantiate(spawnVFXPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Destroy(vfxInstance, ps.main.duration + ps.main.startLifetime.constant); // clean up
    }

        float time = 0f;
        Color color = spriteRenderer.color;
        while (time < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / fadeInDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure final alpha is 1
        spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
    }
}
