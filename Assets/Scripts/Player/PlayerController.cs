using System.Collections;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    [Header("Movement stats")]
    // Player movement 
    public float speed;
    public float jumpingPower;
    public float fallMultiplier;
    public int maxJumpCap;
    public float dashPower;
    public float afterImageCooldown;
    public GameObject afterImagePrefab;
    public GameObject jumpVFXPrefab;
    public float knockbackDuration;
    public float slideDuration;
    public float stunDuration;

    [HideInInspector] public int jumpCounter;

    // Other stuff
    public Transform groundCheck;
    public LayerMask groundLayer;

    public PlayerState currentState;
    [HideInInspector] public int isFacingRight = 1;
    [HideInInspector] public PlayerStaminaManager staminaManager;
    [HideInInspector] public PlayerEnergyManager energyManager;
    [HideInInspector] public PlayerHealthManager healthManager;
    [HideInInspector] public bool canParry = true;

    public float InputX => Input.GetAxisRaw("Horizontal");

    // Combat Stats
    [Header("Combat Stats")]
    public float atkDamage;
    public float atkChargeTime;
    public float atkSpeed;
    public float atkChargeAmt;
    public float critChance;
    public float parryChargeTime;
    public float parryInvulnerableTime;
    public float parryDuration;
    public float parryCoolDownTime;
    public float parryChargeAmt;
    public float shootChargeTime;
    public float shootSpeed;

    [Header("Hitboxes")]
    public Collider2D attackHitbox;
    public Collider2D parryHitbox;
    public Collider2D shootHitbox;

    [Header("Stamina Costs")]
    public float dashCost;
    public float jumpCost;
    public float atkCost;
    public float parryCost;
    public float shootCost;

    [Header("Audio")]
    public AudioClip attackSFX;
    public AudioClip runSFX;
    public AudioClip parrySFX;
    public AudioClip dashSFX;
    public AudioClip jumpSFX;
    public AudioClip shootSFX;
    public AudioClip damagedSFX;

    [Header("Misc")]
    public CameraController cameraController;
    public Animator animator;
    public HitStop hitStop;
    public ParticleSystem shootVFX;
    public Animator slashFXAnimator;

    [HideInInspector] public AudioEmitter audioEmitter;

    void Start()
    {
        // Get stats
        staminaManager = GetComponent<PlayerStaminaManager>();
        energyManager = GetComponent<PlayerEnergyManager>();
        healthManager = GetComponent<PlayerHealthManager>();
        audioEmitter = GetComponent<AudioEmitter>();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        rb = GetComponent<Rigidbody2D>();
        shootHitbox.gameObject.SetActive(false);
        attackHitbox.enabled = false;
        parryHitbox.enabled = false;
        ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        //Debug.Log(currentState);
        //Debug.Log("Current Layer: " + LayerMask.LayerToName(gameObject.layer));
        
        // Kill player if falling off void
        if (math.abs(rb.linearVelocityY) > 100)
        {
            healthManager.TakeDamage(1000, Vector2.zero, 0);
        }

        currentState.Update();
        if ((currentState is PlayerIdleState) || (currentState is PlayerRunningState) || (currentState is PlayerJumpingState)) {
            Flip();
        }
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
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

    private void Flip()
    {
        if ((isFacingRight == 1) && InputX < 0f || (isFacingRight == -1) && InputX > 0f)
        {
            isFacingRight *= (-1);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            // Flip particles also
            shootVFX.transform.localScale = new Vector3(isFacingRight, 1, 1);
        }
    }

    public void DealDamageToEnemy(float damage, Collider2D hitbox) {
        //Debug.Log(hitbox.name);
        //Debug.Log(damage);
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitbox.bounds.center, hitbox.bounds.size, 0);

        // Calculate crit damage
        float finalDmg = damage;
        if (UnityEngine.Random.value <= critChance) {
            finalDmg *= 2f;
        }

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy")) 
            {
                slashFXAnimator.SetTrigger("SlashFX");
                cameraController.StartShake(CameraController.ShakeLevel.light);
                if (hitbox.name == "AttackHitbox")
                {
                    energyManager.ChargeEnergy(atkChargeAmt);
                }
                EnemyHealthManager enemyHealth = hit.GetComponent<EnemyHealthManager>();
                if (enemyHealth != null)
                {
                    //Debug.Log("Attack damage: " + finalDmg);
                    enemyHealth.TakeDamage(finalDmg);
                }
            }
        }
    }

    public IEnumerator StartParryCoolDown() {
        canParry = false;
        yield return new WaitForSeconds(parryCoolDownTime);
        canParry = true;
    }

    public bool CanParry() {
        return canParry;
    }
}
