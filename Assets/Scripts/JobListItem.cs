using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class JobListItem : MonoBehaviour
{
    public Job ListItemJob;

    public Text CompanyTitleText;
    public Text JobSalaryText;
    public Image ExpandImage;
    public RectTransform AllInfoPanel;
    public Image[] JobSkillRequirementBars;
    public Text[] JobSkillRequirementTexts;
    public Button ActionButton;

    public Vector2 MaxSkillReqBarSize;

    private bool infoOpen;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateListItem(Job job)
    {
        ListItemJob = job;

        MaxSkillReqBarSize = JobSkillRequirementBars[0].rectTransform.sizeDelta;

        CompanyTitleText.text = string.Format(
            "{0} - {1}",
            "Default Company",
            job.CurrentTitle.Name);
        JobSalaryText.text = job.Salary.ToString("C0");

        for (int i = 0; i < 5; i++)
        {
            JobSkillRequirementBars[i].rectTransform.sizeDelta =
                new Vector2(MaxSkillReqBarSize.x * (job.CurrentTitle.SkillRequirements[i] / 10.0f), 0.0f);
            JobSkillRequirementTexts[i].text = job.CurrentTitle.SkillRequirements[i].ToString();
        }
    }

    public void SetupActionButton(Color buttonColor, string buttonText, UnityAction buttonAction)
    {
        ActionButton.colors = ColorBlocks.GetColorBlock(buttonColor);
        ActionButton.GetComponentInChildren<Text>().text = buttonText;
        ActionButton.onClick.AddListener(buttonAction);
    }

    public void OnExpandClick()
    {
        infoOpen = !infoOpen;
        AllInfoPanel.gameObject.SetActive(infoOpen);
        ExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, infoOpen ? 0.0f : 180.0f);
    }

    public void OnApplyClick()
    {
        
    }
}