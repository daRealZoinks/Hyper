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
            EventSystem.current.SetSelectedGameObject((currentSelected ? currentSelected : selectables[0]).gameObject);
        }

        private void OnDisable()
        {
            if (!EventSystem.current) { return; }

            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

            if (!currentSelectedGameObject) { return; }

            currentSelected = currentSelectedGameObject.GetComponent<Selectable>();
        }
    }
}