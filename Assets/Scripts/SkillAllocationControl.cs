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

    private const int skill_min_level = 1;
    private const int skill_max_level = 99;

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
        if (new_value < skill_min_level || new_value > skill_max_level)
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
