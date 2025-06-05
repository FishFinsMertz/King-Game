using UnityEngine;

public class KingController : MonoBehaviour
{
    [Header ("Misc")]
    [HideInInspector] public Rigidbody2D rb;
    private KingState currentState;
    public Transform groundCheck;
    public LayerMask groundLayer;

    CameraController cameraController;

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
        currentState.Update();
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
    
    public bool IsGrounded() {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return grounded;
    }
}
