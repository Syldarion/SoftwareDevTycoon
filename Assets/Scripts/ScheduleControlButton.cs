using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScheduleControlButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField]
    private DayOfWeek controlDay;
    [Range(1, 24)]
    [SerializeField]
    private int controlHour;
    [SerializeField]
    private ScheduleItem controlItem;

    void Start()
    {

    }

    public void SetItemToManagerItem()
    {
        controlItem = ScheduleManager.Instance.SelectedItemType;

        switch (controlItem)
        {
            case ScheduleItem.Free:
                GetComponent<Image>().color = Color.white;
                break;
            case ScheduleItem.Job:
                GetComponent<Image>().color = Color.magenta;
                break;
            case ScheduleItem.ContractWork:
                GetComponent<Image>().color = Color.green;
                break;
            case ScheduleItem.Sleep:
                GetComponent<Image>().color = Color.cyan;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ApplyToSchdule();
    }

    private void ApplyToSchdule()
    {
        ScheduleManager.Instance.ActiveSchedule.Schedules[(int)controlDay].Items[controlHour - 1] = controlItem;
    }

    public void SetDay(DayOfWeek day)
    {
        controlDay = day;
    }

    public void SetHour(int hour)
    {
        controlHour = Mathf.Clamp(hour, 1, 24);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Input.GetMouseButton(0))
            SetItemToManagerItem();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetItemToManagerItem();
    }
}