using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectItem : MonoBehaviour
{
    public Text ProjectNameText;
    public Text ProjectStatusText;
    public Text ProjectQualityText;
    public Text ProjectPayoutText;

    void Start() {}

    void Update() {}

    public void PopulateData(Project project)
    {
        ProjectNameText.text = project.Name;
        switch(project.CurrentStatus)
        {
            case Project.Status.InProgress:
                ProjectStatusText.text = "In Progress";
                break;
            case Project.Status.Halted:
                ProjectStatusText.text = "Halted";
                break;
            case Project.Status.OnSale:
                ProjectStatusText.text = "On Sale";
                break;
            case Project.Status.Retired:
                ProjectStatusText.text = "Retired";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        ProjectQualityText.text = project.QualityLevels.Info(" | ", true);
        ProjectPayoutText.text = project.CurrentPayout.ToString("C0");
    }
}
