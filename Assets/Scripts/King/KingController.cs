using Unity.VisualScripting;
using UnityEngine;

public class KingController : MonoBehaviour
{
    [Header("important")]
    public GameObject player;

    [Header("Detection")]
    public float detectionRange;
    public float minFlipThreshold;
    public float closeRange;
    public float midRange;
    public float longRange;

    [Header("Movement")]
    public float chaseSpeed;

    [Header("Hitboxes")]
    public Collider2D thrustHitbox;

    [Header("Attack Stats")]
    public float thrustChargeTime;
    public float thrustDuration;
    public float thrustDmg;
    public float thrustFreezeDuration;

    [Header("Misc")]
    [HideInInspector] public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    public HitStop hitstop;

    // Private / hidden variables
    private KingState currentState;
    private float distanceFromPlayer;
    private Vector2 vectorFromPlayer;
    private int isFacingRight = 1;
    [HideInInspector] public CameraController cameraController;
    [HideInInspector] public bool isAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraController = FindAnyObjectByType<CameraController>();

        ChangeState(new KingIdleState(this));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(IsGrounded());
        //Debug.Log(currentState);
        //Debug.Log(isAttacking);
        currentState.Update();

        // Get info about player
        if (player != null)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            vectorFromPlayer = player.transform.position - transform.position;
        }

        // Flipping logic
        if (!isAttacking) {
            Flip();
        }
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void ChangeState(KingState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public float GetPlayerDistance()
    {
        return distanceFromPlayer;
    }

    public Vector2 GetVectorToPlayer()
    {
        return vectorFromPlayer;
    }

    public bool IsPlayerInFront()
    {
        float directionToPlayer = vectorFromPlayer.x;
        return (isFacingRight == 1 && directionToPlayer > 0) || (isFacingRight == -1 && directionToPlayer < 0);
    }

    public void Flip()
    {
        if (distanceFromPlayer > minFlipThreshold)
        {
            if ((isFacingRight == 1 && vectorFromPlayer.x < 0f) || (isFacingRight == -1 && vectorFromPlayer.x > 0f))
            {
                isFacingRight *= (-1);
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return grounded;
    }
    
        public int DealDamageToPlayer(float damage, Vector2 hitDirection, float knockbackForce, Collider2D hitbox)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitbox.bounds.center, hitbox.bounds.size, 0);
        //Debug.Log(hitbox.name);
        //Debug.Log(damage);

        bool hitPlayer = false;

        foreach (Collider2D hit in hits)
        {

            if (hit.gameObject.layer == LayerMask.NameToLayer("Player") ||
            (hit.gameObject.layer == LayerMask.NameToLayer("Invulnerable") && !hitbox.CompareTag("ParryableAttack")))
            {
                PlayerHealthManager playerHealth = hit.GetComponent<PlayerHealthManager>();

                if (playerHealth != null)
                {
                    if (hitDirection == Vector2.left || hitDirection == Vector2.right)
                    {
                        hitDirection = (hit.transform.position - transform.position).normalized;
                    }
                    playerHealth.TakeDamage(damage, hitDirection, knockbackForce);
                    hitPlayer = true;
                }
            }
        }
        return hitPlayer ? 0 : 1;
    }
}
