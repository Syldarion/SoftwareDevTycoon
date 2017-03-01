using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public enum Skill
{
    Programming,
    UserInterfaces,
    Databases,
    Networking,
    WebDevelopment,
    Count
}

public class SkillInfo
{
    public static readonly string[] SKILL_NAME = {
        "Programming",
        "User Interfaces",
        "Databases",
        "Networking",
        "Web Development"
    };
    public static readonly string[] SKILL_ABBR = {
        "PRG",
        "UIX",
        "DBS",
        "NTW",
        "WEB"
    };
    public static readonly int COUNT = (int)Skill.Count;
}

[Serializable]
public class SkillList
{
    public bool IsEmpty { get { return !allSkills.Any(x => x.Level > 0); } }
    public int Length { get { return allSkills.Count; } }
    public IEnumerable<SkillLevel> Skills { get { return allSkills; } }

    private readonly List<SkillLevel> allSkills;

    public SkillList()
    {
        allSkills = new List<SkillLevel>();
        for(int i = 0; i < SkillInfo.COUNT; i++)
            allSkills.Add(new SkillLevel((Skill)i, 0));
    }

    public SkillList(IEnumerable<SkillLevel> skills)
    {
        allSkills = new List<SkillLevel>();
        foreach(SkillLevel skill in skills)
        {
            int found_index = allSkills.FindIndex(x => x == skill);
            if (found_index != -1)
                allSkills[found_index] += skill;
            else
                allSkills.Add(skill);
        }
    }

    public SkillList AddSkill(SkillLevel skill)
    {
        int found_index = allSkills.FindIndex(x => x == skill);
        if(found_index != -1)
            allSkills[found_index] += skill;
        else
            allSkills.Add(skill);
        return this;
    }

    [NotNull]
    public SkillLevel this[int index]
    {
        get
        {
            if(index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException("index");

            return allSkills[index];
        }
        set
        {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException("index");
            if (value == null) throw new ArgumentNullException("value");

            allSkills[index] = value;
        }
    }

    [NotNull]
    public SkillLevel this[Skill skill]
    {
        get { return allSkills.Find(x => x.Skill == skill); }
        set
        {
            if(value == null) throw new ArgumentNullException("value");
            int found_index = allSkills.FindIndex(x => x.Skill == skill);
            if(found_index != -1)
                allSkills[found_index] = value;
            else
                allSkills.Add(value);
        }
    }

    public int Sum()
    {
        return allSkills.Sum(x => x.Level);
    }

    public bool Contains(Skill skill)
    {
        return allSkills.Exists(x => x.Skill == skill);
    }

    public static SkillList operator+(SkillList left, SkillList right)
    {
        List<SkillLevel> new_skill_list = left.allSkills;
        foreach(SkillLevel skill in right.allSkills)
        {
            int found_index = new_skill_list.FindIndex(x => x == skill);
            if(found_index != -1)
                new_skill_list[found_index] += skill;
            else
                new_skill_list.Add(skill);
        }
        new_skill_list.RemoveAll(x => x.Level <= 0);

        return new SkillList(new_skill_list);
    }

    public static SkillList operator-(SkillList left, SkillList right)
    {
        List<SkillLevel> new_skill_list = left.allSkills;
        foreach(SkillLevel skill in right.allSkills)
        {
            int found_index = new_skill_list.FindIndex(x => x == skill);
            if (found_index != -1)
                new_skill_list[found_index] -= skill;
        }
        new_skill_list.RemoveAll(x => x.Level <= 0);

        return new SkillList(new_skill_list);
    }
}

[Serializable]
public class SkillLevel
{
    public Skill Skill;
    public int Level;

    public SkillLevel(Skill skill, int level)
    {
        Skill = skill;
        Level = level;
    }

    public static SkillLevel operator+(SkillLevel left, SkillLevel right)
    {
        return left.Skill != right.Skill 
            ? left 
            : new SkillLevel(left.Skill, Mathf.Clamp(left.Level + right.Level, 0, int.MaxValue));
    }

    public static SkillLevel operator+(SkillLevel skill, int value)
    {
        return new SkillLevel(skill.Skill, skill.Level + value);
    }

    public static SkillLevel operator-(SkillLevel left, SkillLevel right)
    {
        return left.Skill != right.Skill
            ? left
            : new SkillLevel(left.Skill, Mathf.Clamp(left.Level - right.Level, 0, int.MaxValue));
    }

    public static SkillLevel operator-(SkillLevel skill, int value)
    {
        return new SkillLevel(skill.Skill, skill.Level + value);
    }

    public static bool operator>(SkillLevel left, SkillLevel right)
    {
        return left.Compare(right) > 0;
    }

    public static bool operator>=(SkillLevel left, SkillLevel right)
    {
        return left.Compare(right) >= 0;
    }

    public static bool operator<(SkillLevel left, SkillLevel right)
    {
        return left.Compare(right) < 0;
    }

    public static bool operator<=(SkillLevel left, SkillLevel right)
    {
        return left.Compare(right) <= 0;
    }

    public static bool operator==(SkillLevel left, SkillLevel right)
    {
        object left_obj = left;
        object right_obj = right;
        if(left_obj == null && right_obj == null)
            return true;
        if(left_obj == null || right_obj == null)
            return false;
        return left.Skill == right.Skill;
    }

    public static bool operator!=(SkillLevel left, SkillLevel right)
    {
        return !(left == right);
    }

    public static bool operator >(SkillLevel left, int right)
    {
        return left.Level > right;
    }

    public static bool operator >=(SkillLevel left, int right)
    {
        return left.Level >= right;
    }

    public static bool operator <(SkillLevel left, int right)
    {
        return left.Level < right;
    }

    public static bool operator <=(SkillLevel left, int right)
    {
        return left.Level <= right;
    }

    public static bool operator ==(SkillLevel left, int right)
    {
        return !(left == null) && left.Level == right;
    }

    public static bool operator !=(SkillLevel left, int right)
    {
        return !(left == null) && left.Level != right;
    }

    private int Compare(SkillLevel other)
    {
        return Level.CompareTo(other.Level);
    }

    protected bool Equals(SkillLevel other)
    {
        return Skill == other.Skill && Level == other.Level;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((SkillLevel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)Skill * 397) ^ Level;
        }
    }
}