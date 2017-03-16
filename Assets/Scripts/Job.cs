using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class JobTitle
{
    public static JobTitle[][] AllTitles =
    {
        new []
        {
            new JobTitle("Junior Programmer", 1.0f, new SkillList(new []{new SkillLevel(Skill.Programming, 4)})),
            new JobTitle("Mid-Level Programmer", 1.2f, new SkillList(new []{new SkillLevel(Skill.Programming, 6)})),
            new JobTitle("Senior Programmer", 1.5f, new SkillList(new []{new SkillLevel(Skill.Programming, 8)})),
        },
        new []
        {
            new JobTitle("Junior Designer", 1.0f, new SkillList(new []{new SkillLevel(Skill.UserInterfaces, 4)})),
            new JobTitle("Mid-Level Designer", 1.2f, new SkillList(new []{new SkillLevel(Skill.UserInterfaces, 6)})),
            new JobTitle("Senior Designer", 1.5f, new SkillList(new []{new SkillLevel(Skill.UserInterfaces, 8)})),
        },
        new []
        {
            new JobTitle("Junior Database Administrator", 1.0f, new SkillList(new []{new SkillLevel(Skill.Databases, 4)})),
            new JobTitle("Mid-Level Database Administrator", 1.2f, new SkillList(new []{new SkillLevel(Skill.Databases, 6)})),
            new JobTitle("Senior Database Administrator", 1.5f, new SkillList(new []{new SkillLevel(Skill.Databases, 8)})),
        },
        new []
        {
            new JobTitle("Junior Network Administrator", 1.0f, new SkillList(new []{new SkillLevel(Skill.Networking, 4)})),
            new JobTitle("Mid-Level Network Administrator", 1.2f, new SkillList(new []{new SkillLevel(Skill.Networking, 6)})),
            new JobTitle("Senior Network Administrator", 1.5f, new SkillList(new []{new SkillLevel(Skill.Networking, 8)})),
        },
        new []
        {
            new JobTitle("Junior Web Developer", 1.0f, new SkillList(new []{new SkillLevel(Skill.WebDevelopment, 4)})),
            new JobTitle("Mid-Level Web Developer", 1.2f, new SkillList(new []{new SkillLevel(Skill.WebDevelopment, 6)})),
            new JobTitle("Senior Web Developer", 1.5f, new SkillList(new []{new SkillLevel(Skill.WebDevelopment, 8)})),
        }
    };

    public static JobTitle GetRandomTitle()
    {
        int job_type = Random.Range(0, AllTitles.Length);
        int specific_job = Random.Range(0, AllTitles[job_type].Length);
        return AllTitles[job_type][specific_job];
    }

    public string Name;
    public float PayFactor;

    public SkillList SkillRequirements { get; private set; }

    private JobTitle nextLevel;

    public JobTitle(string name, float factor, SkillList reqs)
    {
        Name = name;
        PayFactor = factor;
        SkillRequirements = reqs;
    }

    public bool MeetsRequirements(Person worker)
    {
        for(int i = 0; i < SkillRequirements.Length; i++)
            if (worker.Skills[SkillRequirements[i].Skill] < SkillRequirements[i])
                return false;
        return true;
    }

    public static void SetupLevels(JobTitle[] titles)
    {
        for (int i = 0; i < titles.Length - 1; i++)
            titles[i].nextLevel = titles[i + 1];
    }

    public JobTitle GetNextLevel() { return nextLevel; }
}

[Serializable]
public class JobApplication
{
    public Job AppliedJob;
    public long ApplicationDateBinary;

    public bool Waiting;
    public bool Accepted;

    public JobApplication(Job applied)
    {
        AppliedJob = applied;
        ApplicationDateBinary = TimeManager.CurrentDate.ToBinary();
        Waiting = true;
        Accepted = false;
    }
    
    public void CheckApplication()
    {
        if (Accepted) return;

        TimeSpan time_since_applying = TimeManager.CurrentDate - DateTime.FromBinary(ApplicationDateBinary);
        float chance_for_reply = time_since_applying.Days / 30.0f;

        if (Random.Range(0.0f, 1.0f) >= chance_for_reply) return;

        int base_accept_chance = 75 + AppliedJob.CurrentTitle.SkillRequirements.Skills
            .Sum(skill => Character.MyCharacter.Skills[skill.Skill].Level - skill.Level);

        Accepted = Random.Range(0, 101) < base_accept_chance;

        JobManager.Instance.RefreshApplicationList();

        InformationPanelManager.Instance.DisplayMessage(
            string.Format("{0} responded to your application!", AppliedJob.CompanyName), 2.0f);
    }
}

