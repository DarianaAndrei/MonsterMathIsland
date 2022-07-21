using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeInSecs = 10;
    public TMP_Text timerText;
    public UnityEvent onTimeLapsed;
    public bool startOnAwake = true;
    public bool isTimerOn;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        if (startOnAwake)
        {
            StartTimer();
        }
        timer = timeInSecs;
    }

    public void StartTimer()
    {
        isTimerOn = true;
        timer = timeInSecs;
    }

    public void PauseTimer()
    {
        isTimerOn = false;
    }

    public void StopTimer()
    {
        isTimerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerOn)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0.00");

            if (timer <= 0)
            {
                onTimeLapsed.Invoke();
                timer = timeInSecs;
            }
        }
    }
}
