﻿using System;
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
    public const int TRAINING_COST_MULTIPLIER = 500;

    //Properties
    public string Name { get { return name; } }
    public int Reputation { get { return reputation; } }
    public int Funds { get { return funds; } }
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
        init_office.OfficeLocation = Character.MyCharacter.CurrentLocation;

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

        StatusBarManager.Instance.UpdateCompanyInfo();
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

        StatusBarManager.Instance.UpdateCompanyInfo();
    }

    public void TrainEmployee(Employee employee)
    {
        int skill_sum = Mathf.Clamp(employee.Skills.Sum(x => x.Level), 1, int.MaxValue);
        int training_cost = TRAINING_COST_MULTIPLIER * skill_sum;

        AdjustFunds(-training_cost);

        for (int i = 0; i < employee.Skills.Length; i++)
        {
            float skill_makeup_percentage = (float)employee.Skills[i].Level / skill_sum;
            int amount_to_increase = Mathf.FloorToInt(10.0f * skill_makeup_percentage * Random.Range(0.5f, 2.0f)) + 1;
            employee.Skills[i].Level += amount_to_increase;
        }
    }

    public void WorkOnProject()
    {
        int[] work_sums = new int[employees[0].Skills.Length];
        for(int i = 0; i < work_sums.Length; i++)
        {
            foreach(Office office in CompanyOffices)
            {
                int office_work_sum = office.Employees.Sum(x => x.Skills[i].Level);
                office_work_sum = Mathf.CeilToInt(office_work_sum * (1.0f + office.QualityBonuses[i]));
                work_sums[i] = office_work_sum;
            }
        }
        
        for(int i = 0; i < work_sums.Length; i++)
            CompanyProject.ApplyWork(employees[0].Skills[i].Skill, work_sums[i]);
    }

    //Getters / Setters

    public IEnumerable<Employee> EmployeeList()
    {
        return employees;
    }
}