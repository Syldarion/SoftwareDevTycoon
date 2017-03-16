using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bank
{
    public static int TotalFunds { get { return sPlayerFunds + sCompanyFunds; } }
    public static int PlayerFunds
    {
        get { return sPlayerFunds; }
        set
        {
            sPlayerFunds = value;
            OnFundsChanged.Invoke(TotalFunds);
        }
    }
    public static int CompanyFunds
    {
        get { return sCompanyFunds; }
        set
        {
            sCompanyFunds = value;
            OnFundsChanged.Invoke(TotalFunds);
        }
    }

    public static UnityEventInt OnFundsChanged = new UnityEventInt();

    private static int sPlayerFunds = 0;
    private static int sCompanyFunds = 0;
}
