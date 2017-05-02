using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class GameSave
{
    public string Name;
    public string LastModDate;
    
    [SerializeField]
    private Character myCharacter;
    [SerializeField]
    private string dateString;
    [SerializeField]
    private Company myCompany;

    public GameSave(string name, DateTime lastModDate)
    {
        Name = name;

        LastModDate = lastModDate.ToString("g");
    }

    public void LoadGame()
    {
        SaveManager.Instance.ActiveSave = this;
        SaveManager.Instance.StartCoroutine(SaveManager.Instance.LoadActiveSave());
    }

    public void PopulateGameInfo()
    {
        LoadCharacterInfo();
        LoadTimeInfo();
        LoadCompanyInfo();
    }

    private void LoadCharacterInfo()
    {
        GameManager.ActiveCharacter = myCharacter;
        GameManager.ActiveCharacter.SetupEvents();
    }

    private void LoadTimeInfo()
    {
        if (dateString == string.Empty)
            TimeManager.SetCurrentDate(DateTime.Today);
        else
        {
            int day = int.Parse(dateString.Substring(0, 2));
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4));

            TimeManager.SetCurrentDate(new DateTime(year, month, day));
        }
    }

    private void LoadCompanyInfo()
    {
        if (myCompany != null)
        {
            Debug.Log("Has Company");
            Company.MyCompany = myCompany;
            Company.MyCompany.SetupEvents();
        }
    }

    public void SaveGame()
    {
        myCharacter = GameManager.ActiveCharacter;
        dateString = TimeManager.DateString;
        myCompany = Company.MyCompany;
    }
}
