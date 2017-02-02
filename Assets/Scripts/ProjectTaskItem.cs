using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectTaskItem : MonoBehaviour
{
    public Text TaskNameText;
    public Image TaskStatusMarker;
    public Text WorkerNameText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateData(ProjectTask task)
    {
        TaskNameText.text = task.Name;
        TaskStatusMarker.color = task.Complete ? Color.green : task.Progress > 0 ? Color.yellow : Color.red;
        WorkerNameText.text = task.Worker != null ? task.Worker.Name : "None";
    }
}