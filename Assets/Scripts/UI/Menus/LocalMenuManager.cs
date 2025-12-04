using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMenuManager : MonoBehaviour
{
    public GameObject localMenu;

    public List<CharacterSelectMenu> characterSelectMenuList;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        if (localMenu.activeSelf && !playerInputManager.joiningEnabled)
        {
            playerInputManager.EnableJoining();
        }

        if (!localMenu.activeSelf && playerInputManager.joiningEnabled)
        {
            playerInputManager.DisableJoining();
        }
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
