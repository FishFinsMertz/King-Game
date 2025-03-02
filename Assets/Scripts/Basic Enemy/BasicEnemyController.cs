using System.Collections;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    // Attack
    public bool isAttacking = false;
    public float slashTimer = 1f;
    public float leapChance;
    private float leapCheckInterval = 3f;
    private bool shouldLeap = false;

    // Hitboxes
    public Collider2D slashHitbox;
    public Collider2D leapHitbox;

    // Movement and detection
    public float chaseSpeed;
    public float detectionRange;
    public float slashRange;
    public float minThreshold;
    public float minLeapRange;
    public float maxLeapRange;

    // Other stuff
    public Transform groundCheck;
    public LayerMask groundLayer;

    public CameraController cameraController;

    private BasicEnemyState currentState;

    // Where enemy is facing
    public int isFacingRight = 1;

    // Player information
    public GameObject player; // Declare player as a private field
    public float distanceFromPlayer; // Store the distance
    public Vector2 vectorFromPlayer;
      
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        cameraController = FindAnyObjectByType<CameraController>();

        ChangeState(new BasicEnemyIdleState(this));

        StartCoroutine(DetermineLeapBehavior());
    }

    void Update()
    {
        currentState.Update();
        if (!isAttacking) {
            Flip();
        }

        if (player != null)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            vectorFromPlayer = player.transform.position - transform.position;
            //Debug.Log("Vector from player: " + vectorFromPlayer);
            //Debug.Log("BasicEnemyController.cs: Distance from Player: " + distanceFromPlayer);
            //Debug.Log(isFacingRight);
        }
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void ChangeState(BasicEnemyState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return grounded;
    }

    public bool IsPlayerInFront() {
        float directionToPlayer = vectorFromPlayer.x;
        return (isFacingRight == 1 && directionToPlayer > 0) || (isFacingRight == -1 && directionToPlayer < 0);
    }

    public bool playerInLeapRange() {
        return distanceFromPlayer <= maxLeapRange && distanceFromPlayer >= minLeapRange;
    }

    private IEnumerator DetermineLeapBehavior()
    {
        while (true)
        {
            shouldLeap = Random.value < leapChance; 
            //Debug.Log("Should Leap: " + shouldLeap);
            yield return new WaitForSeconds(leapCheckInterval);
        }
    }

    public bool ShouldLeap()
    {
        return shouldLeap;
    }

    private void Flip()
    {
        if (distanceFromPlayer > minThreshold) {
            if ((isFacingRight == 1 && vectorFromPlayer.x < 0f) || (isFacingRight == -1 && vectorFromPlayer.x > 0f))
            {
                isFacingRight *= (-1);
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public void DealDamageToPlayer(float damage, Vector2 hitDirection, float knockbackForce, Collider2D hitbox)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitbox.bounds.center, hitbox.bounds.size, 0);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player")) // Check layer instead of tag
            {
                PlayerHealthManager playerHealth = hit.GetComponent<PlayerHealthManager>();

                if (playerHealth != null)
                {
                    if (hit.gameObject.layer == LayerMask.NameToLayer("Invulnerable")) // Don't deal damage if invulnerable
                        continue;

                    if (hitDirection == Vector2.left || hitDirection == Vector2.right)
                    {
                        hitDirection = (hit.transform.position - transform.position).normalized;
                    }
                    playerHealth.TakeDamage(damage, hitDirection, knockbackForce);
                }
            }
        }
    }
}
