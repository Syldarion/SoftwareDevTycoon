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
    public Text OfficeDetailBuildingCount;
    public Text OfficeDetailUpkeepCost;
    public Text OfficeDetailBonuses;
    public ProgressBar OfficeDetailRemainingSpace;
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
    
    private Office selectedOffice;
    private Employee selectedEmployee;

    void Start()
    {
        Instance = this;
        selectedOffice = null;
        selectedEmployee = null;
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

    public void UpdateCompanyInfoPanel()
    {
        if (Company.MyCompany == null)
            return;

        InfoCompanyName.text = Company.MyCompany.Name;
        InfoOfficeCount.text = Company.MyCompany.CompanyOffices.Count.ToString();
        InfoEmployeeCount.text = Company.MyCompany.TeamSize.ToString();
    }

    public void OpenNewCompanyPanel()
    {
        SDTUIController.Instance.OpenCanvas(NewCompanyPanel);
    }

    public void CloseNewCompanyPanel()
    {
        SDTUIController.Instance.CloseCanvas(NewCompanyPanel);
    }

    public void OpenCompanyOfficesPanel()
    {
        foreach (Transform child in OfficeList)
            Destroy(child.gameObject);
        foreach (Office office in Company.MyCompany.CompanyOffices)
        {
            var new_office_item = Instantiate(CompanyOfficeItemPrefab);
            new_office_item.PopulateData(office);
            new_office_item.transform.SetParent(OfficeList, false);
        }

        SDTUIController.Instance.OpenCanvas(OfficesPanel);
    }

    public void CloseCompanyOfficesPanel()
    {
        SDTUIController.Instance.CloseCanvas(OfficesPanel);
    }

    public void OpenCompanyEmployeesPanel()
    {
        foreach (Transform child in EmployeeList)
            Destroy(child.gameObject);
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

        OfficeDetailLocation.text = string.Format("Office Location\n{0}", office.OfficeLocation.Name);
        OfficeDetailBuildingCount.text = office.Buildings.Count.ToString();
        OfficeDetailUpkeepCost.text = office.TotalUpkeepCost.ToString("C");
        //office building bonuses, once they're implemented

        float space_percentage = (float)office.RemainingSpace / office.Space;
        string space_string = string.Format("{0} / {1}", office.RemainingSpace, office.Space);
        OfficeDetailRemainingSpace.SetProgress(space_percentage);
        OfficeDetailRemainingSpace.SetBarText(space_string, true);

        OfficeAddBuildingButton.gameObject.SetActive(office.RemainingSpace > 0);
        OfficeBuyButton.GetComponentInChildren<Text>().text = string.Format("Buy Office: ${0}", office.PurchasePrice);
        OfficeBuyButton.gameObject.SetActive(!Company.MyCompany.CompanyOffices.Contains(office));
        OfficeSellButton.GetComponentInChildren<Text>().text = string.Format("Sell Office: ${0}", office.SellPrice);
        OfficeSellButton.gameObject.SetActive(Company.MyCompany.CompanyOffices.Contains(office) &&
                                        Company.MyCompany.CompanyOffices.Count > 1);
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
        int new_office_space;
        if(!int.TryParse(FirstOfficeSpaceInput.text, out new_office_space)) new_office_space = 0;
        new_office_space = Math.Abs(new_office_space);

        int total_cost = Company.BASE_COMPANY_COST + (Office.COST_PER_SPACE * new_office_space);

        NewCompanyCostText.text = string.Format("New Company Cost: ${0}", total_cost);

        Debug.Log(string.Format("Player: {0}, Company: {1}", Character.MyCharacter.Money, total_cost));

        CreateCompanyButton.onClick.RemoveListener(CreateCompany);
        if(total_cost <= Character.MyCharacter.Money && total_cost != 0)
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