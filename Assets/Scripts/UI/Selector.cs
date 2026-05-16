using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hyper.UI
{
    public class Selector<T> : MonoBehaviour
    {
        public Button leftButton;
        public Button rightButton;
        public TextMeshProUGUI textMeshPro;

        public List<T> options = new();

        public event Action<T> OnSelectionChanged;

        public T CurrentValue => options.Count > 0 ? options[CurrentIndex] : default;
        public int CurrentIndex { get; private set; } = 0;

        private void OnEnable()
        {
            leftButton.onClick.AddListener(LeftButton);
            rightButton.onClick.AddListener(RightButton);
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(LeftButton);
            rightButton.onClick.RemoveListener(RightButton);
        }

        private void Start()
        {
            if (options.Count == 0)
            {
                Debug.LogWarning($"Selector on {gameObject.name} has no options configured", this);
                return;
            }

            UpdateDisplay();
        }

        public void LeftButton()
        {
            if (options.Count == 0) return;

            CurrentIndex = (CurrentIndex - 1 + options.Count) % options.Count;
            UpdateDisplay();
        }

        public void RightButton()
        {
            if (options.Count == 0) return;

            CurrentIndex = (CurrentIndex + 1) % options.Count;
            UpdateDisplay();
        }

        public void SetValue(T value)
        {
            int index = options.IndexOf(value);
            if (index >= 0)
            {
                CurrentIndex = index;
                UpdateDisplay();
            }
        }

        public void SetIndex(int index)
        {
            if (index >= 0 && index < options.Count)
            {
                CurrentIndex = index;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            textMeshPro.text = CurrentValue?.ToString() ?? "";
            OnSelectionChanged?.Invoke(CurrentValue);
        }
    }
}
