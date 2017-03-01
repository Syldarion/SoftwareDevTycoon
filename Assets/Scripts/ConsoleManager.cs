using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConsoleCommand
{
    public static ConsoleCommand[] Library = {
        new ConsoleCommand("setmoney", "usage: setmoney {value}", "Sucessfully set money!", 1, x =>
        {
            int value = Convert.ToInt32(x[0]);
            if(Company.MyCompany == null)
                Character.MyCharacter.AdjustMoney(value - Character.MyCharacter.Money);
            else
                Company.MyCompany.AdjustFunds(value - Company.MyCompany.Funds);
        }),
        new ConsoleCommand("setrep", "usage: setrep {value}", "Successfully set reputation!", 1, x =>
        {
            int value = Convert.ToInt32(x[0]);
            if(Company.MyCompany == null)
                Character.MyCharacter.AdjustReputation(value - Character.MyCharacter.Reputation);
            else 
                Company.MyCompany.AdjustReputation(value - Company.MyCompany.Reputation);
        }),
    };

    public string CommandName;
    public string UsageText;
    public string ExecutedText;
    public UnityAction<object[]> OnExecute;
    public int ArgumentCount { get; private set; }

    public ConsoleCommand(string name, string usage, string executed, int argc, UnityAction<object[]> action)
    {
        CommandName = name;
        UsageText = usage;
        ExecutedText = executed;
        OnExecute = action;
        ArgumentCount = argc;
    }

    public void Execute(object[] args)
    {
        object[] new_args = new object[ArgumentCount];
        Array.Copy(args, new_args, ArgumentCount);
        OnExecute.Invoke(new_args);
    }

    public static ConsoleCommand GetCommand(string commandName)
    {
        return Library.FirstOrDefault(command => command.CommandName == commandName);
    }
}

public class ConsoleManager : MonoBehaviour
{
    public CanvasGroup ConsoleCanvas;
    public RectTransform ConsoleMessageList;
    public Text ConsoleMessageTemplate;
    public InputField ConsoleInput;

    void Start() {}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            SDTUIController.Instance.OpenCanvas(ConsoleCanvas);
            ConsoleInput.Select();
            ConsoleInput.ActivateInputField();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SDTUIController.Instance.CloseCanvas(ConsoleCanvas);
        }
    }

    public void ParseInput(string input)
    {
        ConsoleInput.text = string.Empty;
        string[] split_input = input.Split(' ');
        ConsoleCommand command = ConsoleCommand.GetCommand(split_input[0]);
        if(command == null)
        {
            OutputMessage(string.Format("Could not find {0} in the command library!", split_input[0]));
            return;
        }
        if(split_input.Length < command.ArgumentCount - 1)
        {
            OutputMessage(string.Format("Insufficient args for {0}!\n{1}", command.CommandName, command.UsageText));
            return;
        }

        object[] arg_array = new object[split_input.Length - 1];
        Array.Copy(split_input, 1, arg_array, 0, arg_array.Length);

        command.Execute(arg_array);
    }

    public void OutputMessage(string message)
    {
        GameObject new_message = Instantiate(ConsoleMessageTemplate).gameObject;
        new_message.SetActive(true);
        new_message.transform.SetParent(ConsoleMessageList.transform, false);
        new_message.GetComponentInChildren<Text>().text = message;
    }
}
