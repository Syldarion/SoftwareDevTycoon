using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompanyOfficeItem : MonoBehaviour, IPointerDownHandler
{
    public Office ItemOffice;
    public Text LocationText;
    public Text BuildingCountText;
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
        LocationText.text = string.Format("Location\n{0}", office.OfficeLocation.Name);
        //BuildingCountText.text = office.Buildings.Count.ToString();
        UpkeepCostText.text = office.TotalUpkeepCost.ToString("C");
    }
}
