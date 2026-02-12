using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; 

public class PickMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI menuPanelTextField;

    const int MaxPerTeam = 2;

    enum Picker
    {
        Player1,
        Player2
    }

    int pickStep = 0;

    public Transform Player1Pick1Pos;
    public Transform Player1Pick2Pos;
    public Transform Player2Pick1Pos;
    public Transform Player2Pick2Pos;

    Transform[] PickedCharacterPos;

    public Button[] characterButtons;

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
    void Start()
    {
        ResetSelection();

        PickedCharacterPos = new Transform[]
        {
            Player1Pick1Pos,
            Player2Pick1Pos,
            Player2Pick2Pos,
            Player1Pick2Pos
        };

        UpdatePickerText();
    }

    // Update is called once per frame
    // void Update()
    // {}

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

        foreach (var button in characterButtons)
        {
            if (button != null)
                button.interactable = true;
        }

        UpdatePickerText();

        Debug.Log("Character select reset");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Borde fortfarande funka för snabba scener?
        // Alt. SceneManager.LoadSceneAsync("GameScene"); för load in background 

        Debug.Log("Back to main menu");

    }

    public void finishedSelectionPlay()
    {
        // Alt. SceneManager.LoadSceneAsync("PlayScene");
        SceneManager.LoadScene("PlayScene");
        Debug.Log("To PlayScene"); 
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
            Debug.Log("Player 1 selected: " + characterName);
        }
        else
        {
            if (player2Team.Count >= MaxPerTeam)
            {
                return;
            }

            player2Team.Add(characterName);
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

        HandlePick();
    }

    [SerializeField] Button Viking1Button;
    [SerializeField] Button Viking2Button;
    [SerializeField] Button GladiatorButton;
    [SerializeField] Button SamuraiButton;

    public void VikingMaleClicked()
    {
        Debug.Log("Viking 1 selected");

        registerPick("Viking1", Viking1Button);
    }

    public void VikingFemaleClicked()
    {
        Debug.Log("Viking 2 selected");

        registerPick("Viking2", Viking2Button);
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
