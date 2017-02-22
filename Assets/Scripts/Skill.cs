using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static bool operator ==(Skill a, Skill b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if ((object)a == null || (object)b == null)
            return false;

        return (a.Name == b.Name) && (a.Code == b.Code);
    }

    public static bool operator !=(Skill a, Skill b)
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