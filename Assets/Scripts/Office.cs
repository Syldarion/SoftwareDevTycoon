using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEditor;
using UnityEngine.Events;

[Serializable]
public class Office
{
    public const int MIN_OFFICE_SPACE = 50;
    public const int MAX_OFFICE_SPACE = 200;
    public const int COST_PER_SPACE = 100;

    //Properties
    public int Space { get { return space; } }
    public int RemainingSpace
    {
        get { return space - Features.Sum(x => x.Size); }
    }
    public int TotalUpkeepCost
    {
        get { return Features.Sum(x => x.UpkeepCost); }
    }
    public int PurchasePrice { get { return Space * COST_PER_SPACE; } }
    public int SellPrice
    {
        get
        {
            //this needs to be better later, take into account new buildings and such
            int used_space = Space - RemainingSpace;
            return used_space * COST_PER_SPACE * 2 + RemainingSpace * COST_PER_SPACE;
        }
    }

    //Public Fields
    public List<OfficeFeature> Features;
    public List<Employee> Employees;
    public Location OfficeLocation;
    public float[] QualityBonuses;
    public float MoraleModifier;
    public float SalesModifier;
    
    //Private Fields
    private int space;
    
    public Office(int officeSpace)
    {
        Features = new List<OfficeFeature>();
        space = Mathf.Clamp(officeSpace, MIN_OFFICE_SPACE, MAX_OFFICE_SPACE);
    }

    public void SetupEvents()
    {

    }

    public void AddOfficeFeature(OfficeFeature feature)
    {
        if(RemainingSpace - feature.Size >= 0)
        {
            Features.Add(feature);
            feature.ApplyBonuses(this);
        }
    }

    /// <summary>
    /// Inncreases the amount of space available for office buildings
    /// </summary>
    /// <param name="extraSpace">The amount of space to add to the office</param>
    /// <returns>The cost of increasing the space</returns>
    public int IncreaseOfficeSpace(int extraSpace)
    {
        if (extraSpace < 0 || space >= MAX_OFFICE_SPACE)
            return 0;

        int old_space = space;
        space = Mathf.Clamp(space + extraSpace, 0, MAX_OFFICE_SPACE);
        return (space - old_space) * COST_PER_SPACE;
    }
}

[Serializable]
public class OfficeFeature
{
    public static OfficeFeature[] AllFeatures = {
        new OfficeFeature("Cubicles", 100, 100)
            .AddBonus(new OfficeMoraleBonus(-0.1f))
            .AddBonus(new OfficeQualityBonus(new[] {0, 1, 2, 3, 4}, 0.05f))
            .AddDescription("Decreases office morale by 10%\nIncreases all project quality factors by 5%"),
        new OfficeFeature("Open Floorplan", 100, 100)
            .AddBonus(new OfficeMoraleBonus(0.1f))
            .AddBonus(new OfficeQualityBonus(new[] {0, 1, 2, 3, 4}, -0.05f))
            .AddDescription("Increases office morale by 10%\nDecreases all project quality factors by 5%"),
        new OfficeFeature("Recreation Area", 100, 500)
            .AddBonus(new OfficeMoraleBonus(0.2f))
            .AddBonus(new OfficeQualityBonus(new[] {0, 1, 2, 3, 4}, -0.05f))
            .AddDescription("Increases office morale by 20%\nDecreases all project quality factors by 5%"),
        new OfficeFeature("Conference Hall", 500, 1000)
            .AddBonus(new OfficeSalesBonus(0.1f))
            .AddBonus(new OfficeQualityBonus(new[] {0, 1, 2, 3, 4}, 0.05f))
            .AddDescription("Increases sales by 10%\nIncreases all project quality factors by 5%"),
        new OfficeFeature("Cafeteria", 500, 500)
            .AddBonus(new OfficeMoraleBonus(0.1f))
            .AddDescription("Increases office morale by 10%"),
        new OfficeFeature("Server Room", 100, 1000)
            .AddBonus(new OfficeQualityBonus(new [] {2, 3}, 0.1f))
            .AddDescription("Increases databases and networking project quality factors by 10%"),
        new OfficeFeature("IT Department", 100, 5000)
            .AddBonus(new OfficeMoraleBonus(0.1f))
            .AddBonus(new OfficeQualityBonus(new [] {0, 1, 2, 3, 4}, 0.25f))
            .AddDescription("Increases office morale by 10%\nIncreases all project quality factors by 25%"), 
    };

    //Properties


    //Public Fields
    public string Name;
    public int Size;
    public int UpkeepCost;
    public List<OfficeBonus> Bonuses;
    public string BonusDescription;

    //Private Fields

    public OfficeFeature(string featureName, int size, int upkeep)
    {
        Name = featureName;
        Size = size;
        UpkeepCost = upkeep;
    }

    public OfficeFeature AddBonus(OfficeBonus bonus)
    {
        Bonuses.Add(bonus);
        return this;
    }

    public OfficeFeature AddDescription(string desc)
    {
        BonusDescription = desc;
        return this;
    }

    public void ApplyBonuses(Office office)
    {
        foreach(OfficeBonus bonus in Bonuses)
            bonus.OnAdd(office);
    }

    public void RemoveBonuses(Office office)
    {
        foreach(OfficeBonus bonus in Bonuses)
            bonus.OnRemove(office);
    }
}

public class OfficeBonus
{
    public UnityAction<Office> OnAdd, OnRemove;

    public OfficeBonus() { }
}

public class OfficeMoraleBonus : OfficeBonus
{
    public OfficeMoraleBonus(float increaseBy)
    {
        OnAdd = x => x.MoraleModifier += increaseBy;
        OnRemove = x => x.MoraleModifier -= increaseBy;
    }
}

public class OfficeQualityBonus : OfficeBonus
{
    public OfficeQualityBonus(IEnumerable<int> qualityIndices, float increaseBy)
    {
        foreach (int i in qualityIndices)
        {
            int i1 = i;
            OnAdd += x => x.QualityBonuses[i1] += increaseBy;
            OnRemove += x => x.QualityBonuses[i1] -= increaseBy;
        }
    }
}

public class OfficeSalesBonus : OfficeBonus
{
    public OfficeSalesBonus(float increaseBy)
    {
        OnAdd = x => x.SalesModifier += increaseBy;
        OnRemove = x => x.SalesModifier -= increaseBy;
    }
}