using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TabControl : Button
{
    public CanvasGroup TabPanel;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        GetComponentInParent<TabGroup>().SetActiveTab(this);
    }
}