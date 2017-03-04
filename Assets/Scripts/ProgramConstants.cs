using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class ServerMessageCodes
{
    //Login Service Codes
    public const int ACCOUNT_NAME_TAKEN = 5001;
    public const int NO_USER = 5002;
    public const int INCORRECT_PASS = 5003;

    public static int GetServerMessageCode(string serverResponse)
    {
        if (serverResponse.Contains(":"))
            return int.Parse(serverResponse.Split(':')[0]);
        return -1;
    }
}

public static class ColorBlocks
{
    private static float highlightMult = 245.0f / 255.0f;
    private static float pressedMult = 200.0f / 255.0f;

    public static ColorBlock GetColorBlock(Color color)
    {
        ColorBlock block = new ColorBlock()
        {
            colorMultiplier = 1,
            disabledColor = new Color(200.0f, 200.0f, 200.0f, 128.0f) / 255.0f,
            fadeDuration = 0.1f,
            highlightedColor = color * highlightMult,
            normalColor = color,
            pressedColor = color * pressedMult
        };

        return block;
    }
}

public static class ControlKeys
{
    public struct ControlKey
    {
        public KeyCode Key;
        public KeyCode Modifier;
    }

    public static ControlKey OPEN_CONTRACT_PANEL = new ControlKey() {Key = KeyCode.C, Modifier = KeyCode.LeftShift};
    public static ControlKey OPEN_JOB_PANEL = new ControlKey() {Key = KeyCode.J, Modifier = KeyCode.LeftShift};
    public static ControlKey PRINT_JOB_DEBUG = new ControlKey() {Key = KeyCode.J, Modifier = KeyCode.LeftControl};
    public static ControlKey TIME_PAUSE_TOGGLE = new ControlKey() {Key = KeyCode.Space, Modifier = KeyCode.None};
    public static ControlKey OPEN_SCHEDULE_PANEL = new ControlKey() {Key = KeyCode.S, Modifier = KeyCode.LeftShift};

    public static bool GetControlKey(ControlKey control)
    {
        if (control.Modifier == KeyCode.None)
            return Input.GetKey(control.Key);

        return Input.GetKey(control.Modifier) && Input.GetKey(control.Key);
    }

    public static bool GetControlKeyDown(ControlKey control)
    {
        if (control.Modifier == KeyCode.None)
            return Input.GetKeyDown(control.Key);

        return Input.GetKey(control.Modifier) && Input.GetKeyDown(control.Key);
    }

    public static bool GetControlKeyUp(ControlKey control)
    {
        if (control.Modifier == KeyCode.None)
            return Input.GetKeyUp(control.Key);

        return Input.GetKey(control.Modifier) && Input.GetKeyUp(control.Key);
    }
}

public static class PrebuiltProjectTasks
{
    public struct TaskNameWorkPair
    {
        public string Name;
        public int[] Work;
    }

    //prebuilt tasks will be used in the GenerateTask function of ProjectTask
    //WorkNeeded values are provided in a per-day way here, to be multiplied in the function
    public static TaskNameWorkPair[] AllTasks = 
    {
        //Programming Tasks
        new TaskNameWorkPair() {Name = "Design primary feature", Work = new[] {3, 0, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Design secondary feature", Work = new[] {2, 0, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Design extra feature", Work = new[] {1, 0, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Create primary feature", Work = new[] {6, 0, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Create secondary feature", Work = new[] {4, 0, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Create extra feature", Work = new[] {2, 0, 0, 0, 0}},
        //User Interfaces Tasks
        new TaskNameWorkPair() {Name = "Design UI layout", Work = new[] {0, 4, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Implement UI layout", Work = new[] {0, 2, 0, 0, 0}},
        new TaskNameWorkPair() {Name = "Create UI unit tests", Work = new[] {0, 3, 0, 0, 0}},
        //Databases Tasks
        new TaskNameWorkPair() {Name = "Design database schema", Work = new [] {0, 0, 5, 0, 0} },
        new TaskNameWorkPair() {Name = "Create database table", Work = new [] {0, 0, 3, 0, 0} },
        //Networking Tasks
        new TaskNameWorkPair() {Name = "Create load balancer", Work = new [] {0, 0, 0, 5, 0} },
        new TaskNameWorkPair() {Name = "Setup DNS server", Work = new [] {0, 0, 0, 4, 0} },
        //Web Development Tasks
        new TaskNameWorkPair() {Name = "Design page layout", Work = new [] {0, 0, 0, 0, 2} },
        new TaskNameWorkPair() {Name = "Create web server", Work = new [] {0, 0, 0, 0, 4} },
        new TaskNameWorkPair() {Name = "Create web page", Work = new [] {0, 0, 0, 0, 3} },

        //Mixed Tasks (these don't exist for now)
    };
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
