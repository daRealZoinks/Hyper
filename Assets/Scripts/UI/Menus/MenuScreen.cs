using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class MenuScreen : MonoBehaviour
    {
        private List<Selectable> selectables;

        public Selectable currentSelected;

        private void Awake()
        {
            selectables = GetComponentsInChildren<Selectable>().ToList();

            EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
        }

        private void OnEnable()
        {
            DeviceManager.Singleton.OnDeviceTypeChanged += OnDeviceTypeChanged;

            EventSystem.current.SetSelectedGameObject((currentSelected ? currentSelected : selectables[0]).gameObject);
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
                    EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
                }
            }
        }
    }
}