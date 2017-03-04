using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmployeeItem : MonoBehaviour, IPointerDownHandler
{
    public Employee ItemEmployee;
    public Text NameAgeText;
    public Text BriefSkillsText;
    public Text TitleText;

    void Start() {}

    void Update() {}

    public void OnPointerDown(PointerEventData eventData)
    {
        CompanyManager.Instance.PopulateEmployeeDetail(ItemEmployee);
    }

    public void PopulateData(Employee employee)
    {
        ItemEmployee = employee;
        NameAgeText.text = string.Format(
            "{0} ({1})",
            employee.Name,
            employee.Age);
        BriefSkillsText.text = string.Join(" | ", employee.Skills.Skills.Select(x =>
            string.Format("{0}: {1}", SkillInfo.SKILL_ABBR[(int)x.Skill], x.Level)).ToArray());
        TitleText.text = employee.CurrentTitle.Name;
    }
}
