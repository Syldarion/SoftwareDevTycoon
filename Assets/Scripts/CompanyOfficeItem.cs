using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompanyOfficeItem : MonoBehaviour, IPointerDownHandler
{
    public Office ItemOffice;
    public Text LocationText;
    public Text UpkeepCostText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CompanyManager.Instance.PopulateOfficeDetail(ItemOffice);
    }

    public void PopulateData(Office office)
    {
        ItemOffice = office;
        LocationText.text = office.OfficeLocation.Name;
        UpkeepCostText.text = Company.MyCompany.CompanyOffices.Contains(office) 
            ? "Upkeep: " + office.TotalUpkeepCost.ToString("C0")
            : "Cost: " + office.PurchasePrice.ToString("C0");
    }
}
