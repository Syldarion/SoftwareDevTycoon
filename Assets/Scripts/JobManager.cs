using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class JobManager : Singleton<JobManager>
{
    public JobListItem JobListItemPrefab;

    public CanvasGroup JobSearchPanel;
    public CanvasGroup JobInfoPanel;

    [Header("Job Search UI References")]
    public RectTransform ApplicationListTransform;
    public RectTransform SearchListTransform;
    public List<JobListItem> SearchListItems;
    public Image ApplicationExpandImage;
    public Image SearchExpandImage;
    public InputField CompanyNameFilter;
    public InputField JobTitleFilter;
    public InputField JobSalaryFilter;
    public Text[] SkillRequirementFilterTexts;

    [Header("Job Info UI References")]
    public Text JobTitleText;
    public Text JobCompanyText;
    public Text JobPayText;
    public Text ActiveTaskNameText;
    public ProgressBar ActiveTaskProgressBar;
    public Text ActiveTaskDaysText;
    public Text ProjectNameText;
    public ProgressBar ProjectProgressBar;

    [Header("Variables")]
    public int[] SkillRequirementFilters;
    public List<JobApplication> ActiveApplications;

    private bool applicationSectionOpen = true;
    private bool searchSectionOpen = true;
    private bool open;

    void Awake()
    {
        Instance = this;

        SkillRequirementFilters = new int[5];
        for (int i = 0; i < 5; i++)
        {
            SkillRequirementFilters[i] = Character.MyCharacter.Skills[i].Level;
            SkillRequirementFilterTexts[i].text = SkillRequirementFilters[i].ToString();
        }

        ActiveApplications = new List<JobApplication>();
    }

    void Start()
    {
        SetupEvents();
    }

    public void SetupEvents()
    {
        TimeManager.PerDayEvent.RemoveListener(CheckActiveApplications);
        TimeManager.PerDayEvent.RemoveListener(UpdateJobInfo);

        TimeManager.PerDayEvent.AddListener(CheckActiveApplications);
        TimeManager.PerDayEvent.AddListener(UpdateJobInfo);
    }

    void Update()
    {
        if (ControlKeys.GetControlKeyDown(ControlKeys.OPEN_JOB_PANEL))
        {
            if (!open && Job.MyJob == null)
                OpenJobSearch();
            else if (Job.MyJob == null)
                CloseJobSearch();
            else if (!open)
                OpenJobInfo();
            else
                CloseJobInfo();
        }

        if (ControlKeys.GetControlKeyDown(ControlKeys.PRINT_JOB_DEBUG))
        {
            if (Job.MyJob == null)
                Debug.Log("No job");
            else
                Debug.Log(string.Format(
                    "Current Job Info\nCompany: {0}\nSalary: {1}\nTitle: {2}\nPerformance: {3}",
                    Job.MyJob.CompanyName,
                    Job.MyJob.Salary,
                    Job.MyJob.CurrentTitle.Name,
                    Job.MyJob.Performance));
        }
    }

    public void UpdateJobInfo()
    {
        if (Job.MyJob != null)
        {
            
        }
    }

    public void OpenJobSearch()
    {
        open = true;

        TimeManager.Pause();
        TimeManager.Lock();

        UIUtilities.ActivateCanvasGroup(JobSearchPanel);
        
        PopulateApplicationList();
        PopulateSearchList();
    }

    public void PopulateApplicationList()
    {
        foreach (Transform child in ApplicationListTransform)
            Destroy(child.gameObject);
        
        foreach (JobApplication application in ActiveApplications)
        {
            var current = application;

            JobListItem new_list_item = Instantiate(JobListItemPrefab);
            new_list_item.transform.SetParent(ApplicationListTransform, false);

            new_list_item.PopulateListItem(current.AppliedJob);
            if (current.Accepted)
                new_list_item.SetupActionButton(Color.green, "Accepted", () =>
                {
                    DialogueBox.Instance.CreateYesNoDialogue("Start this job?",
                        () =>
                        {
                            new_list_item.ListItemJob.HirePlayer();
                            CloseJobSearch();
                            DialogueBox.Instance.Cleanup();
                            Destroy(new_list_item.gameObject);
                            ActiveApplications.Remove(current);
                        },
                        () =>
                        {
                            DialogueBox.Instance.Cleanup();
                            Destroy(new_list_item.gameObject);
                        });
                });
            else if (current.Waiting)
                new_list_item.SetupActionButton(Color.yellow, "Waiting...", () => { });
            else
                new_list_item.SetupActionButton(Color.red, "Rejected", () =>
                {
                    Destroy(new_list_item.gameObject);
                    ActiveApplications.Remove(current);
                });
        }
    }

    public void PopulateSearchList()
    {
        SearchListItems = new List<JobListItem>();
        foreach (Transform child in SearchListTransform)
            Destroy(child.gameObject);

        Job[] generated_jobs = Job.GenerateJobs(50);
        foreach (Job job in generated_jobs)
        {
            JobListItem new_list_item = Instantiate(JobListItemPrefab);
            new_list_item.transform.SetParent(SearchListTransform, false);

            new_list_item.PopulateListItem(job);
            new_list_item.SetupActionButton(Color.green, "Apply", () =>
            {
                CreateJobApplication(new_list_item.ListItemJob);
                SearchListItems.Remove(new_list_item);
                Destroy(new_list_item.gameObject);
            });

            SearchListItems.Add(new_list_item);
        }
    }

    public void CloseJobSearch()
    {
        open = false;
        UIUtilities.DeactivateCanvasGroup(JobSearchPanel);

        TimeManager.Unlock();
        TimeManager.Unpause();
    }

    public void OpenJobInfo()
    {
        if (Job.MyJob == null) return;

        TimeManager.Pause();
        TimeManager.Lock();

        open = true;
        TimeManager.Pause();
        UIUtilities.ActivateCanvasGroup(JobInfoPanel);

        JobTitleText.text = Job.MyJob.CurrentTitle.Name;
        JobCompanyText.text = Job.MyJob.CompanyName;
        JobPayText.text = Job.MyJob.Salary.ToString();
    }

    public void CloseJobInfo()
    {
        open = false;
        UIUtilities.DeactivateCanvasGroup(JobInfoPanel);

        TimeManager.Unlock();
        TimeManager.Unpause();
    }

    public void FilterSearchResults()
    {
        foreach (JobListItem list_item in SearchListItems)
        {
            bool keep_active = !(JobTitleFilter.text != string.Empty 
                && !list_item.ListItemJob.CurrentTitle.Name.Contains(JobTitleFilter.text));
            if (JobSalaryFilter.text != string.Empty && list_item.ListItemJob.Salary < int.Parse(JobSalaryFilter.text))
                keep_active = false;
            for (int i = 0; i < 5; i++)
                if (list_item.ListItemJob.CurrentTitle.SkillRequirements[i] > SkillRequirementFilters[i])
                    keep_active = false;
            list_item.gameObject.SetActive(keep_active);
        }
    }

    public void IncreaseSkillFilter(int index)
    {
        SkillRequirementFilters[index] = Mathf.Clamp(SkillRequirementFilters[index] + 1, 0, 10);
        SkillRequirementFilterTexts[index].text = SkillRequirementFilters[index].ToString();
    }

    public void DecreaseSkillFilter(int index)
    {
        SkillRequirementFilters[index] = Mathf.Clamp(SkillRequirementFilters[index] - 1, 0, 10);
        SkillRequirementFilterTexts[index].text = SkillRequirementFilters[index].ToString();
    }

    public void OnExpandApplicationsClick()
    {
        applicationSectionOpen = !applicationSectionOpen;
        ApplicationListTransform.gameObject.SetActive(applicationSectionOpen);
        ApplicationExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, applicationSectionOpen ? 0.0f : 180.0f);
    }

    public void OnExpandSearchClick()
    {
        searchSectionOpen = !searchSectionOpen;
        SearchListTransform.gameObject.SetActive(searchSectionOpen);
        SearchExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, searchSectionOpen ? 0.0f : 180.0f);
    }

    public void CreateJobApplication(Job job)
    {
        JobApplication new_application = new JobApplication(job);
        ActiveApplications.Add(new_application);
        PopulateApplicationList();
    }

    public void RemoveJobApplication(JobApplication application)
    {
        ActiveApplications.Remove(application);
        PopulateApplicationList();
    }

    public void CheckActiveApplications()
    {
        foreach (JobApplication application in ActiveApplications)
        {
            application.CheckApplication();
        }
    }
}