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
}