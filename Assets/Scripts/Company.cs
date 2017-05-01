using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
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
    public int Funds
    {
        get { return funds; }
        set
        {
            funds = Mathf.Clamp(value, -int.MaxValue, int.MaxValue);
            StatusBarManager.Instance.UpdateFunds(funds);
        }
    }
    public int Reputation
    {
        get { return reputation; }
        set { reputation = Mathf.Clamp(value, 0, 100); }
    }
    public int TeamSize { get { return employees.Count; } }

    //Public Fields
    public List<Project> CompanyProjects;
    public Project ActiveCompanyProject;
    public Contract ActiveContract;
    public List<Office> CompanyOffices;

    //Private Fields
    [SerializeField]
    private string companyName;
    [SerializeField]
    private int funds;
    [SerializeField]
    private int reputation;
    [SerializeField]
    private List<Employee> employees;

    public Company(string name)
    {
        companyName = name;

        CompanyProjects = new List<Project>();
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
            OfficeLocation = GameManager.ActiveCharacter.CurrentLocation
        };
        new_company.AddOffice(init_office);

        int new_company_cost = BASE_COMPANY_COST + (Office.COST_PER_SPACE * init_office.Space);

        if(GameManager.ActiveCharacter.ActiveContract != null)
        {
            Contract temp = GameManager.ActiveCharacter.ActiveContract;
            Contract.SetPlayerActiveContract(null);
            Contract.SetCompanyActiveContract(temp);
        }

        if(Job.MyJob != null)
            Job.MyJob.FirePlayer();
        
        int temp_funds = GameManager.ActiveCharacter.Funds - new_company_cost;
        GameManager.ActiveCharacter.Funds = 0;
        new_company.Funds = temp_funds;
        
        return new_company;
    }

    public void SetupEvents()
    {
        TimeManager.PerMonthEvent.RemoveListener(PayMonthlyCosts);
        TimeManager.PerDayEvent.RemoveListener(WorkOnActiveContract);
        TimeManager.PerDayEvent.RemoveListener(WorkOnProject);

        TimeManager.PerMonthEvent.AddListener(PayMonthlyCosts);
        TimeManager.PerDayEvent.AddListener(WorkOnActiveContract);
        TimeManager.PerDayEvent.AddListener(WorkOnProject);
    }

    public void PayMonthlyCosts()
    {
        PayEmployees();
        PayForOffices();
    }

    public void DeclareBankruptcy()
    {
        //rekt
    }

    public void AddOffice(Office newOffice)
    {
        CompanyOffices.Add(newOffice);
    }

    public void RemoveOffice(Office office)
    {
        if(CompanyOffices.Contains(office))
            CompanyOffices.Remove(office);
    }

    public void PayEmployees()
    {
        int total_payroll = employees.Aggregate(0, (current, emp) => current + emp.Pay);
        Funds -= total_payroll;
        Reputation++;
    }

    public void PayForOffices()
    {
        int total_cost = CompanyOffices.Aggregate(0, (current, office) => current + office.TotalUpkeepCost);
        Funds -= total_cost;
    }

    public void HireEmployee(Employee employee)
    {
        if(employees.Contains(employee))
            return;
        
        Funds -= employee.HireCost;
        employee.HireDateBinary = TimeManager.CurrentDate.ToBinary();

        employees.Add(employee);

        MyCompany.CompanyOffices.First().Employees.Add(employee);
    }

    public void FireEmployee(Employee employee)
    {
        if (!employees.Contains(employee))
            return;
        
        TimeSpan job_length = TimeManager.CurrentDate - DateTime.FromBinary(employee.HireDateBinary);
        int total_sev_pay = BASE_SEVERANCE_PAY + 
            Mathf.CeilToInt((job_length.Days / 356.0f) * (employee.Salary / 12.0f));
        
        Funds -= total_sev_pay;
        Reputation--;

        RemoveEmployee(employee);
    }

    public void RemoveEmployee(Employee employee)
    {
        employees.Remove(employee);
        foreach (Office office in CompanyOffices)
            if (office.Employees.Contains(employee))
            {
                office.Employees.Remove(employee);
                break;
            }
    }

    public void TrainEmployee(Employee employee, int trainCost)
    {
        int skill_sum = Mathf.Clamp(employee.Skills.Sum(), 1, int.MaxValue);

        Funds -= trainCost;

        for (int i = 0; i < SkillInfo.COUNT; i++)
        {
            float skill_makeup_percentage = (float)employee.Skills[i].Level / skill_sum;
            int amount_to_increase = Mathf.FloorToInt(10.0f * skill_makeup_percentage * Random.Range(0.5f, 2.0f)) + 1;
            employee.Skills[i].Level += amount_to_increase;
        }
    }

    public void WorkOnActiveContract()
    {
        if (ActiveContract == null) return;

        var work_sums = new SkillList();
        foreach (Office office in CompanyOffices)
        {
            var office_work_sum = new SkillList();
            foreach (Employee emp in office.Employees)
                office_work_sum += emp.Skills;
            office_work_sum = office.ApplyQualityBonus(office_work_sum);
            work_sums += office_work_sum;
        }

        if (ActiveContract.ApplyWork(work_sums))
            Contract.SetCompanyActiveContract(null);
    }

    public void WorkOnProject()
    {
        if (ActiveCompanyProject == null) return;

        var work_sums = new SkillList();
        foreach (Office office in CompanyOffices)
        {
            var office_work_sum = new SkillList();
            foreach (Employee emp in office.Employees)
                office_work_sum += emp.Skills;
            office_work_sum = office.ApplyQualityBonus(office_work_sum);
            work_sums += office_work_sum;
        }

        ActiveCompanyProject.ApplyWork(work_sums);
    }

    public void SetActiveProject(Project project)
    {
        if (project == null || 
            (project.CurrentStatus == Project.Status.InProgress || 
            project.CurrentStatus == Project.Status.Halted)) return;

        if (ActiveCompanyProject != null)
            ActiveCompanyProject.CurrentStatus = Project.Status.Halted;

        ActiveCompanyProject = project;
        ActiveCompanyProject.CurrentStatus = Project.Status.InProgress;
    }

    public void CheckEmployeeMorale()
    {
        foreach(Office office in CompanyOffices)
        {
            foreach(Employee emp in office.Employees)
            {
                if (Random.Range(0, 30) > emp.Morale)
                {
                    RemoveEmployee(emp);
                    InformationPanelManager.Instance.DisplayMessage(
                        string.Format("{0} quit!", emp.Name), 1.0f);
                }
            }
        }
    }

    //Getters / Setters

    public IEnumerable<Employee> EmployeeList()
    {
        return employees;
    }
}
