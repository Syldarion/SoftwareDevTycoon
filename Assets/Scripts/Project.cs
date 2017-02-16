using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Project
{
    public string Name;

    public Project(string name)
    {
        if (name == string.Empty) name = "New Project";
        if (name.Length > 20) name = name.Substring(0, 20);
        Name = name;
    }

    public static Project GenerateProject(int taskCount)
    {
        Project new_project = new Project("Job Project");

        return new_project;
    }
}