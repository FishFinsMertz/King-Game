using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    public float speed = 8f;
    public float jumpingPower = 16f;
    public float fallMultiplier = 4f;
    public int maxJumpCap = 5;
    public int jumpCounter = 0;
    public float airControlSpeed = 6f; 


    public Transform groundCheck;
    public LayerMask groundLayer;

    private PlayerState currentState;
    private bool isFacingRight = true;

    public float InputX => Input.GetAxisRaw("Horizontal");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        currentState.Update();
        Flip();
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
        if (grounded) jumpCounter = 0;
        return grounded;
    }

    private void Flip()
    {
        if (isFacingRight && InputX < 0f || !isFacingRight && InputX > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
