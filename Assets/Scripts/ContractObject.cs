using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngineInternal;

public class ContractObject : MonoBehaviour, IPointerDownHandler
{
    public Contract ObjectContract;

    public Text ContractName;
    public Text ContractPay;
    public Text ContractDays;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateContractInfo(Contract contract)
    {
        ObjectContract = contract;

        ContractName.text = contract.Name;
        ContractPay.text = contract.Payment.ToString("C0");
        ContractDays.text = string.Format(
            "{0} days", contract.DaysToComplete.ToString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ContractManager.Instance.PopulateContractDetail(ObjectContract);
    }
}
