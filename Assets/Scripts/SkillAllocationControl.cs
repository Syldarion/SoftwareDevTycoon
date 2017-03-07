using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillAllocationControl : MonoBehaviour
{
    public Skill SkillType;
    public InputField SkillLevelInput;
    public Text RemainingPointsText;

    public static int RemainingPoints = 15;
    
    public int CurrentSkillLevel;

    private const int SKILL_MIN_LEVEL = 1;
    private const int SKILL_MAX_LEVEL = 99;

    void Awake()
    {
        CurrentSkillLevel = 1;
    }

    void Start()
    {
        UpdateSkillInfo();
    }

    void Update()
    {

    }

    public void ModifySkillLevel(string value)
    {
        int new_value = int.Parse(value);
        int difference = new_value - CurrentSkillLevel;
        if (new_value < SKILL_MIN_LEVEL || new_value > SKILL_MAX_LEVEL)
        {
            UpdateSkillInfo();
            return;
        }

        if(difference > RemainingPoints)
            difference = RemainingPoints;

        RemainingPoints -= difference;
        CurrentSkillLevel += difference;
        UpdateSkillInfo();
    }

    public void UpdateSkillInfo()
    {
        SkillLevelInput.text = CurrentSkillLevel.ToString();
        RemainingPointsText.text = string.Format("Remaining: {0}", RemainingPoints);
    }
}
