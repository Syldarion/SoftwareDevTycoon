using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class Company
{
    public static Company MyCompany;

    public const int BASE_COMPANY_COST = 100000;
    public const int BASE_SEVERANCE_PAY = 20000;
    public const int TRAINING_COST_MULTIPLIER = 500;

    //Properties
    public string Name { get { return companyName; } }
    public int Reputation { get { return reputation; } }
    public int Funds { get { return funds; } }
    public int TeamSize { get { return employees.Count; } }
    
    //Public Fields
    public Project CompanyProject;
    public Contract ActiveContract;
    public List<Office> CompanyOffices;

    //Private Fields
    [SerializeField]
    private string companyName;
    [SerializeField]
    private int reputation;
    [SerializeField]
    private int funds;
    [SerializeField]
    private List<Employee> employees;

    public Company(string name)
    {
        companyName = name;

        CompanyOffices = new List<Office>();
        employees = new List<Employee>();

        SetupEvents();
    }

    public static Company CreateNewCompany(string name, int initSpace)
    {
        if (MyCompany != null) return MyCompany;

        var new_company = new Company(name);

        var init_office = new Office(initSpace)
        {
            OfficeLocation = Character.MyCharacter.CurrentLocation
        };
        new_company.AddOffice(init_office);

        int new_company_cost = BASE_COMPANY_COST + (Office.COST_PER_SPACE * init_office.Space);

        Character.MyCharacter.AdjustMoney(-new_company_cost);
        new_company.AdjustFunds(Character.MyCharacter.Money);
        Character.MyCharacter.AdjustMoney(-Character.MyCharacter.Money);

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
        StatusBarManager.Instance.UpdateCompanyInfo();
    }

    public void RemoveOffice(Office office)
    {
        if(CompanyOffices.Contains(office))
            CompanyOffices.Remove(office);
        StatusBarManager.Instance.UpdateCompanyInfo();
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

        MyCompany.CompanyOffices.First().Employees.Add(employee);

        StatusBarManager.Instance.UpdateCompanyInfo();
    }

    public void FireEmployee(Employee employee)
    {
        if (!employees.Contains(employee))
            return;
        
        TimeSpan job_length = TimeManager.CurrentDate - DateTime.FromBinary(employee.HireDateBinary);
        int total_sev_pay = BASE_SEVERANCE_PAY + 
            Mathf.CeilToInt((job_length.Days / 356.0f) * (employee.Salary / 12.0f));

        AdjustFunds(-total_sev_pay);
        AdjustReputation(-1);

        employees.Remove(employee);

        StatusBarManager.Instance.UpdateCompanyInfo();
    }

    public void TrainEmployee(Employee employee)
    {
        int skill_sum = Mathf.Clamp(employee.Skills.Sum(), 1, int.MaxValue);
        int training_cost = TRAINING_COST_MULTIPLIER * skill_sum;

        AdjustFunds(-training_cost);

        for (int i = 0; i < SkillInfo.COUNT; i++)
        {
            float skill_makeup_percentage = (float)employee.Skills[i].Level / skill_sum;
            int amount_to_increase = Mathf.FloorToInt(10.0f * skill_makeup_percentage * Random.Range(0.5f, 2.0f)) + 1;
            employee.Skills[i].Level += amount_to_increase;
        }
    }

    public void WorkOnActiveContract()
    {
        var work_sums = new SkillList();
        for (int i = 0; i < SkillInfo.COUNT; i++)
        {
            foreach (Office office in CompanyOffices)
            {
                float office_work_sum = office.Employees.Sum(x => x.Skills[i].Level * (x.Morale / 70.0f));
                office_work_sum = office_work_sum * (1.0f + office.QualityBonuses[i]);
                work_sums[(Skill)i] += Mathf.CeilToInt(office_work_sum);
            }
        }

        if(ActiveContract.ApplyWork(work_sums))
            Contract.SetCompanyActiveContract(null);
    }

    public void WorkOnProject()
    {
        var work_sums = new SkillList();
        for (int i = 0; i < SkillInfo.COUNT; i++)
        {
            foreach (Office office in CompanyOffices)
            {
                float office_work_sum = office.Employees.Sum(x => x.Skills[i].Level * (x.Morale / 70.0f));
                office_work_sum = office_work_sum * (1.0f + office.QualityBonuses[i]);
                work_sums[(Skill)i] += Mathf.CeilToInt(office_work_sum);
            }
        }

        CompanyProject.ApplyWork(work_sums);
    }

    //Getters / Setters

    public IEnumerable<Employee> EmployeeList()
    {
        return employees;
    }
}
