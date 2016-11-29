using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[System.Serializable]
public class JobTitle
{
    public string Name { get; private set; }

    public int[] SkillRequirements { get; private set; }

    private JobTitle nextLevel;

    public JobTitle(string name, int[] skillReqs)
    {
        Name = name;
        SkillRequirements = new int[5];
        for (int i = 0; i < skillReqs.Length && i < 5; i++)
            SkillRequirements[i] = Mathf.Abs(skillReqs[i]);
    }

    public bool MeetsRequirements(Person worker)
    {
        for(int i = 0; i < 5; i++)
            if (worker.Skills[i].Level < SkillRequirements[i])
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

[System.Serializable]
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

        int base_accept_chance = 75;
        for (int i = 0; i < 5; i++)
            base_accept_chance += Character.MyCharacter.Skills[i].Level - AppliedJob.CurrentTitle.SkillRequirements[i];

        Accepted = Random.Range(0, 101) < base_accept_chance;
    }
}

[Serializable]
public class Job
{
    public static Job MyJob;

    public string CompanyName;
    public int Salary;
    public JobTitle CurrentTitle;
    public Location JobLocation;
    public int SigningBonus;
    public int SeverancePay;
    public long HireDateBinary; //has to be this for serialization
    public int Performance {get {return performance;} set { performance = Mathf.Clamp(value, 0, 100); } }
    private int performance;

    [SerializeField] private bool isPayWeek;

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

        Character.MyCharacter.AdjustMoney(SigningBonus);

        if (JobLocation != Character.MyCharacter.CurrentLocation)
        {
            Character.MyCharacter.AdjustMoney(5000);
            Character.MyCharacter.CurrentLocation = JobLocation;
        }

        Performance = 80;

        SetupEvents();
    }

    public void Work()
    {
        DayOfWeek current_day = TimeManager.CurrentDate.DayOfWeek;

        int hours_worked_today =
            ScheduleManager.Instance.ActiveSchedule.GetSchedule(current_day)
                .Items.Count(x => x == ScheduleItem.Job);

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

        Character.MyCharacter.AdjustMoney(Mathf.CeilToInt(Salary / 26.0f));
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
    }

    public void FirePlayer()
    {
        TimeManager.PerWeekEvent.RemoveListener(PayPlayer);

        TimeSpan job_length = TimeManager.CurrentDate - DateTime.FromBinary(HireDateBinary);
        int total_sev_pay = SeverancePay + Mathf.CeilToInt((job_length.Days / 30.0f) * (Salary / 12.0f));

        Character.MyCharacter.AdjustMoney(total_sev_pay);
    }

    public static Job[] GenerateJobs(int count)
    {
        List<Job> jobs = new List<Job>();

        int char_name_val = Character.MyCharacter.Name.Aggregate(0, (current, c) => current + c);
        Random.InitState(TimeManager.Month * TimeManager.Year * (char_name_val + 1));

        for (int i = 0; i < count; i++)
        {
            Job new_job = new Job();
            new_job.Salary = Random.Range(50, 76) * 1000;

            int sector = Random.Range(0, JobTitles.AllTitles.Length);
            int job_title = Random.Range(0, JobTitles.AllTitles[sector].Length);

            new_job.CurrentTitle = JobTitles.AllTitles[sector][job_title];

            new_job.JobLocation = Location.GetRandomLocation();

            new_job.Salary = Mathf.CeilToInt(new_job.Salary * (1.0f + job_title * 0.3f));

            new_job.SigningBonus = Mathf.CeilToInt(Random.Range(0.05f, 1.0f) * new_job.Salary);
            new_job.SeverancePay = Random.Range(1, 7) * new_job.Salary;

            jobs.Add(new_job);
        }

        return jobs.ToArray();
    }
}