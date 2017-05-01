using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : Singleton<DialogueManager>
{
    public RectTransform DialoguePanel;

    public DialogueBox MessageDialoguePrefab;
    public DialogueBox YesNoDialoguePrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void CreateMessageDialogue(string message)
    {
        DialogueBox new_dialogue = Instantiate(MessageDialoguePrefab);
        new_dialogue.transform.SetParent(DialoguePanel, false);

        new_dialogue.MessageText.text = message;

        new_dialogue.ActionButtons[0].onClick.AddListener(new_dialogue.CloseDialogueBox);
    }

    public void CreateYesNoDialogue(string message, UnityAction yesAction, UnityAction noAction)
    {
        DialogueBox new_dialogue = Instantiate(YesNoDialoguePrefab);
        new_dialogue.transform.SetParent(DialoguePanel, false);

        new_dialogue.MessageText.text = message;

        new_dialogue.ActionButtons[0].onClick.AddListener(noAction);
        new_dialogue.ActionButtons[1].onClick.AddListener(yesAction);

        new_dialogue.ActionButtons[0].onClick.AddListener(new_dialogue.CloseDialogueBox);
        new_dialogue.ActionButtons[1].onClick.AddListener(new_dialogue.CloseDialogueBox);
    }
}
