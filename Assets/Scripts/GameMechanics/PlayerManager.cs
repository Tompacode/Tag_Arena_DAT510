using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private UIManager uiManager;
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

        if (uiManager == null)
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();
        }
    }

    private void Start()
    {
        Debug.Log($"P1: main [{DescribeCharacter(gm.player1TeamList, 0)}], bench [{DescribeCharacter(gm.player1TeamList, 1)}]");
        Debug.Log($"P2: main [{DescribeCharacter(gm.player2TeamList, 0)}], bench [{DescribeCharacter(gm.player2TeamList, 1)}]");

        activePlayer1 = SpawnCharacter(GetCharacterPrefab(gm.player1TeamList, 0), spawnPointPlayer1, "Player1");
        inactivePlayer1 = SpawnCharacter(GetCharacterPrefab(gm.player1TeamList, 1), benchPointPlayer1, "Player1");

        activePlayer2 = SpawnCharacter(GetCharacterPrefab(gm.player2TeamList, 0), spawnPointPlayer2, "Player2");
        inactivePlayer2 = SpawnCharacter(GetCharacterPrefab(gm.player2TeamList, 1), benchPointPlayer2, "Player2");

        SetPlayerActive(activePlayer1, true);
        SetPlayerActive(inactivePlayer1, false);
        SetPlayerActive(activePlayer2, true);
        SetPlayerActive(inactivePlayer2, false);
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

        SetPlayerActive(activePlayer1, false);
        SetPlayerActive(inactivePlayer1, true);

        SwapTransforms(activePlayer1, inactivePlayer1);
        (activePlayer1, inactivePlayer1) = (inactivePlayer1, activePlayer1);
    }

    private void SwapPlayer2()
    {
        if (gm.player2TeamList == null || gm.player2TeamList.Count < 2)
        {
            Debug.LogWarning("SwapPlayer2 skipped: player2TeamList needs at least two characters.");
            return;
        }

        SetPlayerActive(activePlayer2, false);
        SetPlayerActive(inactivePlayer2, true);

        SwapTransforms(activePlayer2, inactivePlayer2);
        (activePlayer2, inactivePlayer2) = (inactivePlayer2, activePlayer2);
    }

    private static void SwapTransforms(GameObject first, GameObject second) 
    {
        if (first == null || second == null)
        {
            return;
        }

        Vector3 firstPos = first.transform.position;
        Quaternion firstRot = first.transform.rotation;

        first.transform.SetPositionAndRotation(second.transform.position, second.transform.rotation);
        second.transform.SetPositionAndRotation(firstPos, firstRot);
    }

    public void OnPlayerDeath(PlayerMovement deadPlayer)
    {
        if (deadPlayer == null)
        {
            return;
        }

        string tag = deadPlayer.tag;
        bool didReplaceActive = false;

        if (tag == "Player1")
        {
            if (activePlayer1 == deadPlayer.gameObject)
            {
                if (inactivePlayer1 != null)
                {
                    SetPlayerActive(activePlayer1, false);

                    activePlayer1 = inactivePlayer1;
                    inactivePlayer1 = null;

                    SetPlayerActive(activePlayer1, true);
                    didReplaceActive = true;
                }
                else
                {
                    gm?.EndGame("Player1");
                    uiManager?.ShowGameOver(gm?.GetWinner());
                }
            }
            else if (inactivePlayer1 == deadPlayer.gameObject)
            {
                SetPlayerActive(inactivePlayer1, false);
                inactivePlayer1 = null;
            }
        }
        else if (tag == "Player2")
        {   
            if (activePlayer2 == deadPlayer.gameObject)
            {
                if (inactivePlayer2 != null)
                {
                    SetPlayerActive(activePlayer2, false);

                    activePlayer2 = inactivePlayer2;
                    inactivePlayer2 = null;

                    SetPlayerActive(activePlayer2, true);
                    didReplaceActive = true;
                }
                else
                {
                    gm?.EndGame("Player2");
                    uiManager?.ShowGameOver(gm?.GetWinner());
                }
            }
            else if (inactivePlayer2 == deadPlayer.gameObject)
            {
                SetPlayerActive(inactivePlayer2, false);
                inactivePlayer2 = null;
            }
        }

        deadPlayer.gameObject.SetActive(false);

        if (didReplaceActive)
        {
            MoveTo(activePlayer1, spawnPointPlayer1);
            MoveTo(activePlayer2, spawnPointPlayer2);
        }
    }

    private static Vector3 GetReplacementPosition(Vector3 currentPos, Transform fallbackPoint)
    {
        // Keep current X/Z so replacement appears where fight happened,
        // but force Y to a stable ground-level fallback to avoid spawning in air.
        float groundedY = fallbackPoint != null ? fallbackPoint.position.y : currentPos.y;
        return new Vector3(currentPos.x, groundedY, currentPos.z);
    }

    private void SetPlayerActive(GameObject player, bool isActive)
    {
        if (player == null)
        {
            return;
        }

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.enabled = isActive;
        }

        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = isActive;
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