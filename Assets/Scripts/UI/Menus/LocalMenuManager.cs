using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMenuManager : MonoBehaviour
{
    public List<CharacterSelectMenu> characterSelectMenuList;

    public PlayerInputManager playerInputManager;

    private void OnEnable()
    {
        playerInputManager.EnableJoining();
    }

    private void OnDisable()
    {
        playerInputManager.DisableJoining();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        var characterSelectMenu = characterSelectMenuList[playerInput.playerIndex];

        playerInput.transform.SetParent(characterSelectMenu.transform);

        var rectTransform = playerInput.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;

        characterSelectMenu.User = playerInput.user;
    }
}
