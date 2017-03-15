using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Project
{
    public enum Status
    {
        InProgress,
        Halted,
        OnSale,
        Retired
    }

    public static string[] StatusText = new string[]
    {
        "In Progress",
        "Halted",
        "On Sale",
        "Retired"
    };

    public const int VALUE_PER_QUALITY_LEVEL = 50;
    public const int PROJECT_SELL_MONTHS = 12;

    public int CurrentPayout { get; private set; }
    public string ProjectStatus { get { return StatusText[(int)CurrentStatus]; } }

    public string Name;
    public Status CurrentStatus;
    public SkillList QualityLevels;

    private int totalPayout;
    private int payoutPerMonth;

    public Project(string name)
    {
        Name = name;
        CurrentStatus = Status.Halted;
        QualityLevels = new SkillList();
    }

    public void ApplyWork(SkillList work)
    {
        QualityLevels += work;
    }

    public void CompleteProject()
    {
        totalPayout = VALUE_PER_QUALITY_LEVEL * QualityLevels.Sum();

        payoutPerMonth = Mathf.FloorToInt((float)totalPayout / PROJECT_SELL_MONTHS);

        TimeManager.PerMonthEvent.AddListener(Payout);
        CurrentStatus = Status.OnSale;

        InformationPanelManager.Instance.DisplayMessage(
            string.Format("{0} is now on sale!", Name), 2.0f);
    }

    public void Payout()
    {
        Company.MyCompany.Funds += payoutPerMonth;
        CurrentPayout += payoutPerMonth;
        if (CurrentPayout >= totalPayout)
        {
            TimeManager.PerMonthEvent.RemoveListener(Payout);
            CurrentStatus = Status.Retired;

            InformationPanelManager.Instance.DisplayMessage(
                string.Format("{0} is no longer on sale!", Name), 2.0f);
        }
    }
}
