using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boo.Lang.Environments;
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

    [Header("Office Detail UI")]
    public Text OfficeDetailLocation;
    public Text OfficeDetailUpkeepCost;
    public Text OfficeDetailBonuses;
    public ProgressBar OfficeDetailRemainingSpace;
    public Button OfficeAddFeatureButton;
    public Button OfficeAddSpaceButton;
    public Button OfficeSellButton;
    public Button OfficeBuyButton;

    [Header("Employee Detail UI")]
    public Text EmployeeDetailNameAge;
    public Image EmployeeDetailGender;
    public Text EmployeeDetailSkillLevels;
    public Text EmployeeDetailTitle;
    public Text EmployeeDetailPay;
    public Text EmployeeDetailHireDate;
    public Text EmployeeDetailPerformance;
    public Button EmployeeHireButton;
    public Button EmployeeFireButton;
    public Button EmployeeTrainButton;
    public Button EmployeeMoveButton;

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

    public void RefreshCurrentOfficesList()
    {
        foreach (Transform child in CurrentOfficesList)
            Destroy(child.gameObject);
        foreach (Office office in Company.MyCompany.CompanyOffices)
        {
            var new_office_item = Instantiate(CompanyOfficeItemPrefab);
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
            var new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(Office.GenerateOffice());
            new_office_item.transform.SetParent(OfficesForSaleList, false);
        }

        PopulateOfficeDetail(Company.MyCompany.CompanyOffices[0]);
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

    public void RefreshCurrentEmployeesList()
    {
        foreach (Transform child in CurrentEmployeesList)
            Destroy(child.gameObject);
        foreach (Employee employee in Company.MyCompany.EmployeeList())
        {
            var new_employee_item = Instantiate(EmployeeItemPrefab);
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
            var new_employee_item = Instantiate(EmployeeItemPrefab);
            new_employee_item.PopulateData(Employee.GenerateEmployee());
            new_employee_item.transform.SetParent(AvailableEmployeesList, false);
        }

        PopulateEmployeeDetail();
    }

    public void PopulateOfficeDetail(Office office)
    {
        if (office == null) return;
        bool is_company_office = Company.MyCompany.CompanyOffices.Contains(office);

        OfficeDetailLocation.text = string.Format("Office Location\n{0}", office.OfficeLocation.Name);
        OfficeDetailUpkeepCost.text = string.Format("Upkeep: ${0}", office.TotalUpkeepCost);
        StringBuilder bonus_string_builder = new StringBuilder("Office Bonuses\n");
        for(int i = 0; i < office.QualityBonuses.Length; i++)
            if(office.QualityBonuses[i] > 0.0f || office.QualityBonuses[i] < 0.0f)
                bonus_string_builder.AppendLine(string.Format("{0:P} {1} quality bonus", office.QualityBonuses[i],
                    SkillInfo.SKILL_ABBR[i]));
        if(office.MoraleModifier > 0.0f || office.MoraleModifier < 0.0f)
            bonus_string_builder.AppendLine(string.Format("{0:P} employee morale", office.MoraleModifier));
        if(office.SalesModifier > 0.0f || office.SalesModifier < 0.0f)
            bonus_string_builder.AppendLine(string.Format("{0:P} sales bonus", office.SalesModifier));
        OfficeDetailBonuses.text = bonus_string_builder.ToString();
        float space_percentage = (float)office.RemainingSpace / office.Space;
        string space_string = string.Format("{0} / {1}", office.RemainingSpace, office.Space);
        OfficeDetailRemainingSpace.SetProgress(space_percentage);
        OfficeDetailRemainingSpace.SetBarText(space_string, true);

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
        OfficeBuyButton.GetComponentInChildren<Text>().text = string.Format("Buy Office: ${0}", office.PurchasePrice);
        OfficeBuyButton.onClick.AddListener(() =>
        {
            Company.MyCompany.AddOffice(office);
            Company.MyCompany.AdjustFunds(-office.PurchasePrice);
            foreach(Transform child in OfficesForSaleList)
                if(child.GetComponent<CompanyOfficeItem>().ItemOffice == office)
                    Destroy(child.gameObject);
            RefreshCurrentOfficesList();
            PopulateOfficeDetail(office);
        });
        OfficeSellButton.GetComponentInChildren<Text>().text = string.Format("Sell Office: ${0}", office.SellPrice);
        OfficeSellButton.onClick.AddListener(() =>
        {
            Company.MyCompany.RemoveOffice(office);
            Company.MyCompany.AdjustFunds(office.SellPrice);
            RefreshCurrentOfficesList();
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
        //set employee gender image after you make images for the slot
        EmployeeDetailSkillLevels.text = string.Join("\n",
            employee.Skills.Skills.Select(x => {
                StringBuilder skill_builder = new StringBuilder();
                skill_builder.Append(SkillInfo.SKILL_NAME[(int)x.Skill]);
                int spaces = 18 - SkillInfo.SKILL_NAME[(int)x.Skill].Length;
                skill_builder.Append(' ', spaces);
                skill_builder.Append(x.Level);
                return skill_builder.ToString();
            }).ToArray());
        EmployeeDetailTitle.text = employee.CurrentTitle.Name;
        EmployeeDetailPay.text = employee.Salary.ToString("C");
        EmployeeDetailHireDate.text = works_for_company
                                          ? DateTime.FromBinary(employee.HireDateBinary).ToLongDateString()
                                          : string.Empty;
        //EmployeeDetailPerformance.text
        EmployeeHireButton.onClick.RemoveAllListeners();
        EmployeeFireButton.onClick.RemoveAllListeners();
        EmployeeTrainButton.onClick.RemoveAllListeners();
        EmployeeMoveButton.onClick.RemoveAllListeners();

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
        });
        EmployeeTrainButton.onClick.AddListener(() =>
        {
            Company.MyCompany.TrainEmployee(employee);
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

    public void UpdateNewCompanyCostText()
    {
        int new_office_space;
        if(!int.TryParse(FirstOfficeSpaceInput.text, out new_office_space)) new_office_space = 0;
        new_office_space = Mathf.Clamp(Math.Abs(new_office_space), Office.MIN_OFFICE_SPACE, Office.MAX_OFFICE_SPACE);
        FirstOfficeSpaceInput.text = new_office_space.ToString();

        int total_cost = Company.BASE_COMPANY_COST + (Office.COST_PER_SPACE * new_office_space);

        NewCompanyCostText.text = string.Format("New Company Cost: ${0}", total_cost);
        
        CreateCompanyButton.onClick.RemoveListener(CreateCompany);
        if(total_cost <= Character.MyCharacter.Money && total_cost != 0)
            CreateCompanyButton.onClick.AddListener(CreateCompany);
        CreateCompanyButton.colors =
            ColorBlocks.GetColorBlock(total_cost <= Character.MyCharacter.Money ? Color.green : Color.red);
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
        List<OfficeFeature> available_features = office.AvailableFeatures().ToList();

        if(available_features.Count <= 0) return;

        List<string> features_to_strings =
            available_features.Select(x => x.Name).ToList();

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
            Company.MyCompany.AdjustFunds(-office.IncreaseOfficeSpace(extra_space));
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
        List<string> office_list = Company.MyCompany.CompanyOffices.Select(x => x.OfficeLocation.Name).ToList();

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
            Company.MyCompany.AdjustFunds(-total_cost);

            MoveEmployeePanel.alpha = 0;
            MoveEmployeePanel.interactable = false;
            MoveEmployeePanel.blocksRaycasts = false;
        });

        MoveEmployeePanel.alpha = 1;
        MoveEmployeePanel.interactable = true;
        MoveEmployeePanel.blocksRaycasts = true;
    }
}
