using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class ColorBlocks
{
    private static float highlight_mult = 245.0f / 255.0f;
    private static float pressed_mult = 200.0f / 255.0f;

    public static ColorBlock GetColorBlock(Color color)
    {
        var block = new ColorBlock()
        {
            colorMultiplier = 1,
            disabledColor = new Color(200.0f, 200.0f, 200.0f, 128.0f) / 255.0f,
            fadeDuration = 0.1f,
            highlightedColor = color * highlight_mult,
            normalColor = color,
            pressedColor = color * pressed_mult
        };

        return block;
    }
}

public static class PersonNames
{
    public static string[] FirstNames = {
        "Noah", "Liam", "Mason",
        "Jacob", "William", "Ethan",
        "James", "Alexander", "Michael",
        "Benjamin", "Emma", "Olivia",
        "Sophia", "Ava", "Isabella",
        "Mia", "Abigail", "Emily",
        "Charlotte", "Harper"
    };

    public static string[] LastNames = {
        "Smith", "Jones", "Brown",
        "Johnson", "Williams", "Miller",
        "Taylor", "Wilson", "Davis",
        "White", "Clark", "Hall",
        "Thomas", "Thompson", "Moore",
        "Hill", "Walker", "Anderson",
        "Wright", "Martin", "Wood",
        "Allen", "Robinson", "Lewis",
        "Scott"
    };

    public static string[] FullNames =
    {
        "Caleb Makela",
        "Jacob Asmuth",
        "Cris Laney",
        "Curtis Rice",
        "Alex Parsley",
        "Ashley Wagner",
        "Katie Hughes",
        "Zachary Wentworth",
        "Eliza Henderson",
        "Steven Alspach",
        "Todd Breedlove",
        "Troy Scevers",
        "Calvin Caldwell",
        "Devon Andrade",
        "Ryan Williams"
    };

    public static string GetRandomName()
    {
        if (Random.value < 0.05f)
            return FullNames[Random.Range(0, FullNames.Length)];

        return string.Format("{0} {1}", 
            FirstNames[Random.Range(0, FirstNames.Length)], 
            LastNames[Random.Range(0, LastNames.Length)]);
    }
}

public static class CompanyNames
{
    public static string[] Names = {
        "Wintel",
        "Advanced Macro Devices",
        "OUTVidia",
        "Goggle",
        "Whamazon",
        "Circle",
        "BMWare",
        "Macrohard",
        "Pear",
        "Facespace"
    };

    public static string GetRandomName()
    {
        return Names[Random.Range(0, Names.Length)];
    }
}

public static class DialogueMessage
{
    public static string Welcome =
        "Welcome to Software Dev Tycoon, {0}! " +
        "You are the founder of a fledgling software company. " +
        "Uh. Make some software I guess. I'll try to help you out at first.";
    public static string GettingStartedContract =
        "To get started earning money, open the side panel and get yourself " +
        "a new contract! You apply your skill level to the work needed for a contract " +
        "daily, so make sure you have what it takes to complete it before accepting one.";
    public static string FirstContractStarted =
        "You've just started your first contract! Your skills will be applied to it " +
        "daily until time runs out or all work is complete. If all work is completed, you " +
        "will be awarded with the contract payment.";
    public static string FirstContractCompleted =
        "You've just completed your first contract! Reap in the benefits, blah blah blah. " +
        "Consider checking out your company features, such as employees and offices, WOW.";
    public static string MonthlyReport =
        "Report: {0}\n" +
        "Earnings: {1:C0}\n" +
        "Contracts: {2:C0}\n" +
        "Projects: {3:C0}\n" +
        "\n" +
        "Costs: {4:C0}\n" +
        "New Hires: {5:C0}\n" +
        "Employee Pay: {6:C0}\n" +
        "Office Upkeep: {7:C0}\n" +
        "\n" +
        "Total: {8:C0}";
}
