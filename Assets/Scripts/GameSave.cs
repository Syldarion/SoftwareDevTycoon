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
    private Contract activeContract;
    [SerializeField]
    private string dateString;
    [SerializeField]
    private JobApplication[] activeApplications;
    [SerializeField]
    private Job myJob;

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
        LoadContractInfo();
        LoadTimeInfo();
        LoadJobManagerInfo();
        LoadJobInfo();
    }

    private void LoadCharacterInfo()
    {
        Character.MyCharacter = myCharacter;
    }

    private void LoadContractInfo()
    {
        Contract.SetActiveContract(activeContract);
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

    private void LoadJobManagerInfo()
    {
        foreach (JobApplication app in activeApplications)
            JobManager.Instance.ActiveApplications.Add(app);
    }

    private void LoadJobInfo()
    {
        Job.MyJob = myJob;
        Job.MyJob.SetupEvents();
    }

    public void SaveGame()
    {
        myCharacter = Character.MyCharacter;
        activeContract = Contract.ActiveContract;
        dateString = TimeManager.DateString;
        activeApplications = JobManager.Instance.ActiveApplications.ToArray();
        myJob = Job.MyJob;
    }
}