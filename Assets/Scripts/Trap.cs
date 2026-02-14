using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float triggerInterval = 2f;
    [SerializeField] private string triggerName = "Activate";

    private Animator animator;
    private float timer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        timer = triggerInterval;
    }

    private void Update()
    {
        if (animator == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            animator.SetTrigger(triggerName);
            timer = triggerInterval;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && !player.IsDead)
        {
            player.Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && !player.IsDead)
        {
            player.Die();
        }
    }
}
