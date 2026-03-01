using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    [System.Serializable]
    public class Character
    {
        public string characterName;      
        public GameObject prefab;
    }

    [SerializeField] private swapUI ui;

    public Character[] characters;

    public Transform spawnPointPlayer1;
    public Transform spawnPointPlayer2;

    public Transform benchPointPlayer1;
    public Transform benchPointPlayer2;

    GameObject spawnedPlayer1;
    GameObject player1Bench;

    GameObject spawnedPlayer2;
    GameObject player2Bench;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager gm = GameManager.Instance;

        Debug.Log($"P1: [{gm.player1Team[0]}], bench [{gm.player1Team[1]}]");
        Debug.Log($"P2: [{gm.player2Team[0]}], bench [{gm.player2Team[1]}]");

        spawnedPlayer1 = SpawnCharacter(gm.player1Team[0], spawnPointPlayer1);
        player1Bench = SpawnCharacter(gm.player1Team[1], benchPointPlayer1);


        spawnedPlayer2 = SpawnCharacter(gm.player2Team[0], spawnPointPlayer2);
        player2Bench = SpawnCharacter(gm.player2Team[1], benchPointPlayer2);

        ui.BindPlayer1(spawnedPlayer1, player1Bench);
        ui.BindPlayer2(spawnedPlayer2, player2Bench);

    }

    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwapPlayer1();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapPlayer2();
    }

    void SwapPlayer1()
    {
        (spawnedPlayer1, player1Bench) = (player1Bench, spawnedPlayer1);

        MoveTo(spawnedPlayer1, spawnPointPlayer1);
        MoveTo(player1Bench, benchPointPlayer1);

        GameManager.Instance.swapPlayer1();

        ui.BindPlayer1(spawnedPlayer1, player1Bench);
    }

    void SwapPlayer2()
    {
        (spawnedPlayer2, player2Bench) = (player2Bench, spawnedPlayer2);

        MoveTo(spawnedPlayer2, spawnPointPlayer2);
        MoveTo(player2Bench, benchPointPlayer2);

        GameManager.Instance.swapPlayer2();

        ui.BindPlayer2(spawnedPlayer2, player2Bench);
    }

    void MoveTo(GameObject obj, Transform point)
    {
        if (obj == null || point == null) return;
        obj.transform.position = point.position; 
        obj.transform.rotation = point.rotation;
    }

    GameObject SpawnCharacter(string characterName, Transform spawnPoint)
    {
        foreach (Character c in characters)
        {
            if (c.characterName == characterName)
            {
                return Instantiate(c.prefab, spawnPoint.position, spawnPoint.rotation);
            }
        }

        Debug.LogError("No prefab found for: " + characterName);
        return null;
    }

}