using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProjectItem : MonoBehaviour, IPointerDownHandler
{
    public Project ItemProject;

    public Text ProjectNameText;
    public Text ProjectStatusText;

    void Start() {}

    void Update() {}

    public void OnPointerDown(PointerEventData eventData)
    {
        CompanyManager.Instance.PopulateProjectDetail(ItemProject);
    }

    public void PopulateData(Project project)
    {
        ItemProject = project;
        ProjectNameText.text = project.Name;
        ProjectStatusText.text = project.ProjectStatus;
    }
}
