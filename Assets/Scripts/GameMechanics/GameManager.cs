using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;
        public GameObject prefab;
    }

    public Character[] characters;

    public static GameManager Instance;

    public List<Character> player1TeamList;
    public List<Character> player2TeamList;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorInAllScenes = true;
    [SerializeField] private bool hideCursorWhenLocked = true;
    [SerializeField] private bool relockOnMouseClick = true;

    private int player1Remaining;
    private int player2Remaining;

    private int player1RoundsWon;
    private int player2RoundsWon;

    private string winner = "Player1";
    private bool roundEnded;
    private bool cursorLocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        player1Remaining = player1TeamList != null ? player1TeamList.Count : 0;
        player2Remaining = player2TeamList != null ? player2TeamList.Count : 0;

        ApplyCursorPolicy();
    }

    private void Update()
    {
        if (cursorLocked && Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLock(false);
        }

        if (!cursorLocked && relockOnMouseClick && lockCursorInAllScenes)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                SetCursorLock(true);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        roundEnded = false;
        ApplyCursorPolicy();
    }

    private void ApplyCursorPolicy()
    {
        SetCursorLock(lockCursorInAllScenes);
    }

    private void SetCursorLock(bool shouldLock)
    {
        cursorLocked = shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = shouldLock ? !hideCursorWhenLocked : true;
    }

    public void ClearTeams()
    {
        player1TeamList.Clear();
        player2TeamList.Clear();
        player1Remaining = 0;
        player2Remaining = 0;
    }

    public void AddToTeam(string characterName, int player)
    {
        if (player == 1)
        {
            for (int j = 0; j < characters.Length; j++)
            {
                if (characters[j].characterName == characterName)
                {
                    player1TeamList.Add(characters[j]);
                }
            }

            player1Remaining = player1TeamList.Count;
        }
        else if (player == 2)
        {
            for (int j = 0; j < characters.Length; j++)
            {
                if (characters[j].characterName == characterName)
                {
                    player2TeamList.Add(characters[j]);
                }
            }

            player2Remaining = player2TeamList.Count;
        }
    }

    public void OnCharacterDied(string playerTag)
    {
        if (playerTag == "Player1")
        {
            player1Remaining = Mathf.Max(0, player1Remaining - 1);
            if (player1Remaining <= 0)
            {
                EndGame("Player1");
            }
        }
        else if (playerTag == "Player2")
        {
            player2Remaining = Mathf.Max(0, player2Remaining - 1);
            if (player2Remaining <= 0)
            {
                EndGame("Player2");
            }
        }
    }

    public void EndGame(string loserTag)
    {
        if (roundEnded)
        {
            return;
        }

        roundEnded = true;
        winner = loserTag == "Player1" ? "Player2" : "Player1";

        if (winner == "Player1")
        {
            player1RoundsWon++;
        }
        else
        {
            player2RoundsWon++;
        }

        Debug.Log($"Game Over. {winner} wins. Round score: {GetRoundScoreText()}");
        Time.timeScale = 0f;
    }

    public string GetWinner()
    {
        return winner;
    }

    public string GetRoundScoreText()
    {
        return $"{player1RoundsWon}—{player2RoundsWon}";
    }

    public void ResetRoundScore()
    {
        player1RoundsWon = 0;
        player2RoundsWon = 0;
    }
}
