using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueBox : Singleton<DialogueBox>
{
    public Text MessageText;

    public GameObject OptionsPanelTemplate;
    public GameObject OptionButtonTemplate;
    public GameObject InputFieldTemplate;

    private List<Button> optionButtons;
    private List<InputField> inputFields;

    private GameObject currentOptionsPanel;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;

        optionButtons = new List<Button>();
        inputFields = new List<InputField>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void CreateNewDialogue(string message)
    {
        Cleanup();

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        MessageText.text = message;
    }

    public Button AddButton(string text, UnityAction action)
    {
        if (optionButtons.Count % 3 == 0)
        {
            currentOptionsPanel = Instantiate(OptionsPanelTemplate);
            currentOptionsPanel.gameObject.SetActive(true);
            currentOptionsPanel.transform.SetParent(transform, false);
        }

        Button new_button = Instantiate(OptionButtonTemplate).GetComponent<Button>();

        new_button.gameObject.SetActive(true);

        new_button.onClick.AddListener(action);
        new_button.transform.SetParent(currentOptionsPanel.transform, false);
        new_button.GetComponentInChildren<Text>().text = text;

        optionButtons.Add(new_button);

        return new_button;
    }

    public InputField AddInputField(string placeholder)
    {
        GameObject new_field_panel = Instantiate(InputFieldTemplate);
        new_field_panel.gameObject.SetActive(true);
        new_field_panel.transform.SetParent(transform, false);

        InputField new_field = new_field_panel.GetComponentInChildren<InputField>();
        new_field.transform.GetChild(0).GetComponent<Text>().text = placeholder;

        inputFields.Add(new_field);

        return new_field;
    }

    public void Cleanup()
    {
        foreach (Transform child in transform)
            if (child.name != "DialogueMessage")
                Destroy(child.gameObject);

        optionButtons.Clear();
        inputFields.Clear();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void CreateYesNoDialogue(string message, UnityAction yesAction, UnityAction noAction)
    {
        CreateNewDialogue(message);
        AddButton("Yes", yesAction);
        AddButton("No", noAction);
    }
}