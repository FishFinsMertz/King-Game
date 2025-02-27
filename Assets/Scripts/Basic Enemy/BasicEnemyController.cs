using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    // Movement
    public float chaseSpeed;
    public float detectionRange;

    // Other stuff
    public Transform groundCheck;
    public LayerMask groundLayer;

    private BasicEnemyState currentState;
    public int isFacingRight = 1;

    // Player information
    private GameObject player; // Declare player as a private field
    public float distanceFromPlayer; // Store the distance
    public Vector2 vectorFromPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

         ChangeState(new BasicEnemyIdleState(this));
    }

    void Update()
    {
        currentState.Update();
        Flip();

        if (player != null)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            vectorFromPlayer = player.transform.position - transform.position;
            //Debug.Log("Vector from player: " + vectorFromPlayer);
            //Debug.Log("BasicEnemyController.cs: Distance from Player: " + distanceFromPlayer);
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

    private void Flip()
    {
        if ((isFacingRight == 1 && vectorFromPlayer.x < 0f) || (isFacingRight == -1 && vectorFromPlayer.x > 0f))
        {
            isFacingRight *= (-1);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
