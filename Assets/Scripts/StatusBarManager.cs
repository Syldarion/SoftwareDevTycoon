using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusBarManager : Singleton<StatusBarManager>
{
    public Text CurrentFundsText;
    public Text CurrentDateText;
    public Text CompanyNameText;
    public Text CompanyOfficeCountText;
    public Text CompanyEmployeeCountText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TimeManager.PerDayEvent.AddListener(UpdateFunds);
        TimeManager.PerDayEvent.AddListener(UpdateTime);
        TimeManager.PerDayEvent.AddListener(UpdateCompanyInfo);
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

    public void UpdateCompanyInfo()
    {
        if (Company.MyCompany == null)
            return;

        CompanyNameText.text = Company.MyCompany.Name;
        CompanyOfficeCountText.text = Company.MyCompany.CompanyOffices.Count.ToString();
        CompanyEmployeeCountText.text = Company.MyCompany.TeamSize.ToString();
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