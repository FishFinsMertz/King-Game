using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    // Player movement speed
    public float speed;
    public float jumpingPower;
    public float fallMultiplier;
    public int maxJumpCap;
    public float dashPower;

    // Other stuff
    public Transform groundCheck;
    public LayerMask groundLayer;

    private PlayerState currentState;
    public int isFacingRight = 1;

    public float InputX => Input.GetAxisRaw("Horizontal");

    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        rb = GetComponent<Rigidbody2D>();
        ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        currentState.Update();
        if (!(currentState is PlayerKnockbackState)) {
            Flip();
        }
        //Debug.Log(currentState);
        //Debug.Log("Is grounded: " + IsGrounded());
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
        }
    }
}
