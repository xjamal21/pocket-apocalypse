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

        string format = use24HourFormat
            ? (showSeconds ? "HH:mm:ss" : "HH:mm")
            : (showSeconds ? "hh:mm:ss tt" : "hh:mm tt");

        string currentTime = DateTime.Now.ToString(format);

        if (currentTime != lastDisplayedTime)
        {
            clockText.text = currentTime;
            lastDisplayedTime = currentTime;
        }
    }
}