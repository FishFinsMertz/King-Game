using System.Collections;
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
    public float ascentSpeed;
    public float teleportProbability; // UPDATE WHEN WE MAKE A "PHASE 2"

    [Header("Hitboxes & Utility")]
    public Collider2D thrustHitbox;
    public Collider2D megaSlamHitbox;
    public Collider2D flyStrikeHitbox;
    public BoxCollider2D swordSpawnArea;
    public ObjPool swordPool;
    public EnemyTeleporter enemyTeleporter;

    [Header("Attack Stats")]
    // Thrust
    public float thrustChargeTime;
    public float thrustDuration;
    public float thrustDmg;
    public float thrustFreezeDuration;
    //Mega Slam
    public float megaSlamChargeTime;
    public float megaSlamDuration;
    public float megaSlamDmg;
    public float megaSlamFreezeDuration;
    public float megaSlamCoolDown;
    // Fly Strike
    public float flyHoverTime;
    public float flyStrikeDuration;
    public float flyStrikeDmg;
    public float flyStrikeFreezeDuration;
    public float flyStrikeCoolDown;
    public float flyStrikeSpeed;
    public float flyHeight;
    // Sword Barrage
    public int swordBarrageAmount;
    public float swordBarrageInterval;
    public float swordBarrageIntervalOffset;
    public float swordBarrageChargeTime;
    public float swordBarrageCoolDown;

    [Header("Misc")]
    public GameObject nonParryWarning;
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
    private float lastSpawnX = float.NaN; // Track last spawned x, initialize as NaN so first spawn always allowed

    // Attack cooldowns
    [HideInInspector] public bool canMegaSlam = true;
    [HideInInspector] public bool canFlyStrike = true;
    [HideInInspector] public bool canSwordBarrage = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraController = FindAnyObjectByType<CameraController>();
        nonParryWarning.SetActive(false);

        ChangeState(new KingIdleState(this));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(IsGrounded());
        //Debug.Log(currentState);
        //Debug.Log(isAttacking);
        //Debug.Log(canMegaSlam);
        currentState.Update();

        if (Input.GetKeyDown(KeyCode.V))
        { 
            enemyTeleporter.TryTeleport(3f, 0.5f); //Teleport distance and probability of teleporting
        }

        // Get info about player
        if (player != null)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            vectorFromPlayer = player.transform.position - transform.position;
        }

        // Flipping logic
        if (!isAttacking)
        {
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

    // Attack Utility

    public IEnumerator PerformSwordBarrage()
    {
        int curSwordAmt = 0;
        while (curSwordAmt < swordBarrageAmount)
        {
            yield return new WaitForSeconds(swordBarrageInterval + Random.Range(-1f * swordBarrageIntervalOffset, swordBarrageIntervalOffset));
            Vector2 spawnLocation = GetRandomSpawnPosition();

            //Debug.Log("Spawning sword at: " + spawnLocation);
            swordPool.SpawnObject(spawnLocation);
            curSwordAmt++;
        }
    }

    public Vector2 GetRandomSpawnPosition()
    {
        Vector2 center = swordSpawnArea.bounds.center;
        Vector2 extents = swordSpawnArea.bounds.extents;

        float x;
        int attempts = 0;
        const int maxAttempts = 10;  // Avoid infinite loops

        do
        {
            x = Random.Range(center.x - extents.x, center.x + extents.x);
            attempts++;
        }
        while (!float.IsNaN(lastSpawnX) && Mathf.Abs(x - lastSpawnX) < 2f && attempts < maxAttempts);

        lastSpawnX = x;

        float y = Random.Range(center.y - extents.y, center.y + extents.y);

        return new Vector2(x, y);
    }

    // Attack cooldowns
    public IEnumerator StartMegaSlamCoolDown()
    {
        canMegaSlam = false;
        yield return new WaitForSeconds(megaSlamCoolDown);
        canMegaSlam = true;
    }

    public IEnumerator StartFlyStrikeCoolDown()
    {
        canFlyStrike = false;
        yield return new WaitForSeconds(flyStrikeCoolDown);
        canFlyStrike = true;
    }

    public IEnumerator StartSwordbarrageCoolDown()
    {
        canSwordBarrage = false;
        yield return new WaitForSeconds(swordBarrageCoolDown);
        canSwordBarrage = true;
    }
}
