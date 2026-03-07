using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        PlayButton.interactable = pickStep >= pickOrder.Length;
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
    public void ResetSelection()
    {
        pickStep = 0;
        currentPicker = Picker.Player1;

        player1Team.Clear();
        player2Team.Clear();

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
        SelectNextAvailableButton();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 

    }

    public void finishedSelectionPlay()
    {
        SceneManager.LoadScene("Game");
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
                break;
            }

        }
    }

    public void HandlePick()
    {
        // Last pick completes draft
        if (pickStep >= pickOrder.Length - 1)
        {
            pickStep = pickOrder.Length;

            if (menuPanelTextField != null)
            {
                menuPanelTextField.text = "Selection complete";
            }

            if (PlayButton != null)
            {
                PlayButton.interactable = true;
            }

            SelectNextAvailableButton();
            return;
        }

        pickStep++;
        currentPicker = pickOrder[pickStep];
        UpdatePickerText();
    }

    private void registerPick(string characterName, Button clickedButton)
    {
        if (pickStep >= pickOrder.Length){
            return;
        }

        if (currentPicker == Picker.Player1)
        {
            if (player1Team.Count >= MaxPerTeam)
            {
                return;
            }

            player1Team.Add(characterName);
            gm.AddToTeam(characterName, 1);
        }
        else
        {
            if (player2Team.Count >= MaxPerTeam)
            {
                return;
            }

            player2Team.Add(characterName);
            gm.AddToTeam(characterName, 2);
        }

        if (clickedButton != null)
        {
            clickedButton.interactable = false;
        }

        enable3DModelWhenPicked(characterName);
        HandlePick();
        SelectNextAvailableButton();
    }

    private void SelectNextAvailableButton()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        // Draft complete -> force-select Play button
        if (pickStep >= pickOrder.Length)
        {
            if (PlayButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(PlayButton.gameObject);
            }

            return;
        }

        foreach (var button in characterButtons)
        {
            if (button != null && button.interactable)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                return;
            }
        }
    }

    [SerializeField] Button KnightButton;
    [SerializeField] Button VikingFemaleButton;
    [SerializeField] Button GladiatorButton;
    [SerializeField] Button SamuraiButton;

    public void KnightClicked()
    {
        registerPick("Knight", KnightButton);
    }

    public void VikingFemaleClicked()
    {
        registerPick("VikingFemale", VikingFemaleButton);
    }

    public void GladiatorClicked()
    { 
        registerPick("Gladiator", GladiatorButton);
    }

    public void SamuraiClicked()
    {
        registerPick("Samurai", SamuraiButton);
    }
}
