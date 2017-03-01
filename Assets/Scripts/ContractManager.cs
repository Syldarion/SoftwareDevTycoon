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
        if (ControlKeys.GetControlKeyDown(ControlKeys.OPEN_CONTRACT_PANEL))
            OpenContractForm();

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseContractForm();
    }

    public void OpenContractForm()
    {
        if (onCooldown) return;

        foreach (Transform child in ContractWorkPanel.transform)
            Destroy(child.gameObject);
        foreach (Contract contract in Contract.GenerateContracts())
        {
            ContractObject new_contract_object = Instantiate(ContractObjectPrefab).GetComponent<ContractObject>();

            new_contract_object.PopulateContractInfo(contract);
            new_contract_object.transform.SetParent(ContractWorkPanel.transform, false);
        }

        SDTUIController.Instance.OpenCanvas(ContractWorkPanel);
    }

    public void CloseContractForm()
    {
        SDTUIController.Instance.CloseCanvas(ContractWorkPanel);

        StartCoroutine(LockCooldown());
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