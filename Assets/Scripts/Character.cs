using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Random = UnityEngine.Random;

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

    public SkillList Skills = new SkillList(new[] {
        new SkillLevel(Skill.Programming, 1),
        new SkillLevel(Skill.UserInterfaces, 1),
        new SkillLevel(Skill.Databases, 1),
        new SkillLevel(Skill.Networking, 1),
        new SkillLevel(Skill.WebDevelopment, 1),
    });
}

[Serializable]
public class Character : Person
{
    public static Character MyCharacter;

    public Contract ActiveContract;
    public string Birthday;

    public int Money { get; private set; }

    public int Reputation { get; private set; }

    public Character()
    {
        MyCharacter = this;
    }

    public void SetupEvents()
    {
        TimeManager.PerDayEvent.RemoveListener(CheckBirthday);

        TimeManager.PerDayEvent.AddListener(CheckBirthday);
    }

    public void AdjustMoney(int adjustment)
    {
        Money = Mathf.Clamp(Money + adjustment, -int.MaxValue, int.MaxValue);
    }

    public void AdjustReputation(int adjustment)
    {
        Reputation = Mathf.Clamp(Reputation + adjustment, 0, 100);
    }

    public void WorkOnContract()
    {
        SkillList work = new SkillList();
        for(int i = 0; i < work.Length; i++)
            work[i] = Skills[i] + Random.Range(-1, 2);
        
        if(ActiveContract.ApplyWork(work))
            Contract.SetPlayerActiveContract(null);
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
}