using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillAllocationControl : MonoBehaviour
{
    public Skill SkillType;
    public Text SkillNameText;
    public RectTransform SkillLevelBarTransform;
    public Text SkillLevelText;
    public Vector2 MaxBarSize;
    public Text RemainingPointsText;

    public static int RemainingPoints = 15;
    
    public int CurrentSkillLevel;

    private const int SKILL_MIN_LEVEL = 1;
    private const int SKILL_MAX_LEVEL = 10;

    void Awake()
    {
        SkillNameText.text = SkillInfo.SKILL_NAME[(int)SkillType];
        CurrentSkillLevel = 1;

        MaxBarSize = SkillLevelBarTransform.sizeDelta;
    }

    void Start()
    {
        RemainingPointsText.text = string.Format("Remaining: {0}", RemainingPoints);
        SkillLevelBarTransform.sizeDelta = new Vector2(0.1f * MaxBarSize.x, 0.0f);
        SkillLevelText.text = CurrentSkillLevel.ToString();
    }

    void Update()
    {

    }

    public void ModifySkillLevel(int modifier)
    {
        if ((RemainingPoints <= 0 && modifier > 0) || 
            (CurrentSkillLevel >= 10 && modifier > 0) || 
            (CurrentSkillLevel <= 1 && modifier < 0)) return;

        RemainingPoints -= modifier;
        RemainingPointsText.text = string.Format("Remaining: {0}", RemainingPoints);

        CurrentSkillLevel = Mathf.Clamp(CurrentSkillLevel + modifier, SKILL_MIN_LEVEL, SKILL_MAX_LEVEL);
        SkillLevelBarTransform.sizeDelta = new Vector2(CurrentSkillLevel * 0.1f * MaxBarSize.x, 0.0f);
        SkillLevelText.text = CurrentSkillLevel.ToString();
    }
}