using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CompanyManager : Singleton<CompanyManager>
{
    public const int EMPLOYEES_TO_GENERATE = 5;
    public const int OFFICES_TO_GENERATE = 3;
    public const int COST_PER_MILE_MOVED = 10;

    [Header("New Company UI")]
    public CanvasGroup NewCompanyPanel;
    public InputField CompanyNameInput;
    public InputField FirstOfficeSpaceInput;
    public Button CreateCompanyButton;
    public Text NewCompanyCostText;

    [Header("Company Info UI")]
    public Text InfoCompanyName;
    public Text InfoOfficeCount;
    public Text InfoEmployeeCount;

    [Header("Company Offices UI")]
    public CanvasGroup OfficesPanel;
    public RectTransform CurrentOfficesList;
    public RectTransform OfficesForSaleList;
    public Image CurrentOfficesExpandImage;
    public Image OfficesForSaleExpandImage;
    public CompanyOfficeItem CompanyOfficeItemPrefab;

    [Header("Company Employees UI")]
    public CanvasGroup EmployeesPanel;
    public RectTransform CurrentEmployeesList;
    public RectTransform AvailableEmployeesList;
    public Image CurrentEmployeesExpandImage;
    public Image AvailableEmployeesExpandImage;
    public EmployeeItem EmployeeItemPrefab;

    [Header("Company Projects UI")]
    public CanvasGroup ProjectsPanel;
    public RectTransform ProjectsList;
    public Button CreateNewProjectButton;
    public ProjectItem ProjectItemPrefab;

    [Header("Office Detail UI")]
    public Text OfficeDetailLocation;
    public Text OfficeDetailUpkeep;
    public Text OfficeDetailCost;
    public Text OfficeDetailBonusPRG;
    public Text OfficeDetailBonusUIX;
    public Text OfficeDetailBonusDBS;
    public Text OfficeDetailBonusNTW;
    public Text OfficeDetailBonusWEB;
    public Text OfficeDetailBonusMorale;
    public Text OfficeDetailBonusSales;
    public Text OfficeDetailSpace;
    public Button OfficeAddFeatureButton;
    public Button OfficeAddSpaceButton;
    public Button OfficeSellButton;
    public Button OfficeBuyButton;

    [Header("Employee Detail UI")]
    public Text EmployeeDetailNameAge;
    public Text EmployeeDetailTitle;
    public Text EmployeeDetailSkillPRG;
    public Text EmployeeDetailSkillUIX;
    public Text EmployeeDetailSkillDBS;
    public Text EmployeeDetailSkillNTW;
    public Text EmployeeDetailSkillWEB;
    public Text EmployeeDetailPay;
    public InputField EmployeeDetailHours;
    public Text EmployeeDetailHireDate;
    public Button EmployeeHireButton;
    public Button EmployeeFireButton;
    public Button EmployeeTrainButton;
    public Button EmployeeMoveButton;

    [Header("Project Detail UI")]
    public RectTransform ProjectDetailPanel;
    public Text ProjectNameText;
    public Text ProjectStatusText;
    public Text ProjectCurrentPayoutText;
    public Text ProjectQualityPRGText;
    public Text ProjectQualityUIXText;
    public Text ProjectQualityDBSText;
    public Text ProjectQualityNTWText;
    public Text ProjectQualityWEBText;
    public Button ProjectSetActiveButton;
    public Button ProjectSellButton;

    [Header("New Project UI")]
    public RectTransform NewProjectPanel;
    public InputField NewProjectNameInput;
    public Button NewProjectCreateButton;
    public Button NewProjectCancelButton;

    [Header("Add Feature UI")]
    public CanvasGroup AddFeaturePanel;
    public Dropdown FeatureListDropdown;
    public Text FeatureBonusText;
    public Button AddFeatureButton;

    [Header("Add Office Space UI")]
    public CanvasGroup AddSpacePanel;
    public InputField ExtraSpaceInput;
    public Button AddSpaceButton;

    [Header("Move Employee UI")]
    public CanvasGroup MoveEmployeePanel;
    public Dropdown MoveLocationDropdown;
    public Text MoveEmployeeCostText;
    public Button MoveEmployeeButton;

    private bool currentOfficesSectionOpen = true;
    private bool officesForSaleSectionOpen = true;
    private bool currentEmployeesSectionOpen = true;
    private bool availableEmployeesSectionOpen = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNewCompanyPanel();
            CloseCompanyOfficesPanel();
            CloseCompanyEmployeesPanel();
            CloseCompanyProjectsPanel();
        }
    }

    public void OpenNewCompanyPanel()
    {
        if(Company.MyCompany != null) return;
        SDTUIController.Instance.OpenCanvas(NewCompanyPanel);
    }

    public void CloseNewCompanyPanel()
    {
        SDTUIController.Instance.CloseCanvas(NewCompanyPanel);
    }

    public void OpenCompanyOfficesPanel()
    {
        if(Company.MyCompany == null) return;
        
        RefreshCurrentOfficesList();
        RefreshAvailableOfficesList();

        SDTUIController.Instance.OpenCanvas(OfficesPanel);
    }

    public void CloseCompanyOfficesPanel()
    {
        SDTUIController.Instance.CloseCanvas(OfficesPanel);
    }

    public void OpenCompanyEmployeesPanel()
    {
        if (Company.MyCompany == null) return;

        RefreshCurrentEmployeesList();
        RefreshAvailableEmployeesList();

        PopulateEmployeeDetail();

        SDTUIController.Instance.OpenCanvas(EmployeesPanel);
    }

    public void CloseCompanyEmployeesPanel()
    {
        SDTUIController.Instance.CloseCanvas(EmployeesPanel);
    }

    public void OpenCompanyProjectsPanel()
    {
        if (Company.MyCompany == null) return;

        if (Company.MyCompany.ActiveCompanyProject != null)
            PopulateProjectDetail(Company.MyCompany.ActiveCompanyProject);

        SDTUIController.Instance.OpenCanvas(ProjectsPanel);
    }

    public void CloseCompanyProjectsPanel()
    {
        SDTUIController.Instance.CloseCanvas(ProjectsPanel);
    }

    public void RefreshCurrentOfficesList()
    {
        foreach (Transform child in CurrentOfficesList)
            Destroy(child.gameObject);
        foreach (Office office in Company.MyCompany.CompanyOffices)
        {
            CompanyOfficeItem new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(office);
            new_office_item.transform.SetParent(CurrentOfficesList, false);
        }

        PopulateOfficeDetail(Company.MyCompany.CompanyOffices[0]);
    }

    public void RefreshAvailableOfficesList()
    {
        foreach (Transform child in OfficesForSaleList)
            Destroy(child.gameObject);
        for (int i = 0; i < OFFICES_TO_GENERATE; i++)
        {
            CompanyOfficeItem new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(Office.GenerateOffice());
            new_office_item.transform.SetParent(OfficesForSaleList, false);
        }

        PopulateOfficeDetail(Company.MyCompany.CompanyOffices[0]);
    }

    public void RefreshCurrentEmployeesList()
    {
        foreach (Transform child in CurrentEmployeesList)
            Destroy(child.gameObject);
        foreach (Employee employee in Company.MyCompany.EmployeeList())
        {
            EmployeeItem new_employee_item = Instantiate(EmployeeItemPrefab);
            new_employee_item.PopulateData(employee);
            new_employee_item.transform.SetParent(CurrentEmployeesList, false);
        }

        PopulateEmployeeDetail();
    }

    public void RefreshAvailableEmployeesList()
    {
        foreach(Transform child in AvailableEmployeesList)
            Destroy(child.gameObject);
        for (int i = 0; i < EMPLOYEES_TO_GENERATE; i++)
        {
            EmployeeItem new_employee_item = Instantiate(EmployeeItemPrefab);
            new_employee_item.PopulateData(Employee.GenerateEmployee());
            new_employee_item.transform.SetParent(AvailableEmployeesList, false);
        }

        PopulateEmployeeDetail();
    }

    public void RefreshProjectsList()
    {
        foreach (Transform child in ProjectsList)
            Destroy(child.gameObject);
        foreach(Project project in Company.MyCompany.CompanyProjects.OrderBy(x => x.CurrentStatus))
        {
            ProjectItem new_project_item = Instantiate(ProjectItemPrefab);
            new_project_item.PopulateData(project);
            new_project_item.transform.SetParent(ProjectsList, false);
        }

        CreateNewProjectButton.transform.SetAsLastSibling();
    }

    public void PopulateOfficeDetail(Office office)
    {
        if (office == null) return;
        bool is_company_office = Company.MyCompany.CompanyOffices.Contains(office);

        OfficeDetailLocation.text = office.OfficeLocation.Name;
        OfficeDetailUpkeep.text = office.TotalUpkeepCost.ToString("C0");
        OfficeDetailCost.text = is_company_office
            ? office.SellPrice.ToString("C0")
            : office.PurchasePrice.ToString("C0");
        OfficeDetailBonusPRG.text = office.QualityBonuses[0].ToString("P");
        OfficeDetailBonusUIX.text = office.QualityBonuses[1].ToString("P");
        OfficeDetailBonusDBS.text = office.QualityBonuses[2].ToString("P");
        OfficeDetailBonusNTW.text = office.QualityBonuses[3].ToString("P");
        OfficeDetailBonusWEB.text = office.QualityBonuses[4].ToString("P");
        OfficeDetailBonusMorale.text = office.MoraleModifier.ToString("P");
        OfficeDetailBonusSales.text = office.SalesModifier.ToString("P");
        OfficeDetailSpace.text = string.Format("{0}/{1}", office.RemainingSpace, office.Space);

        OfficeAddFeatureButton.onClick.RemoveAllListeners();
        OfficeAddSpaceButton.onClick.RemoveAllListeners();
        OfficeBuyButton.onClick.RemoveAllListeners();
        OfficeSellButton.onClick.RemoveAllListeners();

        OfficeAddFeatureButton.onClick.AddListener(() =>
        {
            OpenAddFeaturePanel(office);
        });
        OfficeAddSpaceButton.onClick.AddListener(() =>
        {
            OpenAddSpacePanel(office);
        });
        OfficeBuyButton.onClick.AddListener(() =>
        {
            Company.MyCompany.AddOffice(office);
            Company.MyCompany.Funds -= office.PurchasePrice;
            foreach(Transform child in OfficesForSaleList)
                if(child.GetComponent<CompanyOfficeItem>().ItemOffice == office)
                    Destroy(child.gameObject);
            RefreshCurrentOfficesList();
            PopulateOfficeDetail(office);
        });
        OfficeSellButton.onClick.AddListener(() =>
        {
            Company.MyCompany.RemoveOffice(office);
            Company.MyCompany.Funds += office.SellPrice;
            RefreshCurrentOfficesList();
            PopulateOfficeDetail(office);
        });

        OfficeAddFeatureButton.gameObject.SetActive(is_company_office && office.RemainingSpace > 0);
        OfficeAddSpaceButton.gameObject.SetActive(is_company_office);
        OfficeBuyButton.gameObject.SetActive(!is_company_office);
        OfficeSellButton.gameObject.SetActive(is_company_office && Company.MyCompany.CompanyOffices.Count > 1);
    }

    public void PopulateEmployeeDetail()
    {
        PopulateEmployeeDetail(
            CurrentEmployeesList.childCount > 0
                ? CurrentEmployeesList.GetChild(0).GetComponent<EmployeeItem>().ItemEmployee
                : AvailableEmployeesList.childCount > 0
                    ? AvailableEmployeesList.GetChild(0).GetComponent<EmployeeItem>().ItemEmployee
                    : null);
    }

    public void PopulateEmployeeDetail(Employee employee)
    {
        if (employee == null) return;
        bool works_for_company = Company.MyCompany.EmployeeList().Contains(employee);

        EmployeeDetailNameAge.text = string.Format(
            "{0} ({1})", employee.Name, employee.Age);
        EmployeeDetailTitle.text = employee.CurrentTitle.Name;

        EmployeeDetailSkillPRG.text = employee.Skills[Skill.Programming].Level.ToString();
        EmployeeDetailSkillUIX.text = employee.Skills[Skill.UserInterfaces].Level.ToString();
        EmployeeDetailSkillDBS.text = employee.Skills[Skill.Databases].Level.ToString();
        EmployeeDetailSkillNTW.text = employee.Skills[Skill.Networking].Level.ToString();
        EmployeeDetailSkillWEB.text = employee.Skills[Skill.WebDevelopment].Level.ToString();

        EmployeeDetailPay.text = employee.Salary.ToString("C0");
        EmployeeDetailHours.text = works_for_company 
            ? employee.WorkingHours.ToString()
            : "0";
        EmployeeDetailHireDate.text = works_for_company 
            ? DateTime.FromBinary(employee.HireDateBinary).ToString("dd/MM/yyyy") 
            : "0";

        EmployeeDetailHours.onEndEdit.RemoveAllListeners();
        EmployeeHireButton.onClick.RemoveAllListeners();
        EmployeeFireButton.onClick.RemoveAllListeners();
        EmployeeTrainButton.onClick.RemoveAllListeners();
        EmployeeMoveButton.onClick.RemoveAllListeners();

        EmployeeDetailHours.onEndEdit.AddListener(x =>
        {
            if (!works_for_company)
                EmployeeDetailHours.text = "0";
            else
                employee.SetWorkingHours(int.Parse(x));

            PopulateEmployeeDetail(employee);
        });
        EmployeeHireButton.onClick.AddListener(() =>
        {
            Company.MyCompany.HireEmployee(employee);
            foreach(Transform child in AvailableEmployeesList)
                if(child.GetComponent<EmployeeItem>().ItemEmployee == employee)
                    Destroy(child.gameObject);
            RefreshCurrentEmployeesList();
            PopulateEmployeeDetail(employee);
        });
        EmployeeFireButton.onClick.AddListener(() =>
        {
            Company.MyCompany.FireEmployee(employee);
            RefreshCurrentEmployeesList();
            PopulateEmployeeDetail(employee);
        });
        EmployeeTrainButton.onClick.AddListener(() =>
        {
            TryTrainEmployee(employee);
        });
        EmployeeMoveButton.onClick.AddListener(() =>
        {
            OpenMoveEmployeePanel(employee);
        });

        EmployeeHireButton.gameObject.SetActive(!works_for_company);
        EmployeeFireButton.gameObject.SetActive(works_for_company);
        EmployeeTrainButton.gameObject.SetActive(works_for_company);
        EmployeeMoveButton.gameObject.SetActive(works_for_company);
    }

    public void PopulateProjectDetail(Project project)
    {
        if (project == null) return;

        if (NewProjectPanel.gameObject.activeSelf)
            NewProjectPanel.gameObject.SetActive(false);
        ProjectDetailPanel.gameObject.SetActive(true);

        ProjectNameText.text = project.Name;
        ProjectStatusText.text = project.ProjectStatus;
        ProjectCurrentPayoutText.text = project.CurrentPayout.ToString("C0");

        ProjectQualityPRGText.text = project.QualityLevels[Skill.Programming].Level.ToString();
        ProjectQualityUIXText.text = project.QualityLevels[Skill.UserInterfaces].Level.ToString();
        ProjectQualityDBSText.text = project.QualityLevels[Skill.Databases].Level.ToString();
        ProjectQualityNTWText.text = project.QualityLevels[Skill.Networking].Level.ToString();
        ProjectQualityWEBText.text = project.QualityLevels[Skill.WebDevelopment].Level.ToString();

        ProjectSetActiveButton.onClick.RemoveAllListeners();
        ProjectSellButton.onClick.RemoveAllListeners();

        ProjectSetActiveButton.onClick.AddListener(() =>
        {
            Company.MyCompany.SetActiveProject(project);
            RefreshProjectsList();
            PopulateProjectDetail(project);
        });
        ProjectSellButton.onClick.AddListener(() =>
        {
            project.CompleteProject();
            Company.MyCompany.SetActiveProject(Company.MyCompany.CompanyProjects.FirstOrDefault(x => x.CurrentStatus == Project.Status.Halted));
            RefreshProjectsList();
            PopulateProjectDetail(project);
        });

        ProjectSetActiveButton.gameObject.SetActive(project != Company.MyCompany.ActiveCompanyProject);
        ProjectSellButton.gameObject.SetActive(
            project.CurrentStatus == Project.Status.InProgress ||
            project.CurrentStatus == Project.Status.Halted);
    }

    public void OpenNewProjectPanel()
    {
        if (ProjectDetailPanel.gameObject.activeSelf)
            ProjectDetailPanel.gameObject.SetActive(false);
        NewProjectPanel.gameObject.SetActive(true);

        NewProjectNameInput.text = string.Empty;
    }

    public void CreateNewProject()
    {
        string new_project_name = NewProjectNameInput.text;
        new_project_name = new_project_name.Trim();
        if (string.IsNullOrEmpty(new_project_name))
            new_project_name = string.Format("Default-{0}", TimeManager.CurrentDate.ToString("ddMMyy"));

        var new_project = new Project(new_project_name);
        Company.MyCompany.CompanyProjects.Add(new_project);

        if(Company.MyCompany.ActiveCompanyProject == null)
        {
            Company.MyCompany.ActiveCompanyProject = new_project;
            new_project.CurrentStatus = Project.Status.InProgress;
        }

        CloseNewProjectPanel();

        RefreshProjectsList();
        PopulateProjectDetail(new_project);
    }

    public void CloseNewProjectPanel()
    {
        NewProjectPanel.gameObject.SetActive(false);
        ProjectDetailPanel.gameObject.SetActive(true);
    }

    public void UpdateNewCompanyCostText()
    {
        int new_office_space;
        if(!int.TryParse(FirstOfficeSpaceInput.text, out new_office_space)) new_office_space = 0;
        new_office_space = Mathf.Clamp(Math.Abs(new_office_space), Office.MIN_OFFICE_SPACE, Office.MAX_OFFICE_SPACE);
        FirstOfficeSpaceInput.text = new_office_space.ToString();

        int total_cost = Company.BASE_COMPANY_COST + (Office.COST_PER_SPACE * new_office_space);

        NewCompanyCostText.text = string.Format("New Company Cost: ${0}", total_cost);
        
        CreateCompanyButton.onClick.RemoveListener(CreateCompany);
        if(total_cost <= Character.MyCharacter.Funds && total_cost != 0)
            CreateCompanyButton.onClick.AddListener(CreateCompany);
        CreateCompanyButton.colors =
            ColorBlocks.GetColorBlock(total_cost <= Character.MyCharacter.Funds ? Color.green : Color.red);
    }

    public void CreateCompany()
    {
        if(Character.MyCharacter.ActiveContract != null)
            Character.MyCharacter.ActiveContract.CancelContract();
        if(Job.MyJob != null)
            Job.MyJob.FirePlayer();
        Company.MyCompany = Company.CreateNewCompany(CompanyNameInput.text, int.Parse(FirstOfficeSpaceInput.text));
        CloseNewCompanyPanel();
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

    public void TryTrainEmployee(Employee employee)
    {
        int skill_sum = Mathf.Clamp(employee.Skills.Sum(), 1, int.MaxValue);
        int training_cost = Company.TRAINING_COST_MULTIPLIER * skill_sum;

        DialogueBox.Instance.CreateYesNoDialogue(
            string.Format("Train {0} for {1:C0}?", employee.Name, training_cost),
            () =>
            {
                Company.MyCompany.TrainEmployee(employee, training_cost);
                DialogueBox.Instance.Cleanup();
            },
            () => DialogueBox.Instance.Cleanup());

        PopulateEmployeeDetail(employee);
    }

    public void OnExpandCurrentOfficesClick()
    {
        currentOfficesSectionOpen = !currentOfficesSectionOpen;
        CurrentOfficesList.gameObject.SetActive(currentOfficesSectionOpen);
        CurrentOfficesExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f,
            currentOfficesSectionOpen ? 0.0f : 180.0f);
    }

    public void OnExpandOfficesForSaleClick()
    {
        officesForSaleSectionOpen = !officesForSaleSectionOpen;
        OfficesForSaleList.gameObject.SetActive(officesForSaleSectionOpen);
        OfficesForSaleExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f,
            officesForSaleSectionOpen ? 0.0f : 180.0f);
    }

    public void OnExpandCurrentEmployeesClick()
    {
        currentEmployeesSectionOpen = !currentEmployeesSectionOpen;
        CurrentEmployeesList.gameObject.SetActive(currentEmployeesSectionOpen);
        CurrentEmployeesExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f,
            currentEmployeesSectionOpen ? 0.0f : 180.0f);
    }

    public void OnExpandAvailableEmployeesClick()
    {
        availableEmployeesSectionOpen = !availableEmployeesSectionOpen;
        AvailableEmployeesList.gameObject.SetActive(availableEmployeesSectionOpen);
        AvailableEmployeesExpandImage.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f,
            availableEmployeesSectionOpen ? 0.0f : 180.0f);
    }

    public void OpenAddFeaturePanel(Office office)
    {
        var available_features = office.AvailableFeatures().ToList();

        if(available_features.Count <= 0) return;

        var features_to_strings = available_features.Select(x => x.Name).ToList();

        FeatureListDropdown.ClearOptions();
        FeatureListDropdown.AddOptions(features_to_strings);
        FeatureListDropdown.value = 0;
        FeatureListDropdown.RefreshShownValue();

        FeatureListDropdown.onValueChanged.RemoveAllListeners();
        FeatureListDropdown.onValueChanged.AddListener(x =>
        {
            FeatureBonusText.text = available_features[x].BonusDescription;
            AddFeatureButton.GetComponentInChildren<Text>().text =
                string.Format("Add Feature ({0:C0} / month)", available_features[x].TotalCost);
        });

        AddFeatureButton.onClick.RemoveAllListeners();
        AddFeatureButton.onClick.AddListener(() =>
        {
            office.AddOfficeFeature(OfficeFeature.AllFeatures.ToList().FindIndex(x => x.Name == FeatureListDropdown.captionText.text));
            RefreshCurrentOfficesList();
            PopulateOfficeDetail(office);
            AddFeaturePanel.alpha = 0;
            AddFeaturePanel.interactable = false;
            AddFeaturePanel.blocksRaycasts = false;
        });

        AddFeaturePanel.alpha = 1;
        AddFeaturePanel.interactable = true;
        AddFeaturePanel.blocksRaycasts = true;
    }

    public void OpenAddSpacePanel(Office office)
    {
        ExtraSpaceInput.text = string.Empty;

        AddSpaceButton.onClick.RemoveAllListeners();
        AddSpaceButton.onClick.AddListener(() =>
        {
            int extra_space = int.Parse(ExtraSpaceInput.text);
            Company.MyCompany.Funds -= office.IncreaseOfficeSpace(extra_space);
            RefreshCurrentOfficesList();
            PopulateOfficeDetail(office);
            AddSpacePanel.alpha = 0;
            AddSpacePanel.interactable = false;
            AddSpacePanel.blocksRaycasts = false;
        });

        AddSpacePanel.alpha = 1;
        AddSpacePanel.interactable = true;
        AddSpacePanel.blocksRaycasts = true;
    }

    public void OpenMoveEmployeePanel(Employee employee)
    {
        var office_list = Company.MyCompany.CompanyOffices.Select(x => x.OfficeLocation.Name).ToList();

        MoveLocationDropdown.ClearOptions();
        MoveLocationDropdown.AddOptions(office_list);
        MoveLocationDropdown.value = 0;
        MoveLocationDropdown.RefreshShownValue();

        MoveLocationDropdown.onValueChanged.RemoveAllListeners();
        MoveLocationDropdown.onValueChanged.AddListener(x =>
        {
            Office current_office = Company.MyCompany.CompanyOffices.Find(e =>
                e.Employees.Contains(employee));
            Office selected_office = Company.MyCompany.CompanyOffices[x];

            int total_cost = COST_PER_MILE_MOVED * 
                Location.GetDistance(current_office.OfficeLocation, selected_office.OfficeLocation);
            MoveEmployeeCostText.text = string.Format("Relocation Cost: {0:C0}", total_cost);
        });

        MoveEmployeeButton.onClick.RemoveAllListeners();
        MoveEmployeeButton.onClick.AddListener(() =>
        {
            Office current_office = Company.MyCompany.CompanyOffices.Find(e =>
                e.Employees.Contains(employee));
            Office selected_office = Company.MyCompany.CompanyOffices[MoveLocationDropdown.value];

            int total_cost = COST_PER_MILE_MOVED * 
                Location.GetDistance(current_office.OfficeLocation, selected_office.OfficeLocation);
            current_office.MoveEmployee(employee, selected_office);
            Company.MyCompany.Funds -= total_cost;

            MoveEmployeePanel.alpha = 0;
            MoveEmployeePanel.interactable = false;
            MoveEmployeePanel.blocksRaycasts = false;
        });

        MoveEmployeePanel.alpha = 1;
        MoveEmployeePanel.interactable = true;
        MoveEmployeePanel.blocksRaycasts = true;
    }
}
