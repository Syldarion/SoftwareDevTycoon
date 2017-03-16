using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class LocationDistance
{
    public Location LocationA;
    public Location LocationB;
    public int Distance;

    public LocationDistance(Location a, Location b, int d)
    {
        LocationA = a;
        LocationB = b;
        Distance = d;
    }
}

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

    public static LocationDistance[] LocationDistances = {
        new LocationDistance(Locations[0], Locations[1], 140),      //Portland -> Seattle 
        new LocationDistance(Locations[0], Locations[2], 4970),     //Portland -> London
        new LocationDistance(Locations[0], Locations[3], 554),      //Portland -> San Jose
        new LocationDistance(Locations[0], Locations[4], 552),      //Portland -> San Francisco
        new LocationDistance(Locations[0], Locations[5], 2398),     //Portland -> Raleigh
        new LocationDistance(Locations[0], Locations[6], 1647),     //Portland -> Madison
        new LocationDistance(Locations[0], Locations[7], 1703),     //Portland -> Austin
        new LocationDistance(Locations[0], Locations[8], 2551),     //Portland -> Boston
        new LocationDistance(Locations[0], Locations[9], 487),      //Portland -> Sacramento
        new LocationDistance(Locations[0], Locations[10], 2192),    //Portland -> Atlanta
        new LocationDistance(Locations[1], Locations[2], 4847),     //Seattle -> London
        new LocationDistance(Locations[1], Locations[3], 691),      //Seattle -> San Jose
        new LocationDistance(Locations[1], Locations[4], 691),      //Seattle -> San Francisco
        new LocationDistance(Locations[1], Locations[5], 2389),     //Seattle -> Raleigh
        new LocationDistance(Locations[1], Locations[6], 1620),     //Seattle -> Madison
        new LocationDistance(Locations[1], Locations[7], 1760),     //Seattle -> Austin
        new LocationDistance(Locations[1], Locations[8], 2505),     //Seattle -> Boston
        new LocationDistance(Locations[1], Locations[9], 623),      //Seattle -> Sacramento
        new LocationDistance(Locations[1], Locations[10], 2201),    //Seattle -> Atlanta
        new LocationDistance(Locations[2], Locations[3], 5409),     //London -> San Jose
        new LocationDistance(Locations[2], Locations[4], 5425),     //London -> San Francisco
        new LocationDistance(Locations[2], Locations[5], 3920),     //London -> Raleigh
        new LocationDistance(Locations[2], Locations[6], 3980),     //London -> Madison
        new LocationDistance(Locations[2], Locations[7], 4494),     //London -> Austin
        new LocationDistance(Locations[2], Locations[8], 3297),     //London -> Boston
        new LocationDistance(Locations[2], Locations[9], 5339),     //London -> Sacramento
        new LocationDistance(Locations[2], Locations[10], 4254),    //London -> Atlanta
        new LocationDistance(Locations[3], Locations[4], 29),       //San Jose -> San Francisco
        new LocationDistance(Locations[3], Locations[5], 2401),     //San Jose -> Raleigh
        new LocationDistance(Locations[3], Locations[6], 1757),     //San Jose -> Madison
        new LocationDistance(Locations[3], Locations[7], 1468),     //San Jose -> Austin
        new LocationDistance(Locations[3], Locations[8], 2692),     //San Jose -> Boston
        new LocationDistance(Locations[3], Locations[9], 72),       //San Jose -> Sacramento
        new LocationDistance(Locations[3], Locations[10], 2126),    //San Jose -> Atlanta
        new LocationDistance(Locations[4], Locations[5], 2401),     //San Francisco -> Raleigh
        new LocationDistance(Locations[4], Locations[6], 1784),     //San Francisco -> Madison
        new LocationDistance(Locations[4], Locations[7], 1497),     //San Francisco -> Austin
        new LocationDistance(Locations[4], Locations[8], 2719),     //San Francisco -> Boston
        new LocationDistance(Locations[4], Locations[9], 86),       //San Francisco -> Sacramento
        new LocationDistance(Locations[4], Locations[10], 2156),    //San Francisco -> Atlanta
        new LocationDistance(Locations[5], Locations[6], 798),      //Raleigh -> Madison
        new LocationDistance(Locations[5], Locations[7], 1163),     //Raleigh -> Austin
        new LocationDistance(Locations[5], Locations[8], 633),      //Raleigh -> Boston
        new LocationDistance(Locations[5], Locations[9], 2368),     //Raleigh -> Sacramento
        new LocationDistance(Locations[5], Locations[10], 357),     //Raleigh -> Atlanta
        new LocationDistance(Locations[6], Locations[7], 1009),     //Madison -> Austin
        new LocationDistance(Locations[6], Locations[8], 935),      //Madison -> Boston
        new LocationDistance(Locations[6], Locations[9], 1710),     //Madison -> Sacramento
        new LocationDistance(Locations[6], Locations[10], 743),     //Madison -> Atlanta
        new LocationDistance(Locations[7], Locations[8], 834),      //Austin -> Boston
        new LocationDistance(Locations[7], Locations[9], 1463),     //Austin -> Sacramento
        new LocationDistance(Locations[7], Locations[10], 813),     //Austin -> Atlanta
        new LocationDistance(Locations[8], Locations[9], 2645),     //Boston -> Sacramento
        new LocationDistance(Locations[8], Locations[10], 957),     //Boston -> Atlanta
        new LocationDistance(Locations[9], Locations[10], 2102),    //Sacramento -> Atlanta
    };

    public static Location GetRandomLocation()
    {
        return Locations.ElementAt(UnityEngine.Random.Range(0, Locations.Count));
    }

    public static int GetDistance(Location a, Location b)
    {
        LocationDistance distance = LocationDistances.FirstOrDefault(x =>
            x.LocationA == a && x.LocationB == b || x.LocationA == b && x.LocationB == a);
        return distance == null ? 0 : distance.Distance;
    }
}
