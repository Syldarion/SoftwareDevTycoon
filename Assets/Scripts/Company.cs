using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class Employee : Person
{
    public JobTitle CurrentTitle;
    public int HireCost;
    public int Pay;
    public long HireDateBinary;

    public Employee()
    {
        
    }

    public void GiveRaise(float percentage)
    {
        Pay = Mathf.CeilToInt(Pay * (1.0f + percentage));
    }

    public void Promote()
    {
        if (CurrentTitle.GetNextLevel() != null && CurrentTitle.GetNextLevel().MeetsRequirements(this))
            CurrentTitle = CurrentTitle.GetNextLevel();
        GiveRaise(0.2f);
    }

    public static Employee GenerateEmployee(JobTitle positionToFill)
    {
        Employee new_employee = new Employee();

        new_employee.CurrentTitle = positionToFill;
        new_employee.Pay = Mathf.CeilToInt((positionToFill.SkillRequirements.Sum() - 4) * 0.2f)
                           * Random.Range(50, 76) * 1000;
        new_employee.HireCost = Mathf.CeilToInt(Random.Range(0.05f, 1.0f) * new_employee.Pay);
        new_employee.Name = PersonNames.GetRandomName();
        new_employee.Age = Random.Range(18, 50);
        new_employee.CurrentLocation = Location.GetRandomLocation();
        new_employee.PersonGender = (Gender)Random.Range(0, Enum.GetNames(typeof(Gender)).Length);
        new_employee.Skills = new SkillLevel[5];
        for (int i = 0; i < 5; i++)
            new_employee.Skills[i].Level = positionToFill.SkillRequirements[i] + Random.Range(0, 3);

        return new_employee;
    }
}

[System.Serializable]
public class Company
{
    public static Company MyCompany;

    //Properties
    public int TeamSize { get { return employees.Count; } }
    
    //Public Fields
    public Project CompanyProject;
    public List<Office> CompanyOffices;

    //Private Fields
    private string name;
    private int reputation;
    private int funds;
    private int baseSeverancePay;
    private List<Employee> employees;

    public Company(string name)
    {
        this.name = name;

        CompanyOffices = new List<Office>();
        employees = new List<Employee>();
    }

    public void SetupEvents()
    {
        TimeManager.PerMonthEvent.RemoveListener(PayMonthlyCosts);

        TimeManager.PerMonthEvent.AddListener(PayMonthlyCosts);
    }

    public void PayMonthlyCosts()
    {
        PayEmployees();
        PayForOffices();
    }

    public void AdjustFunds(int adjustment)
    {
        funds += adjustment;
    }

    public void AdjustReputation(int adjustment)
    {
        reputation = Mathf.Clamp(reputation + adjustment, 0, 100);
    }

    public void PayEmployees()
    {
        int total_payroll = employees.Aggregate(0, (current, emp) => current + emp.Pay);
    }

    public void PayForOffices()
    {
        int total_cost = CompanyOffices.Aggregate(0, (current, office) => current + office.TotalUpkeepCost);
    }

    public void HireEmployee(Employee employee)
    {
        if(employees.Contains(employee))
            return;
        
        AdjustFunds(-employee.HireCost);
        employee.HireDateBinary = TimeManager.CurrentDate.ToBinary();

        employees.Add(employee);
    }

    public void FireEmployee(Employee employee)
    {
        if (!employees.Contains(employee))
            return;
        
        TimeSpan job_length = TimeManager.CurrentDate - DateTime.FromBinary(employee.HireDateBinary);
        int total_sev_pay = baseSeverancePay + Mathf.CeilToInt((job_length.Days / 356.0f) * (employee.Pay / 12.0f));

        AdjustFunds(-total_sev_pay);

        employees.Remove(employee);
    }

    public void TrainEmployee(Employee employee)
    {
        
    }

    //Getters / Setters

    public string Name()
    {
        return name;
    }

    public int Reputation()
    {
        return reputation;
    }

    public int Funds()
    {
        return funds;
    }
}