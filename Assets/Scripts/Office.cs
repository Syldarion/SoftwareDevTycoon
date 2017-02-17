using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        get { return space - Buildings.Sum(x => x.Size); }
    }
    public int TotalUpkeepCost
    {
        get { return Buildings.Sum(x => x.UpkeepCost); }
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
    public List<OfficeBuilding> Buildings;
    public Location OfficeLocation;
    
    //Private Fields
    private int space;
    
    public Office(int officeSpace)
    {
        Buildings = new List<OfficeBuilding>();
        space = Mathf.Clamp(officeSpace, MIN_OFFICE_SPACE, MAX_OFFICE_SPACE);
    }

    public void SetupEvents()
    {

    }

    public void AddOfficeBuilding(OfficeBuilding building)
    {
        if (RemainingSpace - building.Size >= 0)
            Buildings.Add(building);
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
public class OfficeBuilding
{
    //Properties


    //Public Fields
    public int Size;
    public int UpkeepCost;

    //Private Fields

    public OfficeBuilding(int size, int upkeep)
    {
        Size = size;
        UpkeepCost = upkeep;
    }

    public virtual void ApplyBonus(Office office) { }

    //Getters / Setters
}

public class MainBuilding : OfficeBuilding
{
    public MainBuilding()
        : base(10, 100)
    {
        
    }

    public override void ApplyBonus(Office office)
    {
        
    }
}

[Serializable]
public class CafeteriaBuilding : OfficeBuilding
{
    public CafeteriaBuilding()
        : base(10, 100)
    {
        
    }

    public override void ApplyBonus(Office office)
    {
        
    }
}

[Serializable]
public class ConferenceBuilding : OfficeBuilding
{
    public ConferenceBuilding() 
        : base(10, 100)
    {
    }

    public override void ApplyBonus(Office office)
    {

    }
}

[Serializable]
public class DevelopmentBuilding : OfficeBuilding
{
    public DevelopmentBuilding()
        : base(10, 100)
    {
    }

    public override void ApplyBonus(Office office)
    {

    }
}

[Serializable]
public class RecreationBuilding : OfficeBuilding
{
    public RecreationBuilding()
        : base(10, 100)
    {
    }

    public override void ApplyBonus(Office office)
    {

    }
}