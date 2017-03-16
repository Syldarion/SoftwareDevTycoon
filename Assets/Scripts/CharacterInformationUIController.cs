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
    public Text TitleText;
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
            Character.MyCharacter.Name, Character.MyCharacter.Age);
        LocationText.text = Character.MyCharacter.CurrentLocation.Name;
        TitleText.text = Company.MyCompany == null
            ? "Freelancer"
            : string.Format("Founder of {0}", Company.MyCompany.Name);
        SkillPRGText.text = Character.MyCharacter.Skills[Skill.Programming].Level.ToString();
        SkillUIXText.text = Character.MyCharacter.Skills[Skill.UserInterfaces].Level.ToString();
        SkillDBSText.text = Character.MyCharacter.Skills[Skill.Databases].Level.ToString();
        SkillNTWText.text = Character.MyCharacter.Skills[Skill.Networking].Level.ToString();
        SkillWEBText.text = Character.MyCharacter.Skills[Skill.WebDevelopment].Level.ToString();
        CharacterAvatar.LoadAvatar(Character.MyCharacter);

        SDTUIController.Instance.OpenCanvas(CharacterInformationPanel, false, false);
    }

    public void CloseCharacterInformationForm()
    {
        SDTUIController.Instance.CloseCanvas(CharacterInformationPanel);
    }
}
