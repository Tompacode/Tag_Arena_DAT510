using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private UIManager UIManager;
    [SerializeField] private GameManager gm;

    public Transform spawnPointPlayer1;
    public Transform spawnPointPlayer2;

    public Transform benchPointPlayer1;
    public Transform benchPointPlayer2;

    private GameObject activePlayer1;
    private GameObject inactivePlayer1;

    private GameObject activePlayer2;
    private GameObject inactivePlayer2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gm = GameManager.Instance;
    }

    private void Start()
    {
        Debug.Log($"P1: main [{DescribeCharacter(gm.player1TeamList, 0)}], bench [{DescribeCharacter(gm.player1TeamList, 1)}]");
        Debug.Log($"P2: main [{DescribeCharacter(gm.player2TeamList, 0)}], bench [{DescribeCharacter(gm.player2TeamList, 1)}]");

        activePlayer1 = SpawnCharacter(GetCharacterPrefab(gm.player1TeamList, 0), spawnPointPlayer1, "Player1");
        inactivePlayer1 = SpawnCharacter(GetCharacterPrefab(gm.player1TeamList, 1), benchPointPlayer1, "Player1");

        activePlayer2 = SpawnCharacter(GetCharacterPrefab(gm.player2TeamList, 0), spawnPointPlayer2, "Player2");
        inactivePlayer2 = SpawnCharacter(GetCharacterPrefab(gm.player2TeamList, 1), benchPointPlayer2, "Player2");

        UpdateOpponents();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Player1_Swap"))
        {
            SwapPlayer1();
        }

        if (Input.GetButtonDown("Player2_Swap"))
        {
            SwapPlayer2();
        }
    }

    private void SwapPlayer1()
    {
        if (gm.player1TeamList == null || gm.player1TeamList.Count < 2)
        {
            Debug.LogWarning("SwapPlayer1 skipped: player1TeamList needs at least two characters.");
            return;
        }

        (activePlayer1, inactivePlayer1) = (inactivePlayer1, activePlayer1);

        MoveTo(activePlayer1, spawnPointPlayer1);
        MoveTo(inactivePlayer1, benchPointPlayer1);

        UpdateOpponents();
    }

    private void SwapPlayer2()
    {
        if (gm.player2TeamList == null || gm.player2TeamList.Count < 2)
        {
            Debug.LogWarning("SwapPlayer2 skipped: player2TeamList needs at least two characters.");
            return;
        }

        (activePlayer2, inactivePlayer2) = (inactivePlayer2, activePlayer2);

        MoveTo(activePlayer2, spawnPointPlayer2);
        MoveTo(inactivePlayer2, benchPointPlayer2);

        UpdateOpponents();
    }

    public void OnPlayerDeath(PlayerMovement deadPlayer)
    {
        if (deadPlayer == null)
        {
            return;
        }

        string tag = deadPlayer.tag;

        if (tag == "Player1")
        {
            if (activePlayer1 == deadPlayer.gameObject)
            {
                if (inactivePlayer1 != null)
                {
                    Destroy(activePlayer1);
                    activePlayer1 = inactivePlayer1;
                    inactivePlayer1 = null;
                    MoveTo(activePlayer1, spawnPointPlayer1);
                    UpdateOpponents();
                }
                else
                {
                    gm?.EndGame("Player1");
                }
            }
            else if (inactivePlayer1 == deadPlayer.gameObject)
            {
                Destroy(inactivePlayer1);
                inactivePlayer1 = null;
            }
        }
        else if (tag == "Player2")
        {
            if (activePlayer2 == deadPlayer.gameObject)
            {
                if (inactivePlayer2 != null)
                {
                    Destroy(activePlayer2);
                    activePlayer2 = inactivePlayer2;
                    inactivePlayer2 = null;
                    MoveTo(activePlayer2, spawnPointPlayer2);
                    UpdateOpponents();
                }
                else
                {
                    gm?.EndGame("Player2");
                }
            }
            else if (inactivePlayer2 == deadPlayer.gameObject)
            {
                Destroy(inactivePlayer2);
                inactivePlayer2 = null;
            }
        }
    }

    private void MoveTo(GameObject obj, Transform point)
    {
        if (obj == null || point == null)
        {
            return;
        }

        obj.transform.position = point.position;
        obj.transform.rotation = point.rotation;
    }

    private GameObject SpawnCharacter(GameObject prefab, Transform spawnPoint, string tag)
    {
        if (prefab == null || spawnPoint == null)
        {
            return null;
        }

        var instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        instance.tag = tag;
        return instance;
    }

    private void UpdateOpponents()
    {
        var p1 = activePlayer1 != null ? activePlayer1.GetComponent<PlayerMovement>() : null;
        var p2 = activePlayer2 != null ? activePlayer2.GetComponent<PlayerMovement>() : null;

        if (p1 != null)
        {
            p1.SetOpponent(activePlayer2 != null ? activePlayer2.transform : null);
        }

        if (p2 != null)
        {
            p2.SetOpponent(activePlayer1 != null ? activePlayer1.transform : null);
        }
    }

    private static GameObject GetCharacterPrefab(System.Collections.Generic.List<GameManager.Character> team, int index)
    {
        if (team == null || index < 0 || index >= team.Count)
        {
            return null;
        }

        return team[index]?.prefab;
    }

    private static string DescribeCharacter(System.Collections.Generic.List<GameManager.Character> team, int index)
    {
        var character = (team != null && index >= 0 && index < team.Count) ? team[index] : null;
        return character != null ? character.characterName : "null";
    }

    public GameObject GetActivePlayer1() => activePlayer1;
    public GameObject GetInactivePlayer1() => inactivePlayer1;
    public GameObject GetActivePlayer2() => activePlayer2;
    public GameObject GetInactivePlayer2() => inactivePlayer2;
}