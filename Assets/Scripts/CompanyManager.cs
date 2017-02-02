using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CompanyManager : Singleton<CompanyManager>
{
    [Header("New Company UI")]
    public CanvasGroup NewCompanyPanel;
    public InputField CompanyNameInput;
    public InputField FirstOfficeSpaceInput;
    public Button CreateCompanyButton;
    public Text NewCompanyCostText;

    [Header("Company Info UI")]
    public CanvasGroup InfoPanel;
    public Text InfoName;
    public Text InfoOfficeCount;
    public Text InfoEmployeeCount;
    public ProgressBar InfoProjectProgress;

    [Header("Company Project UI")]
    public CanvasGroup ProjectPanel;
    public Text ProjectName;
    public RectTransform ProjectTaskList;
    public Text ProjectProgressText;
    public ProjectTaskItem ProjectTaskItemPrefab;

    [Header("Company Offices UI")]
    public CanvasGroup OfficesPanel;
    public RectTransform OfficeList;
    public CompanyOfficeItem CompanyOfficeItemPrefab;

    [Header("Company Employees UI")]
    public CanvasGroup EmployeesPanel;
    public RectTransform EmployeeList;
    public EmployeeItem EmployeeItemPrefab;

    [Header("Task Detail UI")]
    public Text TaskDetailName;
    public Text[] TaskDetailWorkNeeded;
    public Text TaskDetailDaysRemaining;
    public Text TaskDetailWorkerName;
    public Text TaskDetailStatus;
    public Dropdown TaskDetailWorkerDropdown;

    [Header("Office Detail UI")]
    public Text OfficeDetailLocation;
    public Text OfficeDetailBuildingCount;
    public Text OfficeDetailBonuses;
    public Text OfficeDetailRemainingSpace;
    public Button OfficeAddBuildingButton;
    public Button OfficeSellButton;
    public Button OfficeBuyButton;

    [Header("Employee Detail UI")]
    public Text EmployeeDetailNameAge;
    public Image EmployeeDetailGender;
    public Text EmployeeDetailLocation;
    public Text[] EmployeeDetailSkillLevels;
    public Text EmployeeDetailTitle;
    public Text EmployeeDetailPay;
    public Text EmployeeDetailHireDate;
    public Text EmployeeDetailPerformance;
    public Button EmployeeHireButton;
    public Button EmployeeFireButton;
    public Button EmployeeTrainButton;

    private ProjectTask selectedTask;
    private Office selectedOffice;
    private Employee selectedEmployee;
    private bool open;

    void Start()
    {
        Instance = this;
        selectedTask = null;
        selectedOffice = null;
        selectedEmployee = null;
        open = false;
    }

    void Update()
    {

    }

    public void UpdateCompanyInfoPanel()
    {
        //call this whenever work is done on the company project
        //or if office count or employee count changes
        if (Company.MyCompany == null)
            return;

        InfoName.text = Company.MyCompany.Name();
        InfoOfficeCount.text = Company.MyCompany.CompanyOffices.Count.ToString();
        InfoEmployeeCount.text = Company.MyCompany.TeamSize.ToString();
        InfoProjectProgress.SetProgress(Company.MyCompany.CompanyProject.Progress);
        InfoProjectProgress.SetBarText(Company.MyCompany.CompanyProject.Progress.ToString("P"), true);
    }

    private void OpenNewCompanyPanel()
    {
        UIUtilities.ActivateWithLock(NewCompanyPanel, ref open);
    }

    public void CloseCompanyInfoPanel()
    {
        UIUtilities.DeactivateWithLock(InfoPanel, ref open);
    }

    public void OpenCompanyProjectPanel()
    {
        UIUtilities.ActivateWithLock(ProjectPanel, ref open);

        ProjectName.text = Company.MyCompany.CompanyProject.Name;
        foreach (Transform child in ProjectTaskList)
            Destroy(child.gameObject);
        foreach (ProjectTask task in Company.MyCompany.CompanyProject.ProjectTasks)
        {
            var new_task_item = Instantiate(ProjectTaskItemPrefab);
            new_task_item.PopulateData(task);
            new_task_item.transform.SetParent(ProjectTaskList, false);
        }
        ProjectProgressText.text = Company.MyCompany.CompanyProject.Progress.ToString("P");
    }

    public void CloseCompanyProjectPanel()
    {
        UIUtilities.DeactivateWithLock(ProjectPanel, ref open);
    }

    public void OpenCompanyOfficesPanel()
    {
        UIUtilities.ActivateWithLock(OfficesPanel, ref open);

        foreach (Transform child in OfficeList)
            Destroy(child.gameObject);
        foreach (Office office in Company.MyCompany.CompanyOffices)
        {
            var new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(office);
            new_office_item.transform.SetParent(OfficeList, false);
        }
    }

    public void CloseCompanyOfficesPanel()
    {
        UIUtilities.DeactivateWithLock(OfficesPanel, ref open);
    }

    public void OpenCompanyEmployeesPanel()
    {
        UIUtilities.ActivateWithLock(EmployeesPanel, ref open);

        foreach (Transform child in EmployeeList)
            Destroy(child.gameObject);
        foreach (Employee employee in Company.MyCompany.EmployeeList())
        {
            var new_employee_item = Instantiate(EmployeeItemPrefab);
            new_employee_item.PopulateData(employee);
            new_employee_item.transform.SetParent(EmployeeList, false);
        }
    }

    public void CloseCompanyEmployeesPanel()
    {
        UIUtilities.DeactivateWithLock(EmployeesPanel, ref open);
    }

    public void PopulateTaskDetail(ProjectTask task)
    {
        if (task == null) return;

        TaskDetailName.text = task.Name;
        for (int i = 0; i < task.WorkNeeded.Length; i++)
            TaskDetailWorkNeeded[i].text = task.WorkNeeded[i].ToString();
        TaskDetailDaysRemaining.text = task.DaysRemaining.ToString();
        TaskDetailWorkerName.text = task.Worker.Name;
        TaskDetailStatus.text = task.Complete
            ? "Complete"
            : task.Progress > 0
                ? "In Progress"
                : "Ready";
        TaskDetailWorkerDropdown.ClearOptions();
        List<Dropdown.OptionData> employee_options =
            Company.MyCompany.EmployeeList().Select(emp => new Dropdown.OptionData(emp.Name)).ToList();
        TaskDetailWorkerDropdown.AddOptions(employee_options);
    }

    public void OnTaskDetailWorkerDropdownChanged(int selectedIndex)
    {
        Employee selected_employee =
            Company.MyCompany.EmployeeList()
                   .First(x => x.Name == TaskDetailWorkerDropdown.options[selectedIndex].text);

        selectedTask.SetWorker(selected_employee);
    }

    public void PopulateOfficeDetail(Office office)
    {
        if (office == null) return;

        OfficeDetailLocation.text = office.OfficeLocation.Name;
        OfficeDetailBuildingCount.text = office.Buildings.Count.ToString();
        //office building bonuses, once they're implemented
        OfficeDetailRemainingSpace.text = office.RemainingSpace.ToString();
        OfficeAddBuildingButton.interactable = office.RemainingSpace > 0;
        OfficeBuyButton.interactable = !Company.MyCompany.CompanyOffices.Contains(office);
        OfficeSellButton.interactable = Company.MyCompany.CompanyOffices.Contains(office) &&
                                        Company.MyCompany.CompanyOffices.Count > 1;
    }

    public void PopulateEmployeeDetail(Employee employee)
    {
        if (employee == null) return;

        bool works_for_company = Company.MyCompany.EmployeeList().Contains(employee);

        EmployeeDetailNameAge.text = string.Format(
            "{0} ({1})", employee.Name, employee.Age);
        //set employee gender image after you make images for the slot
        EmployeeDetailLocation.text = employee.CurrentLocation.Name;
        for (int i = 0; i < employee.Skills.Length; i++)
            EmployeeDetailSkillLevels[i].text = employee.Skills[i].Level.ToString();
        EmployeeDetailTitle.text = employee.CurrentTitle.Name;
        EmployeeDetailPay.text = employee.Pay.ToString("C");
        EmployeeDetailHireDate.text = DateTime.FromBinary(employee.HireDateBinary).ToLongDateString();
        //EmployeeDetailPerformance.text
        EmployeeHireButton.gameObject.SetActive(!works_for_company);
        EmployeeHireButton.onClick.AddListener(() => Company.MyCompany.HireEmployee(employee));
        EmployeeFireButton.gameObject.SetActive(works_for_company);
        EmployeeFireButton.onClick.AddListener(() => Company.MyCompany.FireEmployee(employee));
        EmployeeTrainButton.gameObject.SetActive(works_for_company);
        EmployeeTrainButton.onClick.AddListener(() => Company.MyCompany.TrainEmployee(employee));
    }

    public void UpdateNewCompanyCostText()
    {
        int new_office_space = int.Parse(FirstOfficeSpaceInput.text);
        int total_cost = Company.BASE_COMPANY_COST + (Office.COST_PER_SPACE * new_office_space);
        
        CreateCompanyButton.onClick.RemoveListener(CreateCompany);
        if(total_cost <= Character.MyCharacter.Money)
            CreateCompanyButton.onClick.AddListener(CreateCompany);
        CreateCompanyButton.colors =
            ColorBlocks.GetColorBlock(total_cost <= Character.MyCharacter.Money ? Color.green : Color.red);
    }

    public void CreateCompany()
    {
        Company.MyCompany = Company.CreateNewCompany(CompanyNameInput.text, int.Parse(FirstOfficeSpaceInput.text));
    }

    public void ConfirmBankruptcy()
    {
        DialogueBox.Instance.CreateYesNoDialogue("Are you sure you want to declare bankruptcy?",
                                                 () =>
                                                 {
                                                     Company.MyCompany.DeclareBankruptcy();
                                                     DialogueBox.Instance.Cleanup();
                                                 },
                                                 () => { DialogueBox.Instance.Cleanup(); });
    }

    public void SelectTask(ProjectTask task)
    {
        selectedTask = task;
        PopulateTaskDetail(selectedTask);
    }

    public void SelectOffice(Office office)
    {
        selectedOffice = office;
        PopulateOfficeDetail(selectedOffice);
    }

    public void SelectEmployee(Employee employee)
    {
        selectedEmployee = employee;
        PopulateEmployeeDetail(selectedEmployee);
    }
}