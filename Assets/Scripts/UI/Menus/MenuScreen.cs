using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class MenuScreen : MonoBehaviour
    {
        public bool showButtonPrompts;

        [HideInInspector] public Selectable currentSelected;

        private List<Selectable> _selectables;

        private void Awake()
        {
            _selectables = GetComponentsInChildren<Selectable>().ToList();

            if (_selectables.Count != 0)
            {
                EventSystem.current.SetSelectedGameObject(_selectables[0].gameObject);
            }
        }

        private void OnEnable()
        {
            DeviceManager.Singleton.OnDeviceTypeChanged += OnDeviceTypeChanged;

            if (_selectables.Count != 0)
            {
                EventSystem.current.SetSelectedGameObject((currentSelected ? currentSelected : _selectables[0]).gameObject);
            }
        }

        private void OnDisable()
        {
            DeviceManager.Singleton.OnDeviceTypeChanged -= OnDeviceTypeChanged;

            if (EventSystem.current)
            {
                var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

                if (currentSelectedGameObject)
                {
                    currentSelected = currentSelectedGameObject.GetComponent<Selectable>();
                }
            }
        }

        private void OnDeviceTypeChanged(DeviceManager.DeviceType deviceType)
        {
            if (deviceType == DeviceManager.DeviceType.Mouse || deviceType == DeviceManager.DeviceType.Keyboard)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                if (currentSelected)
                {
                    EventSystem.current.SetSelectedGameObject(currentSelected.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(_selectables[0].gameObject);
                }
            }
        }
    }
}