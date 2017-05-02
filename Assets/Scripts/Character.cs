using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
public class Person
{
    public string Name;
    public int Age;
    public Location CurrentLocation;
    public bool IsMale;

    [SerializeField]
    private float[] headColor;
    [SerializeField]
    private float[] bodyColor;
    [SerializeField]
    private float[] legsColor;

    public SkillList Skills = new SkillList(new[] {
        new SkillLevel(Skill.Programming, 1),
        new SkillLevel(Skill.UserInterfaces, 1),
        new SkillLevel(Skill.Databases, 1),
        new SkillLevel(Skill.Networking, 1),
        new SkillLevel(Skill.WebDevelopment, 1),
    });

    public void SetHeadColor(Color color)
    {
        headColor = new [] {
            color.r,
            color.g,
            color.b,
            color.a
        };
    }

    public void SetBodyColor(Color color)
    {
        bodyColor = new[] {
            color.r,
            color.g,
            color.b,
            color.a
        };
    }

    public void SetLegsColor(Color color)
    {
        legsColor = new[] {
            color.r,
            color.g,
            color.b,
            color.a
        };
    }

    public Color GetHeadColor()
    {
        return new Color(
            headColor[0],
            headColor[1],
            headColor[2],
            headColor[3]);
    }

    public Color GetBodyColor()
    {
        return new Color(
            bodyColor[0],
            bodyColor[1],
            bodyColor[2],
            bodyColor[3]);
    }

    public Color GetLegsColor()
    {
        return new Color(
            legsColor[0],
            legsColor[1],
            legsColor[2],
            legsColor[3]);
    }
}

[Serializable]
public class Character : Person
{
    public string Birthday;
    
    public Character()
    {
    }

    public void SetupEvents()
    {
        TimeManager.PerDayEvent.RemoveListener(CheckBirthday);

        TimeManager.PerDayEvent.AddListener(CheckBirthday);
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
            InformationPanelManager.Instance.DisplayMessage("Happy Birthday!", 5.0f);
        }
    }
}
