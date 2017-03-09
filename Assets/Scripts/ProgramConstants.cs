using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    private static float HIGHLIGHT_MULT = 245.0f / 255.0f;
    private static float PRESSED_MULT = 200.0f / 255.0f;

    public static ColorBlock GetColorBlock(Color color)
    {
        var block = new ColorBlock()
        {
            colorMultiplier = 1,
            disabledColor = new Color(200.0f, 200.0f, 200.0f, 128.0f) / 255.0f,
            fadeDuration = 0.1f,
            highlightedColor = color * HIGHLIGHT_MULT,
            normalColor = color,
            pressedColor = color * PRESSED_MULT
        };

        return block;
    }
}

public static class SDTControls
{
    public struct ControlKey
    {
        public KeyCode Key;
        public KeyCode Modifier;

        public ControlKey(KeyCode k, KeyCode m)
        {
            Key = k;
            Modifier = m;
        }
    }

    private static Dictionary<string, ControlKey> CONTROLS = new Dictionary<string, ControlKey>()
    {
        {"OpenContractPanel", new ControlKey(KeyCode.C, KeyCode.LeftShift) },
        {"OpenJobPanel", new ControlKey(KeyCode.J, KeyCode.LeftShift) },
        {"PauseToggle", new ControlKey(KeyCode.Space, KeyCode.None) },
    };

    public static ControlKey OPEN_CONTRACT_PANEL
    {
        get { return CONTROLS["OpenContractPanel"]; }
        set { CONTROLS["OpenContractPanel"] = value; }
    }

    public static ControlKey OPEN_JOB_PANEL
    {
        get { return CONTROLS["OpenJobPanel"]; }
        set { CONTROLS["OpenJobPanel"] = value; }
    }

    public static ControlKey PAUSE_TOGGLE
    {
        get { return CONTROLS["PauseToggle"]; }
        set { CONTROLS["PauseToggle"] = value; }
    }

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

    public static void LoadControlKeys()
    {
        foreach(string value in CONTROLS.Keys)
        {
            var control = new ControlKey(
                (KeyCode)PlayerPrefs.GetInt(string.Format("{0}Key", value)),
                (KeyCode)PlayerPrefs.GetInt(string.Format("{0}Mod", value)));
            CONTROLS[value] = control;
        }
    }

    public static void SaveControlKeys()
    {
        foreach(string value in CONTROLS.Keys)
        {
            PlayerPrefs.SetInt(string.Format("{0}Key", value), (int)CONTROLS[value].Key);
            PlayerPrefs.SetInt(string.Format("{0}Mod", value), (int)CONTROLS[value].Modifier);
        }
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
