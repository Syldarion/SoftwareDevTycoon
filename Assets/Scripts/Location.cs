using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Location
{
    public string Name { get; private set; }
    public float PriceMultiplier { get; private set; }

    public string State()
    {
        return Name.Split(',')[1];
    }

    public Location(string name, float priceMult)
    {
        Name = name;
        PriceMultiplier = priceMult;
    }

    /// <summary>
    /// Comparison function for sorting locations alphabetically, by state first
    /// </summary>
    /// <param name="a">First location</param>
    /// <param name="b">Second location</param>
    /// <returns>-1 if a is less than b, 0 if a is equal to b, 1 if a is greater than b</returns>
    public static int AlphaSortFunction(Location a, Location b)
    {
        return string.Compare(a.State(), b.State(), StringComparison.Ordinal) == 0
                   ? string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                   : string.Compare(a.State(), b.State(), StringComparison.Ordinal);
    }

    public static List<Location> Locations = new List<Location>(){
        new Location("Portland, OR", 1.0f),
        new Location("Seattle, WA", 1.0f),
        new Location("London, UK", 1.0f),
        new Location("San Jose, CA", 1.0f),
        new Location("San Francisco, CA", 1.0f),
        new Location("Raleigh, NC", 1.0f),
        new Location("Madison, WI", 1.0f),
        new Location("Austin, TX", 1.0f),
        new Location("Boston, MA", 1.0f),
        new Location("Sacramento, CA", 1.0f),
        new Location("Atlanta, GA", 1.0f)
    };

    public static Location GetRandomLocation()
    {
        return Locations.ElementAt(UnityEngine.Random.Range(0, Locations.Count));
    }
}