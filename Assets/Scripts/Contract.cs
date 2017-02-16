using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class Contract
{
    public static Contract ActiveContract;

    public int TotalPointsRemaining { get { return SkillPointsRemaining.Sum(); } }
    public float Progress { get { return (float)TotalPointsRemaining / InitialPointsNeeded; } }

    public string Name;
    //Programming, UserInterfaces, Databases, Networking, Web Development
    public int[] SkillPointsRemaining;
    public int InitialPointsNeeded;
    public int DaysToComplete;
    public int DaysRemaining;
    public int Payment;
    public int ReputationReward;

    public Contract(string name, int[] reqs, int days, int currency, int rep)
    {
        Name = name;
        SkillPointsRemaining = new int[5];
        for (int i = 0; i < 5 && i < reqs.Length; i++)
            SkillPointsRemaining[i] = reqs[i];
        InitialPointsNeeded = SkillPointsRemaining.Sum();
        DaysToComplete = days;
        DaysRemaining = days;
        Payment = currency;
        ReputationReward = rep;
    }

    public void AcceptContract()
    {
        SetActiveContract(this);
    }

    public static void SetActiveContract(Contract contract)
    {
        if (contract != null)
        {
            ActiveContract = contract;
            TimeManager.PerDayEvent.AddListener(WorkContract);
            InformationPanelManager.Instance.ShowActiveContract();
            InformationPanelManager.Instance.UpdateActiveContract();
        }
        else
        {
            TimeManager.PerDayEvent.RemoveListener(WorkContract);
            ActiveContract = null;
            InformationPanelManager.Instance.HideActiveContract();
        }
    }

    public static void WorkContract()
    {
        for (int i = 0; i < ActiveContract.SkillPointsRemaining.Length; i++)
        {
            if (ActiveContract.SkillPointsRemaining[i] <= 0) continue;
            int to_apply = Character.MyCharacter.Skills[i].Level + Random.Range(-1, 2);
            ActiveContract.SkillPointsRemaining[i] = Mathf.Clamp(
                ActiveContract.SkillPointsRemaining[i] - to_apply, 0, int.MaxValue);
            break;
        }

        ActiveContract.DaysRemaining--;

        bool done = !ActiveContract.SkillPointsRemaining.Any(x => x > 0);
        if (done)
        {
            ActiveContract.CompleteContract();
            SetActiveContract(null);
        }
        else if (ActiveContract.DaysRemaining <= 0)
        {
            ActiveContract.CancelContract();
            SetActiveContract(null);
        }

        InformationPanelManager.Instance.UpdateActiveContract();
    }

    public void CompleteContract()
    {
        if (Company.MyCompany == null)
        {
            Character.MyCharacter.AdjustMoney(Payment);
            Character.MyCharacter.Reputation += ReputationReward;
            InformationPanelManager.Instance.DisplayMessage("Completed contract: " + Name, 1.0f);
        }
        else
        {
            Company.MyCompany.AdjustFunds(Payment);
            Company.MyCompany.AdjustReputation(ReputationReward);
        }
    }

    public void CancelContract()
    {
        if (Company.MyCompany == null)
            Character.MyCharacter.Reputation -= Mathf.FloorToInt((float)ReputationReward / 2);
        else
            Company.MyCompany.AdjustReputation(-Mathf.FloorToInt((float)ReputationReward / 2));
        InformationPanelManager.Instance.DisplayMessage("Failed contract: " + Name, 1.0f);
        SetActiveContract(null);
    }

    public static Contract[] GenerateContracts()
    {
        int team_size = Company.MyCompany == null ? 1 : Company.MyCompany.TeamSize;
        int reputation = Company.MyCompany == null ? Character.MyCharacter.Reputation : Company.MyCompany.Reputation;
        reputation = Mathf.Clamp(reputation, 0, 100);

        Contract[] generated_contracts = new Contract[3];

        int char_name_val = Character.MyCharacter.Name.Aggregate(0, (current, c) => current + c);

        Random.InitState(TimeManager.Week * TimeManager.Year * (char_name_val + 1));

        int contract_difficulty = team_size == 1 ? 1 : Mathf.CeilToInt(Mathf.Log(team_size, 2.0f));

        for (int i = 0; i < 3; i++)
        {
            int[] contract_reqs = new int[5];
            int days_to_complete = Random.Range(7, 28);

            int skill_needed = Random.Range(0, 5);
            int skill_needed_val = 0;
            for (int d = 0; d < days_to_complete; d++)
                skill_needed_val += contract_difficulty * Random.Range(3, 8);
            contract_reqs[skill_needed] = skill_needed_val;

            int payout = Mathf.CeilToInt(skill_needed_val * 8 * Random.Range(0.8f, 1.2f));
            if (reputation < 10) payout = Mathf.CeilToInt(payout * 0.5f);
            if (reputation > 90) payout = Mathf.CeilToInt(payout * 2.0f);
            payout = 10 * ((payout + 9) / 10);

            generated_contracts[i] = new Contract(
                "Default",
                contract_reqs,
                days_to_complete,
                payout,
                contract_difficulty * 2);
        }

        return generated_contracts;
    }

    public static bool operator ==(Contract a, Contract b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if ((object)a == null || (object)b == null)
            return false;

        return (a.Name == b.Name) && (a.SkillPointsRemaining == b.SkillPointsRemaining)
               && (a.DaysToComplete == b.DaysToComplete) && (a.Payment == b.Payment)
               && (a.ReputationReward == b.ReputationReward);
    }

    public static bool operator !=(Contract a, Contract b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        Contract contract = obj as Contract;
        if ((object)contract == null)
            return false;

        return (Name == contract.Name) && (SkillPointsRemaining == contract.SkillPointsRemaining)
               && (DaysToComplete == contract.DaysToComplete) && (Payment == contract.Payment)
               && (ReputationReward == contract.ReputationReward);
    }

    public bool Equals(Contract contract)
    {
        if ((object)contract == null)
            return false;

        return (Name == contract.Name) && (SkillPointsRemaining == contract.SkillPointsRemaining)
               && (DaysToComplete == contract.DaysToComplete) && (Payment == contract.Payment)
               && (ReputationReward == contract.ReputationReward);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Name != null ? Name.GetHashCode() : 1);
            hash = hash * 23 + SkillPointsRemaining.GetHashCode();
            hash = hash * 23 + DaysToComplete.GetHashCode();
            hash = hash * 23 + Payment.GetHashCode();
            hash = hash * 23 + ReputationReward.GetHashCode();
            return hash;
        }
    }
}