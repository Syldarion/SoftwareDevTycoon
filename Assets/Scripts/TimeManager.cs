using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : Singleton<TimeManager>
{
    public static UnityEvent PerDayEvent = new UnityEvent();
    public static UnityEvent PerWeekEvent = new UnityEvent();
    public static UnityEvent PerMonthEvent = new UnityEvent();
    public static UnityEvent PerYearEvent = new UnityEvent();
    public static UnityEventBool OnTogglePauseEvent = new UnityEventBool();

    public static DateTime CurrentDate { get; private set; }
    public static int Year { get { return CurrentDate.Year; } }
    public static int Month { get { return CurrentDate.Month; } }
    public static int Week { get { return Mathf.CeilToInt((float)CurrentDate.DayOfYear / 7); } }
    public static int Day { get { return CurrentDate.Day; } }
    public static string DateString { get { return CurrentDate.ToString("ddMMyyyy"); } }

    private static bool PAUSED;
    private static bool LOCKED;

    void Awake()
    {
        Instance = this;
        PAUSED = false;
        CurrentDate = DateTime.Today;
    }

    void Start()
    {
        StopAllCoroutines();
        StartCoroutine(PerDayTick());
    }

    void Update()
    {
        if (SDTControls.GetControlKeyDown(SDTControls.PAUSE_TOGGLE) && !LOCKED)
            TogglePause();
    }

    public static void SetCurrentDate(DateTime date)
    {
        CurrentDate = date;
    }

    public static void TogglePause()
    {
        PAUSED = !PAUSED;
        OnTogglePauseEvent.Invoke(PAUSED);
    }

    public static void Pause()
    {
        if (LOCKED || PAUSED) return;
        PAUSED = true;
        OnTogglePauseEvent.Invoke(PAUSED);
    }

    public static void Unpause()
    {
        if (LOCKED || !PAUSED) return;
        PAUSED = false;
        OnTogglePauseEvent.Invoke(PAUSED);
    }

    public static void Lock()
    {
        LOCKED = true;
    }

    public static void Unlock()
    {
        LOCKED = false;
    }

    IEnumerator PerDayTick()
    {
        while (Application.isPlaying)
        {
            while (!PAUSED)
            {
                int old_week = Week;
                int old_month = Month;
                int old_year = Year;

                CurrentDate = CurrentDate.AddDays(1.0);

                //daily tick
                PerDayEvent.Invoke();

                if (Year > old_year)
                {
                    PerYearEvent.Invoke();
                    old_week = 0;
                    old_month = 0;
                }

                if (Week > old_week)
                    PerWeekEvent.Invoke();
                if (Month > old_month)
                    PerMonthEvent.Invoke();

                yield return new WaitForSeconds(1.0f);
            }

            yield return null;
        }
    }
}
