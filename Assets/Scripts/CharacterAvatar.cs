using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAvatar : MonoBehaviour
{
    public bool Editable;
    public Color[] AvatarColors;

    public Image AvatarHead;
    public Image AvatarMaleBody;
    public Image AvatarFemaleBody;
    public Image AvatarLegs;

    private int headColorIndex;
    private int bodyColorIndex;
    private int legsColorIndex;

    void Awake()
    {
        headColorIndex = 0;
        bodyColorIndex = 0;
        legsColorIndex = 0;
        ChangeHeadColor(AvatarColors[headColorIndex]);
        ChangeBodyColor(AvatarColors[bodyColorIndex]);
        ChangeLegsColor(AvatarColors[legsColorIndex]);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadAvatar(Person person)
    {
        AvatarHead.color = person.GetHeadColor();
        AvatarMaleBody.color = person.GetBodyColor();
        AvatarFemaleBody.color = person.GetBodyColor();
        AvatarLegs.color = person.GetLegsColor();
        AvatarMaleBody.gameObject.SetActive(person.IsMale);
        AvatarFemaleBody.gameObject.SetActive(!person.IsMale);
    }

    public void NextHeadColor()
    {
        headColorIndex++;
        if(headColorIndex >= AvatarColors.Length)
            headColorIndex = 0;
        ChangeHeadColor(AvatarColors[headColorIndex]);
    }

    public void NextBodyColor()
    {
        bodyColorIndex++;
        if(bodyColorIndex >= AvatarColors.Length)
        {
            ToggleGender();
            bodyColorIndex = 0;
        }
        ChangeBodyColor(AvatarColors[bodyColorIndex]);
    }

    public void NextLegsColor()
    {
        legsColorIndex++;
        if(legsColorIndex >= AvatarColors.Length)
            legsColorIndex = 0;
        ChangeLegsColor(AvatarColors[legsColorIndex]);
    }

    public void ChangeHeadColor(Color color)
    {
        if (!Editable) return;

        AvatarHead.color = color;
    }

    public void ChangeBodyColor(Color color)
    {
        if (!Editable) return;

        AvatarMaleBody.color = color;
        AvatarFemaleBody.color = color;
    }

    public void ChangeLegsColor(Color color)
    {
        if (!Editable) return;

        AvatarLegs.color = color;
    }

    public void ToggleGender()
    {
        bool is_male = AvatarMaleBody.gameObject.activeSelf;
        AvatarMaleBody.gameObject.SetActive(!is_male);
        AvatarFemaleBody.gameObject.SetActive(is_male);
    }
}
