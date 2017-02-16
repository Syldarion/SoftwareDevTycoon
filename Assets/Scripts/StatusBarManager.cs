using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusBarManager : Singleton<StatusBarManager>
{
    public Text CurrentFundsText;
    public Text CurrentDateText;

    void Start()
    {
        TimeManager.PerDayEvent.AddListener(UpdateFunds);
        TimeManager.PerDayEvent.AddListener(UpdateTime);
        TimeManager.OnTogglePauseEvent.AddListener(UpdateTimeColor);
    }

    void Update()
    {

    }

    public void UpdateFunds()
    {
        int funds_value = Company.MyCompany == null ? Character.MyCharacter.Money : Company.MyCompany.Funds;
        CurrentFundsText.text = funds_value.ToString("C");
        CurrentFundsText.color = funds_value < 0 ? Color.red : Color.black;
    }

    public void UpdateTime()
    {
        CurrentDateText.text = TimeManager.CurrentDate.ToString("MMM dd, yyyy");
    }

    public void UpdateTimeColor(bool paused)
    {
        CurrentDateText.color = paused ? Color.red : Color.black;
    }
}