[Serializable]
public class Job
{
    public const int BASE_SEVERANCE_PAY = 10000;

    public static Job MyJob;

    public string CompanyName;
    public int Salary;
    public JobTitle CurrentTitle;
    public Location JobLocation;
    public int SigningBonus;
    public long HireDateBinary;
    public int Performance {get {return performance;} set { performance = Mathf.Clamp(value, 0, 100); } }
    private int performance;

    [SerializeField]
    private bool isPayWeek;

    public Job()
    {
        isPayWeek = false;
    }

    public void SetupEvents()
    {
        TimeManager.PerDayEvent.RemoveListener(Work);
        TimeManager.PerWeekEvent.RemoveListener(PayPlayer);
        TimeManager.PerYearEvent.RemoveListener(CheckPerformance);

        TimeManager.PerDayEvent.AddListener(Work);
        TimeManager.PerWeekEvent.AddListener(PayPlayer);
        TimeManager.PerYearEvent.AddListener(CheckPerformance);
    }

    public void HirePlayer()
    {
        MyJob = this;

        HireDateBinary = TimeManager.CurrentDate.ToBinary();
        isPayWeek = false;

        Character.MyCharacter.Funds += SigningBonus;

        if (JobLocation != Character.MyCharacter.CurrentLocation)
        {
            Character.MyCharacter.Funds += 5000;
            Character.MyCharacter.CurrentLocation = JobLocation;
        }

        Performance = 80;

        SetupEvents();
    }

    public void Work()
    {
        DayOfWeek current_day = TimeManager.CurrentDate.DayOfWeek;

        int hours_worked_today = 8;

        Performance += hours_worked_today - (current_day == DayOfWeek.Saturday || current_day == DayOfWeek.Sunday ? 0 : 8);
    }

    public void CheckPerformance()
    {
        if(performance == 100)
            Promote();
        else if (performance >= 80)
            if(Random.value > 0.5f)
                Promote();
        else if (performance >= 60)
            GiveRaise(0.01f * (performance - 59));
        else if (performance == 0)
            FirePlayer();
        else if (performance <= 20)
            if(Random.value > 0.5f)
                    FirePlayer();
    }

    public void PayPlayer()
    {
        isPayWeek = !isPayWeek;

        if (!isPayWeek) return;

        Character.MyCharacter.Funds += Mathf.CeilToInt(Salary / 26.0f);

        InformationPanelManager.Instance.DisplayMessage("Payday!", 1.0f);
    }

    public void GiveRaise(float percentage)
    {
        Salary = Mathf.CeilToInt(Salary * (1.0f + percentage));
    }

    public void Promote()
    {
        if (CurrentTitle.GetNextLevel() != null && CurrentTitle.GetNextLevel().MeetsRequirements(Character.MyCharacter))
            CurrentTitle = CurrentTitle.GetNextLevel();
        GiveRaise(0.2f);

        InformationPanelManager.Instance.DisplayMessage(
            string.Format("You've been promoted to {0} at {1}!", CurrentTitle.Name, CompanyName), 2.0f);
    }

    public void FirePlayer()
    {
        TimeManager.PerWeekEvent.RemoveListener(PayPlayer);

        TimeSpan job_length = TimeManager.CurrentDate - DateTime.FromBinary(HireDateBinary);
        int total_sev_pay = BASE_SEVERANCE_PAY + Mathf.CeilToInt((job_length.Days / 365.0f) * (Salary / 12.0f));

        Character.MyCharacter.Funds += total_sev_pay;

        InformationPanelManager.Instance.DisplayMessage(
            string.Format("You've been fired from {0}!", CompanyName), 2.0f);
    }

    public static Job[] GenerateJobs(int count)
    {
        var jobs = new List<Job>();

        int char_name_val = Character.MyCharacter.Name.Aggregate(0, (current, c) => current + c);
        Random.InitState(TimeManager.Month * TimeManager.Year * (char_name_val + 1));

        for (int i = 0; i < count; i++)
        {
            var new_job = new Job();
            new_job.CompanyName = CompanyNames.GetRandomName();
            new_job.Salary = Random.Range(50, 60) * 1000;
            new_job.CurrentTitle = JobTitle.GetRandomTitle();
            new_job.JobLocation = Location.GetRandomLocation();
            new_job.Salary = Mathf.CeilToInt(new_job.Salary * new_job.CurrentTitle.PayFactor);
            new_job.SigningBonus = Mathf.CeilToInt(Random.Range(0.05f, 1.0f) * new_job.Salary);

            jobs.Add(new_job);
        }

        return jobs.ToArray();
    }
}
