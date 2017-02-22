using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Project
{
    public const int VALUE_PER_QUALITY_LEVEL = 100;
    public const int PROJECT_SELL_MONTHS = 24;

    public string Name;

    public int TotalQuality { get { return QualityLevels.Sum(x => x.Level); } }
    public SkillLevel[] QualityLevels = {
        new SkillLevel(Skill.Programming, 0), 
        new SkillLevel(Skill.UserInterfaces, 0), 
        new SkillLevel(Skill.Databases, 0), 
        new SkillLevel(Skill.Networking, 0), 
        new SkillLevel(Skill.WebDevelopment, 0),
    };

    private int totalPayout;
    private int payoutPerMonth;
    private int currentTotalPayout;

    public Project(string name)
    {
        if (name == string.Empty) name = "New Project";
        if (name.Length > 20) name = name.Substring(0, 20);
        Name = name;
    }

    public void ApplyWork(Skill applySkill, int applyAmount)
    {
        if(applyAmount < 0) return;

        SkillLevel apply_to = QualityLevels.FirstOrDefault(x => x.Skill == applySkill);

        apply_to.Level += applyAmount;
    }

    public void CompleteProject()
    {
        totalPayout = Mathf.Abs(VALUE_PER_QUALITY_LEVEL * TotalQuality);

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
        int total = TotalQuality;
        float[] results = new float[QualityLevels.Length];
        for(int i = 0; i < QualityLevels.Length; i++)
            results[i] = (float)QualityLevels[i].Level / total;
        return results;
    }
}