using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class TimerScript : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI tmText;

    [Header("Timer Settings")]
    public float curTime;
    public bool countDown;
    public float startTime;
    public bool isPaused;

    private string minutes, seconds;

    void Start()
    {
        tmText = GetComponent<TextMeshProUGUI>();
        curTime = startTime;
    }

    void Update()
    {
        curTime = (countDown && !isPaused) ? (curTime - Time.deltaTime) : (curTime + Time.deltaTime);
        minutes = ((int)curTime / 60).ToString("00");
        seconds = ((int)curTime % 60).ToString("00");
        tmText.text = minutes + ":" + seconds;
    }

    bool PauseTimer()
    {
        if (isPaused)
        {
            return false;
        }

        isPaused = true;
        return true;
    }

    bool ResumeTimer()
    {
        if (!isPaused)
        {
            return false;
        }

        isPaused = false;
        return true;
    }

    void SetTimer(float newTime)
    {
        this.curTime = newTime;
    }

    void ResetTimer()
    {
        this.curTime = startTime;
    }
}
