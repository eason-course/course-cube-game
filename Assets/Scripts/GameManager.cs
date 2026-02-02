using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private Transform spawnPoint;

    [Header("UI References")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    private PlayerController player;
    private int score = 0;
    private int totalCollectibles = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        totalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;

        // Subscribe to events
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
        Collectible.OnCollected += HandleCollectibleCollected;

        // Set spawn point if not assigned
        if (spawnPoint == null && player != null)
        {
            GameObject spawn = new GameObject("SpawnPoint");
            spawn.transform.position = player.transform.position;
            spawnPoint = spawn.transform;
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
        Collectible.OnCollected -= HandleCollectibleCollected;
    }

    private void Update()
    {
        // Restart with R key
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player died!");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    private void HandleCollectibleCollected(Collectible collectible)
    {
        score++;
        Debug.Log($"Score: {score}/{totalCollectibles}");
        UpdateUI();

        if (score >= totalCollectibles)
        {
            Debug.Log("Level Complete!");
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Gold: {score}/{totalCollectibles}";
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RespawnPlayer()
    {
        if (player != null && spawnPoint != null)
        {
            player.Respawn(spawnPoint.position);

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }
        }
    }

    public int Score => score;
    public int TotalCollectibles => totalCollectibles;
}
