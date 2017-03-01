using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanelManager : Singleton<InformationPanelManager>
{
    [Header("Active Contract UI")]
    public RectTransform ContractPanel;
    public Text ContractNameText;
    public Text ContractPayText;
    public Text ContractDeadlineText;
    public ProgressBar ContractProgressBar;

    [Header("Message UI")]
    public RectTransform MessagePanel;
    public GameObject MessageTemplatePrefab;

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

    public void UpdateActiveContract()
    {
        Contract active_contract = Company.MyCompany == null 
            ? Character.MyCharacter.ActiveContract
            : Company.MyCompany.ActiveContract;
        if (active_contract == null)
        {
            HideActiveContract();
            return;
        }

        ContractNameText.text = active_contract.Name;
        ContractPayText.text = active_contract.Payment.ToString();
        ContractDeadlineText.text = active_contract.DaysRemaining.ToString();
        ContractProgressBar.SetProgress(active_contract.Progress);
    }

    public void ShowActiveContract()
    {
        ContractPanel.gameObject.SetActive(true);
    }

    public void HideActiveContract()
    {
        ContractPanel.gameObject.SetActive(false);
    }

    public void UpdateCompanyInformation()
    {
        
    }

    public void ShowCompanyInformation()
    {
        
    }

    public void HideCompanyInformation()
    {
        
    }

    public void DisplayMessage(string message, float duration)
    {
        GameObject new_message = Instantiate(MessageTemplatePrefab);
        new_message.transform.SetParent(MessagePanel, false);
        new_message.SetActive(true);
        Text message_text = new_message.GetComponentInChildren<Text>();
        if (message_text != null)
            message_text.text = message;
        Destroy(new_message, duration);
    }
}