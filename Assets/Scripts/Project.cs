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

    public const int VALUE_PER_QUALITY_LEVEL = 100;
    public const int PROJECT_SELL_MONTHS = 24;

    public int CurrentPayout { get; private set; }

    public string Name;
    public Status CurrentStatus;
    
    public SkillList QualityLevels;

    private int totalPayout;
    private int payoutPerMonth;

    public Project(string name)
    {
        if (name == string.Empty) name = "New Project";
        if (name.Length > 20) name = name.Substring(0, 20);
        Name = name;
    }

    public void ApplyWork(SkillList work)
    {
        QualityLevels += work;
    }

    public void CompleteProject()
    {
        totalPayout = Mathf.Abs(VALUE_PER_QUALITY_LEVEL * QualityLevels.Sum());

        payoutPerMonth = Mathf.FloorToInt((float)totalPayout / PROJECT_SELL_MONTHS);

        TimeManager.PerMonthEvent.AddListener(Payout);
    }

    public void Payout()
    {
        Company.MyCompany.AdjustFunds(payoutPerMonth);
        CurrentPayout += payoutPerMonth;
        if(CurrentPayout >= totalPayout)
            TimeManager.PerMonthEvent.RemoveListener(Payout);
    }

    public float[] GetQualityPercentages()
    {
        int total = QualityLevels.Sum();
        float[] results = new float[QualityLevels.Length];
        for(int i = 0; i < QualityLevels.Length; i++)
            results[i] = (float)QualityLevels[i].Level / total;
        return results;
    }
}
