using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeItem : MonoBehaviour
{
    public Text NameAgeText;
    public Text LocationText;
    public Text BriefSkillsText;
    public Text TitleText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateData(Employee employee)
    {
        NameAgeText.text = string.Format(
            "{0} ({1})",
            employee.Name,
            employee.Age);
        LocationText.text = employee.CurrentLocation.Name;
        BriefSkillsText.text = string.Format(
            "{0} | {1} | {2} | {3} | {4}",
            employee.Skills[0].Level,
            employee.Skills[1].Level,
            employee.Skills[2].Level,
            employee.Skills[3].Level,
            employee.Skills[4].Level);
        TitleText.text = employee.CurrentTitle.Name;
    }
}