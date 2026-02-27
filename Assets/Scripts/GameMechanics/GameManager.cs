using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string[] player1Team = new string[2];
    public string[] player2Team = new string[2];

    private void Awake() //Hämta refernser osv
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // remove duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swapPlayer1()
    {
        string temp = player1Team[0];
        player1Team[0] = player1Team[1];
        player1Team[1] = temp;
    }

    public void swapPlayer2()
    {
        string temp = player2Team[0];
        player2Team[0] = player2Team[1];
        player2Team[1] = temp;
    }

    public void ClearTeams()
    {
        player1Team[0] = null;
        player1Team[1] = null;
        player2Team[0] = null;
        player2Team[1] = null;
    }

}
