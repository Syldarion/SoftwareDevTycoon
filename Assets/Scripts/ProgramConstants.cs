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

    public static string GetRandomName()
    {
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
    public static string WelcomeMessage =
        "Welcome to Software Dev Tycoon, {0}! " +
        "You are the founder of a fledgling software company. " +
        "Uh. Make some software I guess. I'll try to help you out at first.";
}
