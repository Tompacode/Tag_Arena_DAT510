using UnityEngine;

public class PickMenu : MonoBehaviour
{

    public int characterID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Viking1Clicked()
    {
        Debug.Log("Viking 1 selected");  // Egen för varje character button 
    }

    public void onButtonClickCharacterButton()
    {
        Debug.Log("Character button clicked: " +  characterID);

    }




}
