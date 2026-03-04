using System.Collections.Generic;
using UnityEngine;

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

    private int player1Remaining;
    private int player2Remaining;

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

    private void Start()
    {
        player1Remaining = player1TeamList != null ? player1TeamList.Count : 0;
        player2Remaining = player2TeamList != null ? player2TeamList.Count : 0;
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
        string winner = loserTag == "Player1" ? "Player2" : "Player1";
        Debug.Log($"Game Over. {winner} wins. {loserTag} has no characters left.");
        Time.timeScale = 0f;
    }
}
