using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public string PlayerID;

    [Header("Stats")]
    [SerializeField] private int health = 100;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private float staminaRegenPerSecond = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] [Range(0f, 1f)] private float blockDamageMultiplier = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackStaminaCost = 10;
    [SerializeField] private float blockStaminaDrainPerSecond = 10f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Input")]
    [SerializeField] private float inputPressThreshold = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private WeaponDamage weapon;

    [Header("Audio")]
    [SerializeField] private AudioClip blockClip;
    [SerializeField] [Range(0f, 1f)] private float blockVolume = 1f;

    private int maxHealth;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private float horizontalInput;
    private bool isBlocking;
    private bool isDead;
    private float nextAttackTime;

    private bool wasAttackPressed;
    private bool wasBlockPressed;

    private void Awake()
    {
        maxHealth = health;
    }

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
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (CompareTag("Player2"))
        {
            PlayerID = "Player2";
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        stamina = Mathf.Min(stamina, maxStamina);

        if (weapon != null)
        {
            weapon.DisableHitBox();
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        horizontalInput = Input.GetAxisRaw(PlayerID + "_" + "Horizontal");
        isGrounded = groundCheck != null && Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown(PlayerID + "_" + "Jump") && isGrounded)
        {
            Jump();
        }

        float attackAxis = Input.GetAxisRaw(PlayerID + "_" + "Attack");
        float blockAxis = Input.GetAxisRaw(PlayerID + "_" + "Block");

        bool attackPressed = attackAxis > inputPressThreshold;
        bool blockPressed = blockAxis > inputPressThreshold;

        if (attackPressed && !wasAttackPressed)
        {
            TriggerAttack();
        }

        if (blockPressed && !wasBlockPressed)
        {
            StartBlock();
        }
        else if (!blockPressed && wasBlockPressed)
        {
            StopBlock();
        }

        wasAttackPressed = attackPressed;
        wasBlockPressed = blockPressed;

        DrainBlockStamina();
        RegenerateStamina();
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
        if (rb == null)
        {
            return;
        }

        Vector3 movement = new Vector3(horizontalInput * speed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = movement;
    }

    private void Jump()
    {
        animator?.SetTrigger("Jump");

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
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

        if (!TrySpendStamina(attackStaminaCost))
        {
            return;
        }

        animator.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
    }

    private void StartBlock()
    {
        if (stamina <= 0f)
        {
            return;
        }

        SetBlocking(true);
    }

    private void StopBlock()
    {
        SetBlocking(false);
    }

    private void SetBlocking(bool value)
    {
        isBlocking = value;
        animator?.SetBool("IsBlocking", isBlocking);
    }

    private void DrainBlockStamina()
    {
        if (!isBlocking)
        {
            return;
        }

        float cost = blockStaminaDrainPerSecond * Time.deltaTime;
        if (!TrySpendStamina(cost))
        {
            StopBlock();
        }
    }

    private void RegenerateStamina()
    {
        if (isBlocking || stamina >= maxStamina)
        {
            return;
        }

        float gain = staminaRegenPerSecond * Time.deltaTime;
        stamina = Mathf.Min(maxStamina, stamina + gain);
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

        bool blockedSuccessfully = isBlocking && finalDamage < amount;
        if (blockedSuccessfully)
        {
            PlayBlockSfx();
        }

        health = Mathf.Max(health - finalDamage, 0);

        if (health <= 0)
        {
            Die();
        }
    }

    private void PlayBlockSfx()
    {
        if (blockClip == null || AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlaySfxAtPoint(blockClip, transform.position, blockVolume);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (weapon != null)
        {
            weapon.DisableHitBox();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        enabled = false;

        PlayerManager.Instance?.OnPlayerDeath(this);
    }

    private void UpdateAnimations()
    {
        if (animator == null || rb == null)
        {
            return;
        }

        animator.SetFloat("VelocityX", -horizontalInput);
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
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

    public int getMaxHealth()
    {
        return maxHealth > 0 ? maxHealth : health;
    }

    public int getStamina()
    {
        return Mathf.RoundToInt(stamina);
    }

    private bool TrySpendStamina(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        if (stamina < amount)
        {
            return false;
        }

        stamina = Mathf.Max(0f, stamina - amount);
        return true;
    }

    public void AE_EnableHitBox()
    {
        if (weapon != null)
        {
            weapon.EnableHitBox();
        }
    }

    public void AE_DisableHitBox()
    {
        if (weapon != null)
        {
            weapon.DisableHitBox();
        }
    }
}
