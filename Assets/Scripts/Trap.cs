using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private TrapType trapType = TrapType.Spike;
    [SerializeField] private float damageDelay = 0f;

    [Header("Animation (for moving traps)")]
    [SerializeField] private bool isAnimated = false;
    [SerializeField] private Vector3 moveDirection = Vector3.up;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 2f;

    public enum TrapType
    {
        Spike,
        Lava,
        MovingPlatform
    }

    private Vector3 startPosition;
    private float moveProgress = 0f;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isAnimated)
        {
            AnimateTrap();
        }
    }

    private void AnimateTrap()
    {
        moveProgress += Time.deltaTime * moveSpeed;
        float offset = Mathf.Sin(moveProgress) * moveDistance;
        transform.position = startPosition + moveDirection.normalized * offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && !player.IsDead)
        {
            if (damageDelay > 0)
            {
                StartCoroutine(DelayedDamage(player));
            }
            else
            {
                player.Die();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && !player.IsDead)
        {
            if (damageDelay > 0)
            {
                StartCoroutine(DelayedDamage(player));
            }
            else
            {
                player.Die();
            }
        }
    }

    private System.Collections.IEnumerator DelayedDamage(PlayerController player)
    {
        yield return new WaitForSeconds(damageDelay);
        if (player != null && !player.IsDead)
        {
            player.Die();
        }
    }
}
