using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    // Attack
    public bool isAttacking = false;
    public float slashDelay = 5f;
    public float slashTimer = 1f;

    // Movement and detection
    public float chaseSpeed;
    public float detectionRange;
    public float slashRange;
    public float minThreshold;

    // Other stuff
    public Transform groundCheck;
    public LayerMask groundLayer;

    public CameraController cameraController;

    private BasicEnemyState currentState;

    // Where enemy is facing
    public int isFacingRight = 1;

    // Player information
    private GameObject player; // Declare player as a private field
    public float distanceFromPlayer; // Store the distance
    public Vector2 vectorFromPlayer;

    [System.Obsolete]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        cameraController = FindObjectOfType<CameraController>();

         ChangeState(new BasicEnemyIdleState(this));
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
}
