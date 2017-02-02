using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyOfficeItem : MonoBehaviour
{
    public Text LocationText;
    public Text BuildingCountText;
    public Text UpkeepCostText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateData(Office office)
    {
        LocationText.text = office.OfficeLocation.Name;
        BuildingCountText.text = office.Buildings.Count.ToString();
        UpkeepCostText.text = office.TotalUpkeepCost.ToString("C");
    }
}