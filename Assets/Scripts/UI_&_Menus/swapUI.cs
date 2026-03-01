using UnityEngine;
using UnityEngine.UI;

public class swapUI : MonoBehaviour
{

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

    GameObject player1Active, player1Bench, player2Active, player2Bench;

    public void BindPlayer1(GameObject active, GameObject bench)
    {
        player1Active = active;
        player1Bench = bench;

        player1ActiveHeadshot.sprite = GetHeadshot(active);
        player1BenchHeadshot.sprite = GetHeadshot(bench);

        Debug.Log("BindPlayer1 called: " + active.name + " / " + bench.name);
    }

    public void BindPlayer2(GameObject active, GameObject bench)
    {
        player2Active = active;
        player2Bench = bench;

        player2ActiveHeadshot.sprite = GetHeadshot(active);
        player2BenchHeadshot.sprite = GetHeadshot(bench);

        Debug.Log("BindPlayer2 called: " + active.name + " / " + bench.name);
    }

    Sprite GetHeadshot(GameObject obj)
    {
        if (obj == null) return null;

        string n = obj.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < characters.Length; i++)
            if (characters[i].characterName == n)
                return characters[i].headshot;

        return null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player1Active != null)
            player1ActiveHealth.value = player1Active.GetComponent<PlayerMovement>().getHealth();

        if (player1Bench != null)
            player1BenchHealth.value = player1Bench.GetComponent<PlayerMovement>().getHealth();

        if (player2Active != null)
            player2ActiveHealth.value = player2Active.GetComponent<PlayerMovement>().getHealth();

        if (player2Bench != null)
            player2BenchHealth.value = player2Bench.GetComponent<PlayerMovement>().getHealth();
    }

}
