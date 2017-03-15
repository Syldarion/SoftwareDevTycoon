using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusBarManager : Singleton<StatusBarManager>
{
    public Text CurrentFundsText;
    public Text CurrentDateText;

    public Color PositiveFundsColor;
    public Color NegativeFundsColor;
    public Color PausedTimeColor;
    public Color UnpausedTimeColor;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupEvents();
    }

    void Update()
    {

    }

    public void SetupEvents()
    {
        TimeManager.PerDayEvent.AddListener(UpdateTime);
        TimeManager.OnTogglePauseEvent.AddListener(UpdateTimeColor);
    }

    public void UpdateFunds(int value)
    {
        CurrentFundsText.text = value.ToString("C0");
        CurrentFundsText.color = value < 0 ? NegativeFundsColor : PositiveFundsColor;
    }

    public void UpdateTime()
    {
        CurrentDateText.text = TimeManager.CurrentDate.ToString("MMM dd, yyyy");
    }

    public void UpdateTimeColor(bool paused)
    {
        CurrentDateText.color = paused ? PausedTimeColor : UnpausedTimeColor;
    }
}
