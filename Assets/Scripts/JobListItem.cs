using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

public class JobListItem : MonoBehaviour
{
    public Job ListItemJob;

    public Text CompanyTitleText;
    public Text JobSalaryText;
    public Text SkillRequirementText;
    public Button ActionButton;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateListItem(Job job)
    {
        ListItemJob = job;

        CompanyTitleText.text = string.Format(
            "{0} - {1}",
            "Default Company",
            job.CurrentTitle.Name);
        JobSalaryText.text = job.Salary.ToString("C0");

        SkillRequirementText.text = job.CurrentTitle.SkillRequirements.Info(" | ", true);
    }

    public void SetupActionButton(Color buttonColor, string buttonText, UnityAction buttonAction)
    {
        ActionButton.colors = ColorBlocks.GetColorBlock(buttonColor);
        ActionButton.GetComponentInChildren<Text>().text = buttonText;
        ActionButton.onClick.AddListener(buttonAction);
    }
}
