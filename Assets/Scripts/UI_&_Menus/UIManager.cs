using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    PlayerManager playerManager;
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

    private void Awake()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        GameObject player1Active = playerManager.GetActivePlayer1();
        GameObject player1Bench = playerManager.GetInactivePlayer1();
        GameObject player2Active = playerManager.GetActivePlayer2();
        GameObject player2Bench = playerManager.GetInactivePlayer2();

        player1ActiveHealth.value = player1Active.GetComponent<PlayerMovement>().getHealth();
        player1BenchHealth.value = player1Bench.GetComponent<PlayerMovement>().getHealth();
        player2ActiveHealth.value = player2Active.GetComponent<PlayerMovement>().getHealth();
        player2BenchHealth.value = player2Bench.GetComponent<PlayerMovement>().getHealth();

        player1ActiveStamina.value = player1Active.GetComponent<PlayerMovement>().getStamina();
        player1BenchStamina.value = player1Bench.GetComponent<PlayerMovement>().getStamina();
        player2ActiveStamina.value = player2Active.GetComponent<PlayerMovement>().getStamina();
        player2BenchStamina.value = player2Bench.GetComponent<PlayerMovement>().getStamina();

        player2BenchHeadshot.sprite = GetHeadshot(player2Bench);
        player1BenchHeadshot.sprite = GetHeadshot(player1Bench);
        player2ActiveHeadshot.sprite = GetHeadshot(player2Active);
        player1ActiveHeadshot.sprite = GetHeadshot(player1Active);

    }

    Sprite GetHeadshot(GameObject obj)
    {
        if (obj == null) return null;
        Debug.Log($"Getting headshot for {obj.name}");
        string n = obj.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < characters.Length; i++)
            if (characters[i].characterName == n)
                return characters[i].headshot;

        return null;
    }
}
