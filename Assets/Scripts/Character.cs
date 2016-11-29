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
public class Skill
{
    public enum SkillType
    {
        Programming,
        UserInterfaces,
        Databases,
        Networking,
        WebDevelopment
    }

    private static readonly Skill[] allSkills = {
        new Skill("Programming", "PRG"),
        new Skill("User Interfaces", "UIX"),
        new Skill("Databases", "DBS"),
        new Skill("Networking", "NTW"),
        new Skill("Web Development", "WEB")
    };

    public static Skill Programming = allSkills[0];
    public static Skill UserInterfaces = allSkills[1];
    public static Skill Databases = allSkills[2];
    public static Skill Networking = allSkills[3];
    public static Skill WebDevelopment = allSkills[4];

    public static Skill GetSkillFromEnum(SkillType type)
    {
        return allSkills[(int)type];
    }

    public string Name { get; private set; }
    public string Code { get; private set; }

    private Skill(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public static bool operator==(Skill a, Skill b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if ((object)a == null || (object)b == null)
            return false;

        return (a.Name == b.Name) && (a.Code == b.Code);
    }

    public static bool operator!=(Skill a, Skill b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        Skill skill = obj as Skill;
        if ((object)skill == null)
            return false;

        return (Name == skill.Name) && (Code == skill.Code);
    }

    public bool Equals(Skill skill)
    {
        if ((object)skill == null)
            return false;

        return (Name == skill.Name) && (Code == skill.Code);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + Code.GetHashCode();
            return hash;
        }
    }
}

[Serializable]
public struct SkillLevel
{
    public Skill Skill;
    public int Level;

    public SkillLevel(Skill skill, int level)
    {
        Skill = skill;
        Level = level;
    }
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
        int sleep = ScheduleManager.Instance.ActiveSchedule.GetTodaysSchedule()
            .Items.Count(x => x == ScheduleItem.Sleep);

        if (sleep < 6) Exhaustion++;
        else if (sleep >= 8) Exhaustion--;
    }
}