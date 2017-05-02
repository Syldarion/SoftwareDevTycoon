﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class ContractTemplate
{
    public static ContractTemplate[] Library = {
        new ContractTemplate("Create audio library", new[] {Skill.Programming}),
        new ContractTemplate("Create networking library", new[] {Skill.Programming, Skill.Networking}),
        new ContractTemplate("Cleanup database schema", new[] {Skill.Databases}),
        new ContractTemplate("Create web page", new[] {Skill.UserInterfaces, Skill.WebDevelopment}),
        new ContractTemplate("Create website back-end", new[] {Skill.WebDevelopment}),
    };

    public string ContractName;
    public Skill[] SkillsNeeded;

    public ContractTemplate(string name, Skill[] skills)
    {
        ContractName = name;
        SkillsNeeded = skills;
    }

    public static ContractTemplate GetRandomTemplate()
    {
        return Library[Random.Range(0, Library.Length)];
    }
}

[System.Serializable]
public class Contract
{
    public int TotalPointsRemaining { get { return SkillPointsRemaining.Sum(); } }
    public float Progress { get { return (float)TotalPointsRemaining / InitialPointsNeeded; } }

    public string Name;
    public SkillList SkillPointsRemaining;
    public int InitialPointsNeeded;
    public int DaysToComplete;
    public int DaysRemaining;
    public int Payment;
    public int ReputationReward;
    public bool Negotiated;
    public bool SuccessfulNegotiation;

    public Contract(string name, SkillList reqs, int days, int currency, int rep)
    {
        Name = name;
        SkillPointsRemaining = reqs;
        InitialPointsNeeded = SkillPointsRemaining.Sum();
        DaysToComplete = days;
        DaysRemaining = days;
        Payment = currency;
        ReputationReward = rep;
        Negotiated = false;
        SuccessfulNegotiation = false;
    }

    public void AcceptContract()
    {
        SetCompanyActiveContract(this);

        if (!TutorialManager.Instance.FirstContractStartedMessageFired)
        {
            DialogueManager.Instance.CreateMessageDialogue(
                DialogueMessage.FirstContractStarted,
                new Vector3(100.0f, 200.0f),
                () => { });
            TutorialManager.Instance.FirstContractStartedMessageFired = true;
        }
    }

    public bool ApplyWork(SkillList work)
    {
        SkillPointsRemaining -= work;
        DaysRemaining--;

        InformationPanelManager.Instance.UpdateActiveContract();
        
        if(SkillPointsRemaining.IsEmpty)
        {
            CompleteContract();
            return true;
        }
        if(DaysRemaining <= 0)
        {
            CancelContract();
            return true;
        }

        return false;
    }

    public static void SetCompanyActiveContract(Contract contract)
    {
        Company.MyCompany.ActiveContract = contract;
        if(contract != null)
        {
            InformationPanelManager.Instance.ShowActiveContract();
            InformationPanelManager.Instance.UpdateActiveContract();
        }
        else
        {
            InformationPanelManager.Instance.HideActiveContract();
        }
    }

    public void CompleteContract()
    {
        Company.MyCompany.Funds += Payment;
        Company.MyCompany.Reputation += ReputationReward;
        ReportManager.Instance.ContractEarnings += Payment;

        InformationPanelManager.Instance.DisplayMessage("Completed contract: " + Name, 1.0f);

        if (!TutorialManager.Instance.FirstContractFinishedMessageFired)
        {
            DialogueManager.Instance.CreateMessageDialogue(
                DialogueMessage.FirstContractCompleted,
                new Vector3(100.0f, 200.0f),
                () => { });
            TutorialManager.Instance.FirstContractFinishedMessageFired = true;
        }
    }

    public void CancelContract()
    {
        Company.MyCompany.Reputation -= Mathf.FloorToInt((float)ReputationReward / 2);
        InformationPanelManager.Instance.DisplayMessage("Failed contract: " + Name, 1.0f);
        SetCompanyActiveContract(null);
    }

    public static Contract[] GenerateContracts(int contracts)
    {
        int team_size = 1 + Company.MyCompany.TeamSize;
        int reputation = Company.MyCompany.Reputation;

        Contract[] generated_contracts = new Contract[contracts];

        int char_name_val = GameManager.ActiveCharacter.Name.Aggregate(0, (current, c) => current + c);

        Random.InitState(TimeManager.Week * TimeManager.Year * (char_name_val + 1));

        int contract_difficulty = team_size == 1 ? 1 : Mathf.CeilToInt(Mathf.Log(team_size, 2.0f));
        Debug.Log(contract_difficulty);

        for (int i = 0; i < generated_contracts.Length; i++)
        {
            int days_to_complete = Random.Range(7, 28);
            var template = ContractTemplate.GetRandomTemplate();
            SkillLevel[] skills_needed = new SkillLevel[template.SkillsNeeded.Length];

            for(int j = 0; j < skills_needed.Length; j++)
            {
                int skill_sum = 0;
                for(int d = 0; d < days_to_complete; d++)
                    skill_sum += contract_difficulty * Random.Range(2, 8);
                skills_needed[j] = new SkillLevel(template.SkillsNeeded[j], skill_sum);
            }
            
            var contract_reqs = new SkillList(skills_needed);

            int payout = Mathf.CeilToInt(skills_needed.Sum(x => x.Level) * 8 * Random.Range(0.8f, 1.2f));
            if (reputation < 10) payout = Mathf.CeilToInt(payout * 0.5f);
            if (reputation > 90) payout = Mathf.CeilToInt(payout * 2.0f);

            generated_contracts[i] = new Contract(
                template.ContractName,
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

    protected bool Equals(Contract other)
    {
        return string.Equals(Name, other.Name) && 
            Equals(SkillPointsRemaining, other.SkillPointsRemaining) && 
            InitialPointsNeeded == other.InitialPointsNeeded && 
            DaysToComplete == other.DaysToComplete && 
            DaysRemaining == other.DaysRemaining && 
            Payment == other.Payment && 
            ReputationReward == other.ReputationReward;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Contract)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash_code = (Name != null ? Name.GetHashCode() : 0);
            hash_code = (hash_code * 397) ^ (SkillPointsRemaining != null ? SkillPointsRemaining.GetHashCode() : 0);
            hash_code = (hash_code * 397) ^ InitialPointsNeeded;
            hash_code = (hash_code * 397) ^ DaysToComplete;
            hash_code = (hash_code * 397) ^ DaysRemaining;
            hash_code = (hash_code * 397) ^ Payment;
            hash_code = (hash_code * 397) ^ ReputationReward;
            return hash_code;
        }
    }
}
