using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.2f;

    [Header("Particle Settings")]
    [SerializeField] private GameObject collectParticlePrefab;
    [SerializeField] private float particleLifetime = 2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip collectSound;

    public static event System.Action<Collectible> OnCollected;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Rotate the collectible
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            Collect();
        }
    }

    private void Collect()
    {
        // Spawn particle effect prefab
        if (collectParticlePrefab != null)
        {
            GameObject particles = Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, particleLifetime);
        }

        // Play sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        OnCollected?.Invoke(this);

        // Hide the collectible
        gameObject.SetActive(false);
    }
}
