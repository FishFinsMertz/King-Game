using Unity.VisualScripting;
using UnityEngine;

public class KingController : MonoBehaviour
{
    [Header("important")]
    public GameObject player;

    [Header("Detection")]
    public float detectionRange;
    public float minFlipThreshold;

    [Header("Movement")]
    public float chaseSpeed;

    [Header("Misc")]
    [HideInInspector] public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    public HitStop hitstop;

    // Private variables
    private KingState currentState;
    private float distanceFromPlayer;
    private Vector2 vectorFromPlayer;
    private int isFacingRight = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CameraController cameraController = FindAnyObjectByType<CameraController>();

        ChangeState(new KingIdleState(this));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(IsGrounded());
        currentState.Update();

        // Get info about player
        if (player != null)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
            vectorFromPlayer = player.transform.position - transform.position;
        }

        // Flipping logic
        Flip();
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
    
    public bool IsPlayerInFront() {
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
}
