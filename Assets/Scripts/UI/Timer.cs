using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float currentTime;
    public bool countDown;

    [Header("Format")]
    public bool hasFormat;
    public TimerFormats format;
    private Dictionary<TimerFormats, string> timeFormats = new Dictionary<TimerFormats, string>();

    // Start is called before the first frame update
    void Start()
    {
        timeFormats.Add(TimerFormats.Whole, "0");
        timeFormats.Add(TimerFormats.TenthDecimal, "0.0");
        timeFormats.Add(TimerFormats.HundrethsDecimal, "0.00");
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime; //if countdown is true, count down, else count up
        SetTimerText();

        //On death add this to stop timer and turn it red
        /* if (!alive) {
         * timerText.color = Color.red;
         * enabled = false;
         * }
         */
    }

    private void SetTimerText()
    {
        timerText.text = hasFormat ? currentTime.ToString(timeFormats[format]) : currentTime.ToString();
    }


}

public enum TimerFormats
{
    Whole,
    TenthDecimal,
    HundrethsDecimal
}
