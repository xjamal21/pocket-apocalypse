using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UserTime : MonoBehaviour
{
    [Header("Format Settings")]
    [SerializeField] private bool use24HourFormat = true;
    [SerializeField] private bool showSeconds = false;

    private TextMeshProUGUI clockText;
    private string lastDisplayedTime = string.Empty;

    private void Awake()
    {
        clockText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (clockText == null) return;

        string format;

        if (use24HourFormat)
        {
            if (showSeconds)
            {
                format = "HH:mm:ss";
            }
            else
            {
                format = "HH:mm";
            }
        }
        else
        {
            if (showSeconds)
            {
                format = "hh:mm:ss tt";
            }
            else
            {
                format = "hh:mm tt";
            }
        }

        string currentTime = DateTime.Now.ToString(format);

        if (currentTime != lastDisplayedTime)
        {
            clockText.text = currentTime;
            lastDisplayedTime = currentTime;
        }
    }
}