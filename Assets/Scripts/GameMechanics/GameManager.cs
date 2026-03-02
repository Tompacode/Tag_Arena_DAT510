using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;
        public GameObject prefab;
        //public Sprite characterIcon; add in future if needed
    }

    public Character[] characters;
   

    public static GameManager Instance;

    public List<Character> player1TeamList;
    public List<Character> player2TeamList;

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


    // Update is called once per frame
    void Update()
    {
        
    }



    public void ClearTeams()
    {
        player1TeamList.Clear();
        player2TeamList.Clear();
    }

    public void AddToTeam(string characterName, int player)
    {
        if (player == 1)
        {
            for(int j = 0; j < characters.Length; j++)
            {
                if (characters[j].characterName == characterName)
                {
                    player1TeamList.Add(characters[j]);
                }
            }
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

        }
    }
}
