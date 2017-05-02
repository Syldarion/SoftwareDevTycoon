using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : Singleton<TrainingManager>
{
    public const int TRAINING_COST_MULTIPLIER = 500;

    public CanvasGroup TrainingPanel;
    public Button[] TrainingButtons;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseTrainingPanel();
    }

    public void OpenTrainingPanel()
    {
        for(int i = 0; i < SkillInfo.COUNT && i < TrainingButtons.Length; i++)
        {
            int training_cost = GameManager.ActiveCharacter.Skills[i].Level * 
                TRAINING_COST_MULTIPLIER;
            TrainingButtons[i].onClick.RemoveAllListeners();
            int skill_index = i;
            TrainingButtons[i].onClick.AddListener(() => TrainCharacter((Skill)skill_index, training_cost));
            TrainingButtons[i].GetComponentInChildren<Text>().text = training_cost.ToString("C0");
        }

        SDTUIController.Instance.OpenCanvas(TrainingPanel);
    }

    public void CloseTrainingPanel()
    {
        SDTUIController.Instance.CloseCanvas(TrainingPanel);
    }

    public void TrainCharacter(Skill trainingSkill, int trainingCost)
    {
        Company.MyCompany.Funds -= trainingCost;

        SkillList training_increase = new SkillList();

        int current_level = GameManager.ActiveCharacter.Skills[trainingSkill].Level;
        int increase = Mathf.CeilToInt(current_level * Random.Range(0.25f, 0.5f));

        training_increase[trainingSkill].Level = increase;

        //residual training increases
        for(int i = 0; i < SkillInfo.COUNT; i++)
        {
            int residual_increase = Mathf.FloorToInt(increase * Random.Range(0.1f, 0.3f));
            training_increase[i].Level += residual_increase;
        }

        GameManager.ActiveCharacter.Skills += training_increase;

        CloseTrainingPanel();

        DialogueManager.Instance.CreateMessageDialogue(
            string.Format("Training Results\n\n{0}", training_increase.InfoLines(20, false)),
            Vector3.zero,
            () => { });
    }
}
