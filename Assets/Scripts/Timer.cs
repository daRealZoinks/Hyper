using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshPro timerText;

    private float time = 0f;
    private bool isTimerRunning = false;

    private void OnCollisionEnter(Collision collision)
    {
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            time += Time.deltaTime;
            UpdateTimerText(time);
        }
    }

    private void UpdateTimerText(float time)
    {
        var minutes = Mathf.FloorToInt(time / 60);
        var seconds = Mathf.FloorToInt(time % 60);
        var milliseconds = Mathf.FloorToInt(time * 1000 % 1000);

        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
