using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportManager : Singleton<ReportManager>
{
    public int ContractEarnings;
    public int ProjectEarnings;

    public int NewHireSpending;
    public int SalarySpending;
    public int OfficeUpkeepSpending;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //TimeManager.PerMonthEvent.AddListener(DisplayMonthlyReport);
    }

    void Update()
    {

    }

    public void DisplayMonthlyReport()
    {
        int total_earnings =
            ContractEarnings +
            ProjectEarnings;

        int total_spending = -1 * (
            NewHireSpending +
            SalarySpending +
            OfficeUpkeepSpending);

        DialogueManager.Instance.CreateMessageDialogue(
            string.Format(DialogueMessage.MonthlyReport,
            TimeManager.CurrentDate.ToString("MMMM yyyy"),
            total_earnings,
            ContractEarnings,
            ProjectEarnings,
            total_spending,
            NewHireSpending,
            SalarySpending,
            OfficeUpkeepSpending,
            total_earnings + total_spending),
            Vector3.zero,
            () => { });

        ContractEarnings = 0;
        ProjectEarnings = 0;

        NewHireSpending = 0;
        SalarySpending = 0;
        OfficeUpkeepSpending = 0;
    }
}
