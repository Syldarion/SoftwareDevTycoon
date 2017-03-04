using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngineInternal;

public class ContractObject : MonoBehaviour
{
    public Text NameText;
    public Text[] SkillsTexts;
    public Text PayText;
    public Text DeadlineText;

    public InputField PayInput;
    public InputField DeadlineInput;

    public Button NegotiateButton;
    public Button AcceptButton;
    public Button CancelButton;

    public Contract ObjectContract;

    private bool negotiating;

    void Awake()
    {
        negotiating = false;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void PopulateContractInfo(Contract contract)
    {
        ObjectContract = contract;

        NameText.text = contract.Name;

        for (int i = 0; i < contract.SkillPointsRemaining.Length; i++)
        {
            SkillsTexts[(int)contract.SkillPointsRemaining[i].Skill].transform.parent.gameObject.SetActive(true);
            SkillsTexts[(int)contract.SkillPointsRemaining[i].Skill].text = contract.SkillPointsRemaining[i].Level.ToString();
        }

        PayText.text = contract.Payment.ToString("C");
        DeadlineText.text = string.Format("{0} days", contract.DaysToComplete);
    }

    public void OnAcceptClick()
    {
        ObjectContract.AcceptContract();
        ContractManager.Instance.CloseContractForm();
    }

    public void OnCancelClick()
    {
        negotiating = false;

        PayInput.gameObject.SetActive(false);
        DeadlineInput.gameObject.SetActive(false);
        AcceptButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(false);

        NegotiateButton.GetComponentInChildren<Text>().text = "Negotiate";
    }

    public void OnNegotiateClick()
    {
        if (negotiating)
        {
            int acceptance_chance = GetNegotiationAcceptanceChance();
            int roll = Random.Range(0, 100) + 1;
            bool success = roll <= acceptance_chance;

            if (success)
            {
                ObjectContract.Payment = PayInput.text != string.Empty
                    ? int.Parse(PayInput.text)
                    : ObjectContract.Payment;
                ObjectContract.DaysToComplete =
                    ObjectContract.DaysRemaining = DeadlineInput.text != string.Empty
                        ? int.Parse(DeadlineInput.text)
                        : ObjectContract.DaysToComplete;

                PopulateContractInfo(ObjectContract);
            }

            NegotiateButton.GetComponentInChildren<Text>().text = success ? "Success" : "Failed";
            NegotiateButton.image.color = success ? Color.green : Color.red;
            NegotiateButton.interactable = false;
        }
        else
        {
            PayInput.text = ObjectContract.Payment.ToString();
            DeadlineInput.text = ObjectContract.DaysToComplete.ToString();
        }

        negotiating = !negotiating;

        PayInput.gameObject.SetActive(negotiating);
        DeadlineInput.gameObject.SetActive(negotiating);
        AcceptButton.gameObject.SetActive(!negotiating);
        CancelButton.gameObject.SetActive(negotiating);
    }

    public void OnNegotiateChange()
    {
        NegotiateButton.GetComponentInChildren<Text>().text =
            string.Format("Negotiate: {0}%", GetNegotiationAcceptanceChance());
    }

    public int GetNegotiationAcceptanceChance()
    {
        int proposed_pay = PayInput.text != string.Empty
                               ? int.Parse(PayInput.text)
                               : ObjectContract.Payment;
        int proposed_days = DeadlineInput.text != string.Empty
                                ? int.Parse(DeadlineInput.text)
                                : ObjectContract.DaysToComplete;
        int reputation = Company.MyCompany == null ? Character.MyCharacter.Reputation : Company.MyCompany.Reputation;

        float new_pay_percentage = (float)proposed_pay / ObjectContract.Payment;
        float new_day_percentage = (float)proposed_days / ObjectContract.DaysToComplete;

        float pay_percentage_change = new_pay_percentage - 1.0f;
        float day_percentage_change = new_day_percentage - 1.0f;

        if (pay_percentage_change > 0.0f) pay_percentage_change *= (0.5f + (float)reputation / 100);
        else pay_percentage_change *= (1.5f - (float)reputation / 100);
        if (day_percentage_change > 0.0f) day_percentage_change *= (0.5f + (float)reputation / 100);
        else day_percentage_change *= (1.5f - (float)reputation / 100);

        float weighted_value = (pay_percentage_change * 0.75f) + (day_percentage_change * 0.25f);

        return Mathf.CeilToInt(Mathf.Clamp(100.0f - (weighted_value * 100.0f), 0.0f, 100.0f));
    }
}
