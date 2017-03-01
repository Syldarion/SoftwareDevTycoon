using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Project
{
    public const int VALUE_PER_QUALITY_LEVEL = 100;
    public const int PROJECT_SELL_MONTHS = 24;

    public string Name;
    
    public SkillList QualityLevels;

    private int totalPayout;
    private int payoutPerMonth;
    private int currentTotalPayout;

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
        currentTotalPayout += payoutPerMonth;
        if(currentTotalPayout >= totalPayout)
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