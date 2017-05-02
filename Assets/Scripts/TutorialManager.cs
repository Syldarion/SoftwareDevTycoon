using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    public bool FirstContractStartedMessageFired;
    public bool FirstContractFinishedMessageFired;

    void Awake()
    {
        Instance = this;

        FirstContractStartedMessageFired = false;
        FirstContractFinishedMessageFired = false;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void BeginTutorial()
    {
        DialogueManager.Instance.CreateMessageDialogue(
            string.Format(DialogueMessage.Welcome, GameManager.ActiveCharacter.Name),
            Vector3.zero,
            () =>
            {
                DialogueManager.Instance.CreateMessageDialogue(
                    DialogueMessage.GettingStartedContract,
                    new Vector3(-300.0f, 0.0f),
                    () => { });
            });
    }
}
