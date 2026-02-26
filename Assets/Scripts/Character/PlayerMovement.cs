using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    
    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 0.2f;
    [SerializeField]
    private LayerMask groundLayer;
    
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private float horizontalInput;
    private bool isFacingRight = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        // Freeze rotation to prevent character from tipping over
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        
        // Handle sprite flipping
        FlipCharacter();
        
        // Update animations
        UpdateAnimations();
    }
    
    void FixedUpdate()
    {
        // Apply movement
        Move();
    }
    
    private void Move()
    {
        // Move on X and Z axis (Z for depth if needed, or keep it 0 for pure side-scroller)
        Vector3 movement = new Vector3(horizontalInput * speed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = movement;
    }
    
    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
    
    private void FlipCharacter()
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }
    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    
    private void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set animation parameters (create these in your Animator Controller)
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VelocityY", rb.linearVelocity.y);
        }
    }
    
    // Visualize ground check in editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    public int getHealth()
    {
        return Random.Range(1, 20);
    }
}
