using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Employee : Person
{
    private const float salary_skill_factor = 0.2f;

    public int Pay
    {
        get
        {
            return Mathf.CeilToInt((Salary / 12.0f));
        }
    }

    public JobTitle CurrentTitle;
    public int HireCost;
    public int Salary;
    public long HireDateBinary;

    public int CurrentOfficeIndex;

    public float Morale { get; private set; }
    public int WorkingHours
    {
        get
        {
            return workingHours;
        }
        set
        {
            if (lockWorkingHours) return;
            workingHours = Mathf.Clamp(value, 0, 80);
            lockWorkingHours = true;
        }
    }

    [SerializeField]
    private int workingHours;
    private bool lockWorkingHours;

    public Employee()
    {

    }

    public void GiveRaise(float percentage)
    {
        Salary = Mathf.CeilToInt(Salary * (1.0f + percentage));
        Morale += percentage * 0.5f;
    }

    public void Promote()
    {
        if (CurrentTitle.GetNextLevel() != null && CurrentTitle.GetNextLevel().MeetsRequirements(this))
            CurrentTitle = CurrentTitle.GetNextLevel();
        GiveRaise(0.2f);
    }

    public SkillList Work()
    {
        SkillList to_return = Skills;

        foreach (SkillLevel skill in to_return.Skills)
            skill.Level = Mathf.CeilToInt(skill.Level * 
                (workingHours / 40.0f) * (Morale / 70.0f));
        
        return to_return;
    }

    public void AdjustMorale()
    {
        int difference_from_norm = workingHours - 40;
        Morale -= Mathf.Pow(difference_from_norm, 1.1f);
    }

    public int SetWorkingHours(int newHours)
    {
        WorkingHours = newHours;
        return WorkingHours;
    }

    public static Employee GenerateEmployee()
    {
        var title = JobTitle.GetRandomTitle();
        var new_employee = new Employee();

        new_employee.CurrentTitle = title;
        new_employee.Salary = Mathf.CeilToInt(title.SkillRequirements.Sum() * salary_skill_factor)
                           * Random.Range(50, 60) * 1000;
        new_employee.HireCost = Mathf.CeilToInt(Random.Range(0.05f, 1.0f) * new_employee.Salary);
        new_employee.Name = PersonNames.GetRandomName();
        new_employee.Age = Random.Range(18, 50);
        new_employee.CurrentLocation = Location.GetRandomLocation();
        new_employee.Skills = new SkillList();
        for (int i = 0; i < SkillInfo.COUNT; i++)
            new_employee.Skills[(Skill)i] = title.SkillRequirements[(Skill)i] + Random.Range(2, 5);
        new_employee.Morale = 50.0f;

        return new_employee;
    }
}
