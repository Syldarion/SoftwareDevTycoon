using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyManager : Singleton<CompanyManager>
{
    public InputField CompanyNameInput;
    public InputField FirstOfficeSpaceInput;
    public Button CreateCompanyButton;
    public Text NewCompanyCostText;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {

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
        Company.CreateNewCompany(CompanyNameInput.text, int.Parse(FirstOfficeSpaceInput.text));
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