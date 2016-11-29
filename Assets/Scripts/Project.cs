using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ProjectTask
{
    //Properties
    public bool Complete { get { return !WorkNeeded.Any(x => x > 0); } }
    public bool OnTime { get { return DaysRemaining < 0; } }
    public float Progress { get { return (float)WorkNeeded.Sum() / TotalWorkNeeded; } }

    //Public fields
    public string Name;
    public int[] WorkNeeded;
    public int TotalWorkNeeded;
    public int DaysRemaining;
    public int DaysToComplete;
    public Person Worker;

    //Private fields

    public ProjectTask()
    {
        Name = "N/A";
        WorkNeeded = new int[5];
        TotalWorkNeeded = 0;
        DaysRemaining = 0;
        DaysToComplete = 0;
        Worker = null;
    }

    public ProjectTask(string name, int[] workNeeded, int days)
    {
        if (name == string.Empty) name = "Default";
        if (name.Length > 20) name = name.Substring(0, 20);
        Name = name;

        SetWorkNeeded(workNeeded);

        DaysToComplete = DaysRemaining = days;
    }

    public void SetWorkNeeded(int[] workNeeded)
    {
        WorkNeeded = new int[5];
        for (int i = 0; i < workNeeded.Length && i < 5; i++)
            WorkNeeded[i] = Mathf.Abs(workNeeded[i]);
        TotalWorkNeeded = WorkNeeded.Sum();
    }

    public void SetWorker(Person worker)
    {
        Worker = worker;
        if(Worker != null)
            TimeManager.PerDayEvent.AddListener(WorkOnTask);
        else 
            TimeManager.PerDayEvent.RemoveListener(WorkOnTask);
    }

    public void WorkOnTask()
    {
        for (int i = 0; i < WorkNeeded.Length; i++)
        {
            if (WorkNeeded[i] <= 0) continue;
            int to_apply = Worker.Skills[i].Level + Random.Range(-1, 2);
            WorkNeeded[i] = Mathf.Clamp(
                WorkNeeded[i] - to_apply, 0, int.MaxValue);
            break;
        }

        DaysRemaining--;

        //update task info UI
        
        if (Complete)
        {
            SetWorker(null);
        }
    }

    public static ProjectTask GenerateTask(int multiplier)
    {
        PrebuiltProjectTasks.TaskNameWorkPair task_info =
            PrebuiltProjectTasks.AllTasks[Random.Range(0, PrebuiltProjectTasks.AllTasks.Length)];

        ProjectTask new_task = new ProjectTask();
        new_task.Name = task_info.Name;
        for(int i = 0; i < multiplier; i++)
            for (int j = 0; j < 5; j++)
                new_task.WorkNeeded[j] += task_info.Work[j] + Random.Range(-1, 2);
        new_task.TotalWorkNeeded = new_task.WorkNeeded.Sum();
        new_task.DaysRemaining = new_task.DaysToComplete = multiplier;

        return new_task;    
    }
}

[System.Serializable]
public class Project
{
    public string Name;
    public List<ProjectTask> ProjectTasks;

    public int TotalWorkRemaining
    {
        get { return ProjectTasks.Aggregate(0, (current, task) => current + task.WorkNeeded.Sum()); }
    }
    public int TotalWorkNeeded
    {
        get { return ProjectTasks.Aggregate(0, (current, task) => current + task.TotalWorkNeeded); }
    }
    public float Progress { get { return (float)TotalWorkRemaining / TotalWorkNeeded; } }

    public Project(string name)
    {
        if (name == string.Empty) name = "New Project";
        if (name.Length > 20) name = name.Substring(0, 20);
        Name = name;

        ProjectTasks = new List<ProjectTask>();
    }

    public static Project GenerateProject(int taskCount)
    {
        Project new_project = new Project("Job Project");
        for(int i = 0; i < taskCount; i++)
            new_project.ProjectTasks.Add(ProjectTask.GenerateTask(Random.Range(1, 14)));

        return new_project;
    }
}