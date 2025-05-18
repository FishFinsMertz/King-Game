using System;
using System.Collections;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    // Attack
    [Header ("Attack Info")]
    [HideInInspector] public bool isAttacking = false;
    public float slashTimer = 1f;
    public float slashChargeTime = 0.5f;
    public float leapChance;
    private float leapCheckInterval = 3f;
    private bool shouldLeap = false;
    public float backAtkChance;
    private float backCheckInterval = 3f;
    private bool shouldBackAtk = false;
    public float backAtkDelay;
    public float backAtkTimer;

    // Hitboxes
    [Header ("Hitboxes")]
    public Collider2D slashHitbox;
    public Collider2D leapHitbox;
    public Collider2D backHitbox;

    [Header ("Movement & Detection")]
    // Movement and detection
    public float chaseSpeed;
    public float detectionRange;
    public float slashRange;
    public float minThreshold;
    public float minLeapRange;
    public float maxLeapRange;
    public float backRange;

    [Header("Attack Warning")]
    public GameObject nonParryWarning;

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

    [Header("Animation")]
    public Animator animator;
      
    void Start() {
        nonParryWarning.SetActive(false);
        slashHitbox.enabled = false;
        leapHitbox.enabled = false;
        rb = GetComponent<Rigidbody2D>();

        cameraController = FindAnyObjectByType<CameraController>();

        ChangeState(new BasicEnemyIdleState(this));

        StartCoroutine(DetermineLeapBehavior());
        StartCoroutine(DetermineBackBehavior());

        // Add slight variance to enemy chaseSpeed
        //float multiplier = Random.value;
        //chaseSpeed += multiplier;
    }

    void Update() {

        //Debug.Log(currentState);
        //Debug.Log(IsPlayerInFront());
        Debug.Log(ShouldBackAtk());
        currentState.Update();
        if (!isAttacking && !(currentState is BasicEnemyBackState)) {
            Flip();
        }

        if (player != null)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            vectorFromPlayer = player.transform.position - transform.position;
        }
    }

    void FixedUpdate() {
        currentState.FixedUpdate();
    }

    public void ChangeState(BasicEnemyState newState) {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public bool IsGrounded() {
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

    private IEnumerator DetermineLeapBehavior() {
        while (true) {
            shouldLeap = UnityEngine.Random.value < leapChance; 
            //Debug.Log("Should Leap: " + shouldLeap);
            yield return new WaitForSeconds(leapCheckInterval);
        }
    }

    public bool ShouldLeap() {
        return shouldLeap;
    }

    private IEnumerator DetermineBackBehavior() {
        while (true) {
            shouldBackAtk = UnityEngine.Random.value < backAtkChance; 
            //Debug.Log("Should Back Atk: " + shouldBackAtk);
            yield return new WaitForSeconds(backCheckInterval);
        }
    }
    
    public bool ShouldBackAtk() {
        return shouldBackAtk;
    }

    private void Flip() {
        if (distanceFromPlayer > minThreshold) {
            if ((isFacingRight == 1 && vectorFromPlayer.x < 0f) || (isFacingRight == -1 && vectorFromPlayer.x > 0f))
            {
                isFacingRight *= (-1);
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public void DealDamageToPlayer(float damage, Vector2 hitDirection, float knockbackForce, Collider2D hitbox) {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitbox.bounds.center, hitbox.bounds.size, 0);
        //Debug.Log(hitbox.name);
        //Debug.Log(damage);

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
                }
            }
        }
    }
}
