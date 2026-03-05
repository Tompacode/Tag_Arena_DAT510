using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public string PlayerID;

    [Header("Stats")]
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int stamina = 100;
    [SerializeField]
    private float speed = 5f;
    [SerializeField] [Range(0f, 1f)]
    private float blockDamageMultiplier = 0.5f;
    [SerializeField]
    private float attackCooldown = 0.5f;

    [Header("Movement Settings")]
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
    private bool isBlocking;
    private bool isDead;
    private float nextAttackTime;
    private Transform opponent;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        if (CompareTag("Player1"))
        {
            PlayerID = "Player1";
            // Player1 faces right (default rotation)
            transform.rotation = Quaternion.Euler(0f, 90f, 0);
        }
        else if (CompareTag("Player2"))
        {
            PlayerID = "Player2";
            // Player2 faces left (180 degrees)
            transform.rotation = Quaternion.Euler(0f, -90f, 0);
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        horizontalInput = Input.GetAxisRaw(PlayerID + "_" + "Horizontal");
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown(PlayerID + "_" + "Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown(PlayerID + "_" + "Attack"))
        {
            TriggerAttack();
        }

        if (Input.GetButtonDown(PlayerID + "_" + "Block"))
        {
            StartBlock();
        }

        if (Input.GetButtonUp(PlayerID + "_" + "Block"))
        {
            StopBlock();
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(horizontalInput * speed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = movement;
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    private void TriggerAttack()
    {
        if (animator == null)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        animator.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

    private void StartBlock()
    {
        SetBlocking(true);
    }

    private void StopBlock()
    {
        SetBlocking(false);
    }

    private void SetBlocking(bool value)
    {
        isBlocking = value;

        if (animator != null)
        {
            animator.SetBool("IsBlocking", isBlocking);
        }
    }

    public void SetOpponent(Transform opp)
    {
        opponent = opp;
    }

    public void TakeDamage(int amount, string attackerTag)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(attackerTag) && CompareTag(attackerTag))
        {
            return;
        }

        float modifier = isBlocking ? blockDamageMultiplier : 1f;
        int finalDamage = Mathf.Max(1, Mathf.CeilToInt(amount * modifier));

        health = Mathf.Max(health - finalDamage, 0);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator?.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        animator?.SetTrigger("Die");

        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        PlayerManager.Instance?.OnPlayerDeath(this);
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetFloat("VelocityY", rb.linearVelocity.y);
        }
    }

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
        return health;
    }

    public int getStamina()
    {
        return stamina;
    }
}
