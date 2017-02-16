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

    private float morale;

    public Employee()
    {
        
    }

    public void GiveRaise(float percentage)
    {
        Pay = Mathf.CeilToInt(Pay * (1.0f + percentage));
        morale += percentage * 0.5f;
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
        new_employee.morale = 50.0f;

        return new_employee;
    }
}

[Serializable]
public class Company
{
    public static Company MyCompany;

    public const int BASE_COMPANY_COST = 500000;

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

    public static Company CreateNewCompany(string name, int initSpace)
    {
        if (MyCompany != null) return MyCompany;

        Company new_company = new Company(name);

        Office init_office = new Office(initSpace);
        init_office.AddOfficeBuilding(new MainBuilding());

        new_company.AddOffice(init_office);

        int new_company_cost = BASE_COMPANY_COST + (Office.COST_PER_SPACE * initSpace);

        Character.MyCharacter.AdjustMoney(-new_company_cost);

        return new_company;
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

        //check for bankruptcy and shit
    }

    public void DeclareBankruptcy()
    {
        //rekt
    }

    public void AdjustReputation(int adjustment)
    {
        reputation = Mathf.Clamp(reputation + adjustment, 0, 100);
    }

    public void AddOffice(Office newOffice)
    {
        CompanyOffices.Add(newOffice);
        CompanyManager.Instance.UpdateCompanyInfoPanel();
    }

    public void PayEmployees()
    {
        int total_payroll = employees.Aggregate(0, (current, emp) => current + emp.Pay);
        AdjustFunds(-total_payroll);
        AdjustReputation(1);
    }

    public void PayForOffices()
    {
        int total_cost = CompanyOffices.Aggregate(0, (current, office) => current + office.TotalUpkeepCost);
        AdjustFunds(-total_cost);
    }

    public void HireEmployee(Employee employee)
    {
        if(employees.Contains(employee))
            return;
        
        AdjustFunds(-employee.HireCost);
        employee.HireDateBinary = TimeManager.CurrentDate.ToBinary();

        employees.Add(employee);

        CompanyManager.Instance.UpdateCompanyInfoPanel();
    }

    public void FireEmployee(Employee employee)
    {
        if (!employees.Contains(employee))
            return;
        
        TimeSpan job_length = TimeManager.CurrentDate - DateTime.FromBinary(employee.HireDateBinary);
        int total_sev_pay = baseSeverancePay + Mathf.CeilToInt((job_length.Days / 356.0f) * (employee.Pay / 12.0f));

        AdjustFunds(-total_sev_pay);
        AdjustReputation(-1);

        employees.Remove(employee);

        CompanyManager.Instance.UpdateCompanyInfoPanel();
    }

    public void TrainEmployee(Employee employee)
    {
        int skill_sum = Mathf.Clamp(employee.Skills.Sum(x => x.Level), 1, int.MaxValue);
        int training_cost = 500 * skill_sum;

        AdjustFunds(-training_cost);

        for (int i = 0; i < employee.Skills.Length; i++)
        {
            float skill_makeup_percentage = (float)employee.Skills[i].Level / skill_sum;
            int amount_to_increase = Mathf.FloorToInt(10.0f * skill_makeup_percentage * Random.Range(0.5f, 2.0f)) + 1;
            employee.Skills[i].Level += amount_to_increase;
        }
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

    public IEnumerable<Employee> EmployeeList()
    {
        return employees;
    }
}