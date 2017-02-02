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
    public GameObject ContractObjectPrefab;

    public Text ActiveContractNameText;
    public Text ActiveContractPayText;
    public Text ActiveContractDeadlineText;
    public ProgressBar ActiveContractProgressBar;

    public Contract ActiveContract { get; private set; }

    private bool open;
    private bool locked;

    void Awake()
    {
        Instance = this;
        open = false;
        locked = false;
    }

    void Start()
    {
    }

    void Update()
    {
        if (ControlKeys.GetControlKeyDown(ControlKeys.OPEN_CONTRACT_PANEL))
        {
            if (!open)
                OpenContractForm();
            else
                CloseContractForm();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && open)
            CloseContractForm();
    }

    public void OpenContractForm()
    {
        if (locked) return;

        UIUtilities.ActivateWithLock(ContractWorkPanel, ref open);

        foreach (Contract contract in Contract.GenerateContracts())
        {
            ContractObject new_contract_object = Instantiate(ContractObjectPrefab).GetComponent<ContractObject>();

            new_contract_object.PopulateContractInfo(contract);
            new_contract_object.transform.SetParent(ContractWorkPanel.transform, false);
        }
    }

    public void CloseContractForm()
    {
        UIUtilities.DeactivateWithLock(ContractWorkPanel, ref open);

        foreach(Transform child in ContractWorkPanel.transform)
            Destroy(child.gameObject);

        locked = true;

        StartCoroutine(LockCooldown());
    }

    private IEnumerator LockCooldown()
    {
        DateTime week_from_now = TimeManager.CurrentDate.AddDays(7.0);
        while (TimeManager.CurrentDate < week_from_now)
            yield return null;
        locked = false;
    }

    public void SetActiveContract(Contract contract)
    {
        ActiveContract = contract;

        if (ActiveContract != null)
        {
            TimeManager.PerDayEvent.AddListener(WorkOnActiveContract);
            ActiveContractNameText.text = contract.Name;
            ActiveContractPayText.text = contract.Payment.ToString("N");
            ActiveContractDeadlineText.text = contract.DaysRemaining.ToString();
        }
        else
        {
            TimeManager.PerDayEvent.RemoveListener(WorkOnActiveContract);
            ActiveContractNameText.text = "No Contract";
            ActiveContractPayText.text = "N/A";
            ActiveContractDeadlineText.text = "N/A";
        }
    }

    public void WorkOnActiveContract()
    {
        //called daily in-game
        if (ActiveContract == null) return;

        ActiveContract.WorkContract();
        UpdateActiveContractInfo();
    }

    public void CancelActiveContract()
    {
        if (ActiveContract == null) return;

        ActiveContract.CancelContract();
        SetActiveContract(null);
    }

    public void UpdateActiveContractInfo()
    {
        if (ActiveContract != null)
        {
            ActiveContractDeadlineText.text = ActiveContract.DaysRemaining.ToString();

            float contract_progress =
                (float)(ActiveContract.TotalPointsNeeded - ActiveContract.SkillPointsNeeded.Sum()) /
                ActiveContract.TotalPointsNeeded;

            ActiveContractProgressBar.SetProgress(contract_progress);
        }
        else
        {
            //ActiveContractProgressBar.rectTransform.sizeDelta = MaxBarSize;
            //ActiveContractProgressBar.color = Color.white;

            //ActiveContractDeadlineBar.rectTransform.sizeDelta = MaxBarSize;
            //ActiveContractDeadlineBar.color = Color.white;
        }
    }
}