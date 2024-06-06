using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CharacterLocation selectedCharacterLocation;
    [SerializeField]
    private TMP_Dropdown Character_dropdown,Location_dropdown;

    void Start (){
        SetDropdownToCurrentCharacterLocation();
    }

    public void OnLocationDropdownValueChanged(int index)
    {
        string selectedValue = Location_dropdown.options[index].text;
        selectedCharacterLocation.location = GetLocationType(selectedValue);
    }

    public void OnCharacterDropdownValueChanged(int index)
    {
        string selectedValue = Character_dropdown.options[index].text;
        selectedCharacterLocation.character = GetCharacterType(selectedValue);
    }

    public void LoadGameScene(){
        SceneManager.LoadScene("Game Scene");
    }
    public void QuitGame(){
        Application.Quit();
    }

    private void SetDropdownToCurrentCharacterLocation()
    {
        // Assuming that the dropdown options are correctly set up as per the enum
        Character_dropdown.value = (int)selectedCharacterLocation.character;
        Character_dropdown.RefreshShownValue();

        Location_dropdown.value = (int)selectedCharacterLocation.location;
        Location_dropdown.RefreshShownValue();
    }

    private CharacterType GetCharacterType(string characterName)
    {
        switch (characterName)
        {
            case "Elijah Thorne, Inventor":
                return CharacterType.Elijah_Thorne_Inventor;
            case "Inspector Rupert Blackwell":
                return CharacterType.Inspector_Rupert_Blackwell;
            case "Lady Constance Fairchild, Spiritualist":
                return CharacterType.Lady_Constance_Fairchild_Spiritualist;
            default:
                return CharacterType.Elijah_Thorne_Inventor;
        }
    }

    private LocationType GetLocationType(string locationName)
    {
        switch (locationName)
        {
            case "Victorian London Street Market":
                return LocationType.Victorian_London_Street_Market;
            case "Victorian Steam-punk Workshop":
                return LocationType.Victorian_Steampunk_Workshop;
            case "Victorian Psychic Medium Caravan":
                return LocationType.Victorian_Psychic_Medium_Caravan;
            default:
                return LocationType.Victorian_London_Street_Market;
        }
    }

}
