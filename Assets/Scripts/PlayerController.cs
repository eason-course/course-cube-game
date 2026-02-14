using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 20f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float deceleration = 10f;

    [Header("Jump Settings")]
    [SerializeField] private bool enableJump = true;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private float riseMultiplier = 5f;
    [SerializeField] private float fallMultiplier = 8f;

    [Header("Death Settings")]
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private float fallDeathY = -100f;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float invincibilityTime = 0.1f;

    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 spawnPosition;
    private bool isDead = false;
    private bool isGrounded = false;

    public static event System.Action OnPlayerDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Lock rotation so the cube doesn't tumble, allow gravity (Y axis free)
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Record spawn position
        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (isDead) return;

        // Fall death check
        if (transform.position.y < fallDeathY)
        {
            Die();
            return;
        }

        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // Get WASD input using new Input System
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.wKey.isPressed) vertical = 1f;
        if (keyboard.sKey.isPressed) vertical = -1f;
        if (keyboard.aKey.isPressed) horizontal = -1f;
        if (keyboard.dKey.isPressed) horizontal = 1f;

        movement = new Vector3(horizontal, 0f, vertical).normalized;

        // Jump
        if (enableJump && keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (movement.magnitude > 0.1f)
        {
            // Apply force for movement
            rb.AddForce(movement * moveForce, ForceMode.Force);

            // Limit horizontal speed
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }
        else
        {
            // Decelerate when no input
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        // Faster jump (quick rise, quick fall)
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0)
            {
                // Rising - apply extra gravity to shorten rise time
                rb.AddForce(Vector3.down * riseMultiplier, ForceMode.Acceleration);
            }
            else
            {
                // Falling - apply stronger gravity for fast descent
                rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Spawn death particle effect
        if (deathParticlePrefab != null)
        {
            GameObject particles = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, particleLifetime);
        }

        // Play death sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        OnPlayerDeath?.Invoke();

        // Respawn at start position
        transform.position = spawnPosition;
        rb.linearVelocity = Vector3.zero;

        Invoke(nameof(ResetDead), invincibilityTime);
    }

    private void ResetDead()
    {
        isDead = false;
    }

    public bool IsDead => isDead;
}
