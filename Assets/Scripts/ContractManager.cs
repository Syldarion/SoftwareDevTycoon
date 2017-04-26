using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ContractManager : Singleton<ContractManager>
{
    public CanvasGroup ContractWorkPanel;
    public RectTransform ContractList;
    public ContractObject ContractListObjectPrefab;

    public Text ContractName;
    public Text WorkRequiredPRG;
    public Text WorkRequiredUIX;
    public Text WorkRequiredDBS;
    public Text WorkRequiredNTW;
    public Text WorkRequiredWEB;
    public InputField ContractPay;
    public InputField ContractDays;
    public Button NegotiateButton;
    public Button AcceptButton;

    private bool onCooldown;

    void Awake()
    {
        Instance = this;
        onCooldown = false;
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseContractForm();
    }

    public void OpenContractForm()
    {
        if (onCooldown) return;

        foreach (Transform child in ContractList)
            Destroy(child.gameObject);
        Contract[] generated_contracts = Contract.GenerateContracts(10);
        foreach (Contract contract in generated_contracts)
        {
            ContractObject new_contract_object = 
                Instantiate(ContractListObjectPrefab);

            new_contract_object.PopulateContractInfo(contract);
            new_contract_object.transform.SetParent(ContractList, false);
        }

        PopulateContractDetail(generated_contracts[0]);
        SDTUIController.Instance.OpenCanvas(ContractWorkPanel);
    }

    public void CloseContractForm()
    {
        SDTUIController.Instance.CloseCanvas(ContractWorkPanel);

        StartCoroutine(LockCooldown());
    }

    public void CancelActiveContract()
    {
        if (Character.MyCharacter.ActiveContract != null)
            Character.MyCharacter.ActiveContract.CancelContract();
        if (Company.MyCompany.ActiveContract != null)
            Company.MyCompany.ActiveContract.CancelContract();
    }

    public void PopulateContractDetail(Contract contract)
    {
        ContractName.text = contract.Name;

        WorkRequiredPRG.text = contract.SkillPointsRemaining[Skill.Programming].Level.ToString();
        WorkRequiredUIX.text = contract.SkillPointsRemaining[Skill.UserInterfaces].Level.ToString();
        WorkRequiredDBS.text = contract.SkillPointsRemaining[Skill.Databases].Level.ToString();
        WorkRequiredNTW.text = contract.SkillPointsRemaining[Skill.Networking].Level.ToString();
        WorkRequiredWEB.text = contract.SkillPointsRemaining[Skill.WebDevelopment].Level.ToString();

        ContractPay.text = contract.Payment.ToString();
        ContractDays.text = contract.DaysToComplete.ToString();

        NegotiateButton.onClick.RemoveAllListeners();
        AcceptButton.onClick.RemoveAllListeners();

        if (!contract.Negotiated)
        {
            NegotiateButton.onClick.AddListener(() =>
            {
                int new_p, new_d;
                if (!int.TryParse(ContractPay.text, out new_p)) new_p = contract.Payment;
                if (!int.TryParse(ContractDays.text, out new_d)) new_d = contract.DaysToComplete;
                TryNegotiate(contract, new_p, new_d);
                PopulateContractDetail(contract);
            });
        }
        else
        {
            NegotiateButton.GetComponentInChildren<Text>().text = contract.SuccessfulNegotiation 
                ? "Success" : "Fail";
            NegotiateButton.image.color = contract.SuccessfulNegotiation 
                ? Color.green : Color.red;
            NegotiateButton.interactable = false;
        }

        AcceptButton.onClick.AddListener(() =>
        {
            contract.AcceptContract();
            CloseContractForm();
        });
    }

    public void TryNegotiate(Contract contract, int newPay, int newDays)
    {
        int acceptance_chance = GetNegotiationAcceptanceChance(contract, newPay, newDays);
        int roll = Random.Range(0, 100) + 1;
        bool success = roll <= acceptance_chance;
        contract.Negotiated = true;
        contract.SuccessfulNegotiation = false;

        if (success)
        {
            contract.Payment = newPay;
            contract.DaysToComplete = newDays;
            contract.SuccessfulNegotiation = true;

            PopulateContractDetail(contract);
        }
    }

    public int GetNegotiationAcceptanceChance(Contract contract, int newPay, int newDays)
    {
        int reputation = Company.MyCompany == null
            ? Character.MyCharacter.Reputation
            : Company.MyCompany.Reputation;

        float new_pay_percentage = (float)newPay / contract.Payment;
        float new_day_percentage = (float)newDays / contract.DaysToComplete;

        float pay_percentage_change = new_pay_percentage - 1.0f;
        float day_percentage_change = new_day_percentage - 1.0f;

        if (pay_percentage_change > 0.0f) pay_percentage_change *= (0.5f + reputation / 100.0f);
        else pay_percentage_change *= (1.5f - reputation / 100.0f);
        if (day_percentage_change > 0.0f) day_percentage_change *= (0.5f + reputation / 100.0f);
        else day_percentage_change *= (1.5f - reputation / 100.0f);

        float weighted_value = (pay_percentage_change * 0.75f) + (day_percentage_change * 0.25f);

        return Mathf.CeilToInt(Mathf.Clamp(100.0f - (weighted_value * 100.0f), 0.0f, 100.0f));
    }

    private IEnumerator LockCooldown()
    {
        onCooldown = true;
        DateTime week_from_now = TimeManager.CurrentDate.AddDays(7.0);
        while (TimeManager.CurrentDate < week_from_now)
            yield return null;
        onCooldown = false;
    }
}
