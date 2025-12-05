using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectScreen : MonoBehaviour
{
    public Image characterProfilePicture;

    public TextMeshProUGUI characterName;

    public List<HyperCharacterInfo> hyperCharacterInfo;

    private int currentCharacterIndex = 0;

    private void Start()
    {
        UpdateCharacterDisplay();
    }

    public void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % hyperCharacterInfo.Count;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + hyperCharacterInfo.Count) % hyperCharacterInfo.Count;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        var characterInfo = hyperCharacterInfo[currentCharacterIndex];
        characterProfilePicture.sprite = characterInfo.characterProfilePicture;
        characterName.text = characterInfo.characterName;
    }
}
