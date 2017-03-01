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
    public RectTransform OfficeList;
    public CompanyOfficeItem CompanyOfficeItemPrefab;

    [Header("Company Employees UI")]
    public CanvasGroup EmployeesPanel;
    public RectTransform EmployeeList;
    public EmployeeItem EmployeeItemPrefab;

    [Header("Office Detail UI")]
    public Text OfficeDetailLocation;
    public Text OfficeDetailUpkeepCost;
    public Text OfficeDetailBonuses;
    public ProgressBar OfficeDetailRemainingSpace;
    public Button OfficeAddFeatureButton;
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

        foreach (Transform child in OfficeList)
            Destroy(child.gameObject);
        for(int i = 0; i < OFFICES_TO_GENERATE; i++)
        {
            var new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(Office.GenerateOffice());
            new_office_item.transform.SetParent(OfficeList, false);
        }
        foreach (Office office in Company.MyCompany.CompanyOffices)
        {
            var new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(office);
            new_office_item.transform.SetParent(OfficeList, false);
        }

        PopulateOfficeDetail(Company.MyCompany.CompanyOffices[0]);

        SDTUIController.Instance.OpenCanvas(OfficesPanel);
    }

    public void CloseCompanyOfficesPanel()
    {
        SDTUIController.Instance.CloseCanvas(OfficesPanel);
    }

    public void OpenCompanyEmployeesPanel()
    {
        if (Company.MyCompany == null) return;

        foreach (Transform child in EmployeeList)
            Destroy(child.gameObject);
        for(int i = 0; i < EMPLOYEES_TO_GENERATE; i++)
        {
            var new_employee_item = Instantiate(EmployeeItemPrefab);
            new_employee_item.PopulateData(Employee.GenerateEmployee());
            new_employee_item.transform.SetParent(EmployeeList, false);
        }
        foreach (Employee employee in Company.MyCompany.EmployeeList())
        {
            var new_employee_item = Instantiate(EmployeeItemPrefab);
            new_employee_item.PopulateData(employee);
            new_employee_item.transform.SetParent(EmployeeList, false);
        }

        SDTUIController.Instance.OpenCanvas(EmployeesPanel);
    }

    public void CloseCompanyEmployeesPanel()
    {
        SDTUIController.Instance.CloseCanvas(EmployeesPanel);
    }

    public void PopulateOfficeDetail(Office office)
    {
        if (office == null) return;
        bool is_company_office = Company.MyCompany.CompanyOffices.Contains(office);

        OfficeDetailLocation.text = string.Format("Office Location\n{0}", office.OfficeLocation.Name);
        OfficeDetailUpkeepCost.text = string.Format("Upkeep: ${0}", office.TotalUpkeepCost);
        StringBuilder bonus_string_builder = new StringBuilder("Office Bonuses\n");
        foreach(OfficeFeature feature in office.Features)
            bonus_string_builder.AppendLine(feature.BonusDescription);
        OfficeDetailBonuses.text = bonus_string_builder.ToString();
        float space_percentage = (float)office.RemainingSpace / office.Space;
        string space_string = string.Format("{0} / {1}", office.RemainingSpace, office.Space);
        OfficeDetailRemainingSpace.SetProgress(space_percentage);
        OfficeDetailRemainingSpace.SetBarText(space_string, true);

        OfficeAddFeatureButton.gameObject.SetActive(is_company_office && office.RemainingSpace > 0);
        OfficeBuyButton.GetComponentInChildren<Text>().text = string.Format("Buy Office: ${0}", office.PurchasePrice);
        OfficeBuyButton.gameObject.SetActive(!is_company_office);
        OfficeSellButton.GetComponentInChildren<Text>().text = string.Format("Sell Office: ${0}", office.SellPrice);
        OfficeSellButton.gameObject.SetActive(is_company_office && Company.MyCompany.CompanyOffices.Count > 1);
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
        EmployeeDetailPay.text = employee.Pay.ToString("C");
        EmployeeDetailHireDate.text = works_for_company
                                          ? DateTime.FromBinary(employee.HireDateBinary).ToLongDateString()
                                          : "N/A";
        //EmployeeDetailPerformance.text
        EmployeeHireButton.onClick.AddListener(() => Company.MyCompany.HireEmployee(employee));
        EmployeeHireButton.gameObject.SetActive(!works_for_company);
        EmployeeFireButton.onClick.AddListener(() => Company.MyCompany.FireEmployee(employee));
        EmployeeFireButton.gameObject.SetActive(works_for_company);
        EmployeeTrainButton.onClick.AddListener(() => Company.MyCompany.TrainEmployee(employee));
        EmployeeTrainButton.gameObject.SetActive(works_for_company);
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
}