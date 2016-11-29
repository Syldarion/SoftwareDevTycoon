using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public enum ScheduleItem
{
    Free,
    Job,
    ContractWork,
    Sleep
}

[Serializable]
public struct DaySchedule
{
    public DayOfWeek Day;
    public ScheduleItem[] Items;

    public DaySchedule(DayOfWeek day, ScheduleItem[] items)
    {
        Day = day;
        Items = new ScheduleItem[24];
        for (int i = 0; i < items.Length && i < 24; i++)
            Items[i] = items[i];
    }
}

[Serializable]
public class Schedule
{
    public DaySchedule[] Schedules;

    public Schedule()
    {
        Schedules = new DaySchedule[7];
        for (int i = 0; i < 7; i++)
        {
            Schedules[i] = new DaySchedule(
                (DayOfWeek)i,
                new ScheduleItem[24]);
        }
    }

    public Schedule(DaySchedule[] schedules)
    {
        Schedules = new DaySchedule[7];
        for (int i = 0; i < schedules.Length && i < 7; i++)
            Schedules[i] = schedules[i];
    }

    public DaySchedule GetTodaysSchedule()
    {
        return GetSchedule(TimeManager.CurrentDate.DayOfWeek);
    }

    public DaySchedule GetSchedule(DayOfWeek day)
    {
        return Schedules.Where(x => x.Day == day).ToArray()[0];
    }
}

public class ScheduleManager : Singleton<ScheduleManager>
{
    public ScheduleItem SelectedItemType;

    public CanvasGroup SchedulePanel;
    public RectTransform[] DaySchedulePanels;
    public Button[] ItemTypeSelectButtons;
    public ScheduleControlButton ScheduleButtonPrefab;

    public Schedule ActiveSchedule;

    private DayOfWeek[] days =
    {
        DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday,
    };
    private bool open;

    void Awake()
    {
        Instance = this;

        ActiveSchedule = new Schedule();

        for (int i = 0; i < ItemTypeSelectButtons.Length; i++)
        {
            var i1 = i;
            ItemTypeSelectButtons[i].onClick.AddListener(() => SelectItemType((ScheduleItem)i1));
        }

        open = false;
    }

    void Start()
    {
        SetupSchedulePlanner();
    }

    void Update()
    {
        if (ControlKeys.GetControlKeyDown(ControlKeys.OPEN_SCHEDULE_PANEL))
        {
            if(open)
                CloseSchedulePlanner();
            else
                OpenSchedulePlanner();
        }
    }

    public void SetupSchedulePlanner()
    {
        for (int i = 0; i < days.Length && i < DaySchedulePanels.Length; i++)
        {
            for (int j = 1; j <= 24; j++)
            {
                ScheduleControlButton new_button = Instantiate(ScheduleButtonPrefab);
                new_button.transform.SetParent(DaySchedulePanels[i], false);
                
                new_button.SetDay(days[i]);
                new_button.SetHour(j);
            }
        }  
    }

    void OpenSchedulePlanner()
    {
        open = true;

        TimeManager.Pause();
        TimeManager.Lock();

        UIUtilities.ActivateCanvasGroup(SchedulePanel);
    }

    void CloseSchedulePlanner()
    {
        open = false;

        UIUtilities.DeactivateCanvasGroup(SchedulePanel);

        TimeManager.Unlock();
        TimeManager.Unpause();
    }

    public void SelectItemType(ScheduleItem type)
    {
        SelectedItemType = type;
    }
}