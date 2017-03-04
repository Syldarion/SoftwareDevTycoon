using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Employee : Person
{
    private const float SALARY_SKILL_FACTOR = 0.2f;

    public int Pay { get { return Mathf.CeilToInt(Salary / 12.0f); } }

    public JobTitle CurrentTitle;
    public int HireCost;
    public int Salary;
    public long HireDateBinary;

    public int CurrentOfficeIndex;

    public float Morale { get; private set; }

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

    public static Employee GenerateEmployee()
    {
        JobTitle title = JobTitle.GetRandomTitle();
        Employee new_employee = new Employee();

        new_employee.CurrentTitle = title;
        new_employee.Salary = Mathf.CeilToInt(title.SkillRequirements.Sum() * SALARY_SKILL_FACTOR)
                           * Random.Range(50, 60) * 1000;
        new_employee.HireCost = Mathf.CeilToInt(Random.Range(0.05f, 1.0f) * new_employee.Salary);
        new_employee.Name = PersonNames.GetRandomName();
        new_employee.Age = Random.Range(18, 50);
        new_employee.CurrentLocation = Location.GetRandomLocation();
        new_employee.PersonGender = (Gender)Random.Range(0, Enum.GetNames(typeof(Gender)).Length);
        new_employee.Skills = new SkillList();
        for (int i = 0; i < SkillInfo.COUNT; i++)
            new_employee.Skills[(Skill)i] = title.SkillRequirements[(Skill)i] + Random.Range(0, 3);
        new_employee.Morale = 50.0f;

        return new_employee;
    }
}
