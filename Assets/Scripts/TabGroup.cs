using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabGroup : MonoBehaviour
{
    public TabControl ActiveTab;

    private List<TabControl> controls;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void SetActiveTab(TabControl tab)
    {
        if (!controls.Contains(tab)) return;

        UIUtilities.DeactivateCanvasGroup(ActiveTab.TabPanel);
        ActiveTab = tab;
        UIUtilities.ActivateCanvasGroup(ActiveTab.TabPanel);
    }
}