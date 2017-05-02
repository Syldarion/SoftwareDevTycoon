using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformationUIController : MonoBehaviour
{
    public CanvasGroup CharacterInformationPanel;
    public Text NameAgeText;
    public Text LocationText;
    public Text SkillPRGText;
    public Text SkillUIXText;
    public Text SkillDBSText;
    public Text SkillNTWText;
    public Text SkillWEBText;
    public CharacterAvatar CharacterAvatar;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseCharacterInformationForm();
    }

    public void OpenCharacterInformationForm()
    {
        NameAgeText.text = string.Format("{0} ({1})",
            GameManager.ActiveCharacter.Name, GameManager.ActiveCharacter.Age);
        LocationText.text = GameManager.ActiveCharacter.CurrentLocation.Name;
        SkillPRGText.text = GameManager.ActiveCharacter.Skills[Skill.Programming].Level.ToString();
        SkillUIXText.text = GameManager.ActiveCharacter.Skills[Skill.UserInterfaces].Level.ToString();
        SkillDBSText.text = GameManager.ActiveCharacter.Skills[Skill.Databases].Level.ToString();
        SkillNTWText.text = GameManager.ActiveCharacter.Skills[Skill.Networking].Level.ToString();
        SkillWEBText.text = GameManager.ActiveCharacter.Skills[Skill.WebDevelopment].Level.ToString();
        CharacterAvatar.LoadAvatar(GameManager.ActiveCharacter);

        SDTUIController.Instance.OpenCanvas(CharacterInformationPanel, false, false);
    }

    public void CloseCharacterInformationForm()
    {
        SDTUIController.Instance.CloseCanvas(CharacterInformationPanel);
    }
}
