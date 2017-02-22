using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

public enum CharacterGender
{
    Male = 0,
    Female = 1
}

[Serializable]
public class Person
{
    public enum Gender
    {
        Male,
        Female
    }

    public string Name;
    public int Age;
    public Gender PersonGender;
    public Location CurrentLocation;

    public SkillLevel[] Skills =
    {
        new SkillLevel(Skill.Programming, 1),
        new SkillLevel(Skill.UserInterfaces, 1),
        new SkillLevel(Skill.Databases, 1),
        new SkillLevel(Skill.Networking, 1),
        new SkillLevel(Skill.WebDevelopment, 1),
    };
}

[Serializable]
public class Character : Person
{
    public static Character MyCharacter;
    
    public string Birthday;

    public int Money { get; private set; }

    public int Reputation
    {
        get { return reputation; }
        set { reputation = Mathf.Clamp(value, 0, 100); }
    }
    private int reputation;

    public int Exhaustion { get {return exhaustion;} set { exhaustion = Mathf.Clamp(value, 0, 5); } }
    private int exhaustion;

    public Character()
    {
        MyCharacter = this;
    }

    public void SetupEvents()
    {
        TimeManager.PerDayEvent.RemoveListener(CheckBirthday);
        TimeManager.PerDayEvent.RemoveListener(Sleep);

        TimeManager.PerDayEvent.AddListener(CheckBirthday);
        TimeManager.PerDayEvent.AddListener(Sleep);
    }

    public void AdjustMoney(int adjustment)
    {
        Money += adjustment;
    }

    public void CheckBirthday()
    {
        string[] bd_separated = Birthday.Split('-');
        if (bd_separated.Length != 3) return;
        int day = int.Parse(bd_separated[0]);
        int month = int.Parse(bd_separated[1]);

        if (day == TimeManager.Day && month == TimeManager.Month)
        {
            Age++;
        }
    }

    public void Sleep()
    {
        int sleep = 8;
        if (sleep < 6) Exhaustion++;
        else if (sleep >= 8) Exhaustion--;
    }
}