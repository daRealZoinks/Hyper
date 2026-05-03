using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public TextMeshProUGUI textMeshPro;

    public List<string> options = new() { "Option A", "Option B", "Option C" };
    private int currentIndex = 0;

    public void LeftButton()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = options.Count - 1;
        }

        textMeshPro.text = options[currentIndex];
    }

    public void RightButton()
    {
        currentIndex++;

        if (currentIndex >= options.Count)
        {
            currentIndex = 0;
        }

        textMeshPro.text = options[currentIndex];
    }

    private void Start()
    {
        textMeshPro.text = options[currentIndex];
    }
}