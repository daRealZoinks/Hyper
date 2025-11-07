using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public Button firstButtonToSelect;

    public Button lastButtonSelected;

    public InputSystemUIInputModule _inputSystemUIInputModule;

    private void Awake()
    {
        var eventSystem = EventSystem.current;

        _inputSystemUIInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();

        _inputSystemUIInputModule.move.action.performed += (_) =>
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                (lastButtonSelected ? lastButtonSelected : firstButtonToSelect).Select();
            }
        };
    }

    private void OnEnable()
    {
        (lastButtonSelected ? lastButtonSelected : firstButtonToSelect).Select();
    }

    private void OnDisable()
    {
        var eventSystem = EventSystem.current;

        if (eventSystem.currentSelectedGameObject)
        {
            lastButtonSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        }
    }
}
