using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class PickMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI menuPanelTextField;

    const int MaxPerTeam = 2;
    public GameManager gm;
    enum Picker
    {
        Player1,
        Player2
    }

    // Counters 
    int pickStep = 0;
    int player1TeamIndex = 0;
    int player2TeamIndex = 0;

    public Transform Player1Pick1Pos;
    public Transform Player1Pick2Pos;
    public Transform Player2Pick1Pos;
    public Transform Player2Pick2Pos;

    Transform[] PickedCharacterPos;

    public Button[] characterButtons;

    public Button PlayButton;

    Picker[] pickOrder =
    {
    Picker.Player1,
    Picker.Player2,
    Picker.Player2,
    Picker.Player1
    };

    Picker currentPicker = Picker.Player1;

    List<string> player1Team = new List<string>();

    List<string> player2Team = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

        gm = GameManager.Instance;

        if (gm == null)
            Debug.LogError("No GameManager.Instance found.");

        //gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Start()
    {
        PlayButton.enabled = false;
        PickedCharacterPos = new Transform[]
        {
            Player1Pick1Pos,
            Player2Pick1Pos,
            Player2Pick2Pos,
            Player1Pick2Pos
        };

        ResetSelection();
        UpdatePickerText();
    }

    // Update is called once per frame
    // void Update()
    // {}

    private void Update()
    {
        if(pickStep == 3)
        {
           PlayButton.enabled = true;
        }
    }

    void UpdatePickerText()
    {
        if (menuPanelTextField != null) // Undviker NullReferenceException
        {
            if (currentPicker == Picker.Player1)
            {
                menuPanelTextField.text = "Player 1 is picking";
            }
            else
            {
                menuPanelTextField.text = "Player 2 is picking";
            }
        }
    }
    void ResetSelection()
    {
        pickStep = 0;
        currentPicker = Picker.Player1;

        player1Team.Clear();
        player2Team.Clear();

        player1TeamIndex = 0;
        player2TeamIndex = 0;

        if (gm != null)
            gm.ClearTeams();

        foreach (var button in characterButtons)
        {
            if (button != null)
                button.interactable = true;
        }

        if (PickedCharacterPos != null)
        {
            foreach (var pos in PickedCharacterPos)
            {
                if (pos == null) continue;

                for (int i = 0; i < pos.childCount; i++)
                {
                    pos.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        UpdatePickerText();

        Debug.Log("Character select reset");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
        Debug.Log("Back to main menu");

    }

    public void finishedSelectionPlay()
    {
        SceneManager.LoadScene("Game");
        Debug.Log("To Game Scene"); 
    }

    public void enable3DModelWhenPicked(string characterName)
    {
        if (PickedCharacterPos == null || pickStep >= PickedCharacterPos.Length)
        {
            return;
        }
            
        Transform pickPos = PickedCharacterPos[pickStep];
        if (pickPos == null) {
            return;
        }

        for (int i = 0; i < pickPos.childCount; i++)
        {
            GameObject child = pickPos.GetChild(i).gameObject;
            child.SetActive(false);

            if (child.name.Contains(characterName))
            {
                child.SetActive(true);
                Debug.Log("Enabled model: " + child.name);
                break;
            }

        }
    }

    public void HandlePick()
    {
        // Varannan spelare väljer 
        // if (currentPicker == Picker.Player1)
        //    currentPicker = Picker.Player2;
        // else
        //    currentPicker = Picker.Player1;


        // 1 2 2 1 
        if (pickStep >= pickOrder.Length - 1)
        {
            Debug.Log("Selection complete!");

            if (menuPanelTextField != null)
                menuPanelTextField.text = "Selection complete";

            return;
        }

        pickStep++;
        currentPicker = pickOrder[pickStep];

        UpdatePickerText();
    }

    void registerPick(string characterName, Button clickedButton)
    {
        if (pickStep >= pickOrder.Length){
            Debug.Log("Selection already complete");
            return;
        }

        if (currentPicker == Picker.Player1)
        {
            if (player1Team.Count >= MaxPerTeam)
            {
                return;
            }

            player1Team.Add(characterName);

            if (player1TeamIndex < gm.player1Team.Length)
            {
                gm.player1Team[player1TeamIndex] = characterName;
                player1TeamIndex++;
            }

            Debug.Log("Player 1 selected: " + characterName);
        }
        else
        {
            if (player2Team.Count >= MaxPerTeam)
            {
                return;
            }

            player2Team.Add(characterName);

            if (player2TeamIndex < gm.player2Team.Length)
            {
                gm.player2Team[player2TeamIndex] = characterName;
                player2TeamIndex++;
            }

            Debug.Log("Player 2 selected: " + characterName);
        }

        if (clickedButton != null)
        {
            clickedButton.interactable = false;
        }

        if (pickStep < PickedCharacterPos.Length && PickedCharacterPos[pickStep] != null)
        {
            Transform pos = PickedCharacterPos[pickStep];
            Debug.Log(characterName + " stands at position: " + pos.name);
        }

        enable3DModelWhenPicked(characterName);
        HandlePick();
    }

    [SerializeField] Button KnightButton;
    [SerializeField] Button VikingFemaleButton;
    [SerializeField] Button GladiatorButton;
    [SerializeField] Button SamuraiButton;

    public void KnightClicked()
    {
        Debug.Log("Knight selected");

        registerPick("Knight", KnightButton);
    }

    public void VikingFemaleClicked()
    {
        Debug.Log("Viking female selected");

        registerPick("VikingFemale", VikingFemaleButton);
    }

    public void GladiatorClicked()
    {
        Debug.Log("Gladiator selected");

        registerPick("Gladiator", GladiatorButton);
    }

    public void SamuraiClicked()
    {
        Debug.Log("Samurai selected");

        registerPick("Samurai", SamuraiButton);
    }




}
