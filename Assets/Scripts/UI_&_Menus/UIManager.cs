using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;

    [System.Serializable]
    public class Character
    {
        public string characterName;
        public Sprite headshot;
    }

    public Character[] characters;

    public Image player1ActiveHeadshot;
    public Image player2ActiveHeadshot;

    public Image player1BenchHeadshot;
    public Image player2BenchHeadshot;

    public Slider player1ActiveHealth;
    public Slider player2ActiveHealth;

    public Slider player1BenchHealth;
    public Slider player2BenchHealth;

    public Slider player1ActiveStamina; 
    public Slider player2ActiveStamina;

    public Slider player1BenchStamina;
    public Slider player2BenchStamina;

    [Header("Game Over UI")]
    public GameObject gameOverUI;
    public TextMeshProUGUI gameOverText;

    private void Awake()
    {
        if (playerManager == null)
        {
            playerManager = Object.FindFirstObjectByType<PlayerManager>();
        }
        gameOverUI.SetActive(false);
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        GameObject player1Active = playerManager != null ? playerManager.GetActivePlayer1() : null;
        GameObject player1Bench = playerManager != null ? playerManager.GetInactivePlayer1() : null;
        GameObject player2Active = playerManager != null ? playerManager.GetActivePlayer2() : null;
        GameObject player2Bench = playerManager != null ? playerManager.GetInactivePlayer2() : null;

        if (player1Active != null)
        {
            PlayerMovement pm1Active = player1Active.GetComponent<PlayerMovement>();
            if (pm1Active != null)
            {
                player1ActiveHealth.maxValue = pm1Active.getMaxHealth();
                player1ActiveHealth.value = pm1Active.getHealth();
                player1ActiveStamina.value = pm1Active.getStamina();
            }

            player1ActiveHeadshot.sprite = GetHeadshot(player1Active);
        }

        if (player1Bench != null)
        {
            PlayerMovement pm1Bench = player1Bench.GetComponent<PlayerMovement>();
            if (pm1Bench != null)
            {
                player1BenchHealth.maxValue = pm1Bench.getMaxHealth();
                player1BenchHealth.value = pm1Bench.getHealth();
                player1BenchStamina.value = pm1Bench.getStamina();
            }

            player1BenchHeadshot.sprite = GetHeadshot(player1Bench);
            player1BenchHeadshot.enabled = true;
            player1BenchHealth.gameObject.SetActive(true);
            player1BenchStamina.gameObject.SetActive(true);
        }
        else
        {
            player1BenchHeadshot.enabled = false;
            player1BenchHealth.gameObject.SetActive(false);
            player1BenchStamina.gameObject.SetActive(false);
        }

        if (player2Active != null)
        {
            PlayerMovement pm2Active = player2Active.GetComponent<PlayerMovement>();
            if (pm2Active != null)
            {
                player2ActiveHealth.maxValue = pm2Active.getMaxHealth();
                player2ActiveHealth.value = pm2Active.getHealth();
                player2ActiveStamina.value = pm2Active.getStamina();
            }

            player2ActiveHeadshot.sprite = GetHeadshot(player2Active);
        }

        if (player2Bench != null)
        {
            PlayerMovement pm2Bench = player2Bench.GetComponent<PlayerMovement>();
            if (pm2Bench != null)
            {
                player2BenchHealth.maxValue = pm2Bench.getMaxHealth();
                player2BenchHealth.value = pm2Bench.getHealth();
                player2BenchStamina.value = pm2Bench.getStamina();
            }

            player2BenchHeadshot.sprite = GetHeadshot(player2Bench);
            player2BenchHeadshot.enabled = true;
            player2BenchHealth.gameObject.SetActive(true);
            player2BenchStamina.gameObject.SetActive(true);
        }
        else
        {
            player2BenchHeadshot.enabled = false;
            player2BenchHealth.gameObject.SetActive(false);
            player2BenchStamina.gameObject.SetActive(false);
        }
    }

    public void ShowGameOver(string winner)
    {
        if (gameOverText != null)
        {
            gameOverText.text = $"{winner} wins!";
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    private Sprite GetHeadshot(GameObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        string n = obj.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].characterName == n)
            {
                return characters[i].headshot;
            }
        }

        return null;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
}