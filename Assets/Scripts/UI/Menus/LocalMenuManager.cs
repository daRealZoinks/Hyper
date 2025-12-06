using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LocalMenuManager : MonoBehaviour
{
    public List<CharacterSelectMenu> characterSelectMenuList;

    public PlayerInputManager playerInputManager;
    public EventSystem eventSystem;

    private void OnEnable()
    {
        eventSystem.enabled = false;
        playerInputManager.EnableJoining();
    }

    private void OnDisable()
    {
        eventSystem.enabled = true;
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
