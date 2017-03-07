using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAvatar : MonoBehaviour
{
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
        AvatarHead.color = color;
    }

    public void ChangeBodyColor(Color color)
    {
        AvatarMaleBody.color = color;
        AvatarFemaleBody.color = color;
    }

    public void ChangeLegsColor(Color color)
    {
        AvatarLegs.color = color;
    }

    public void ToggleGender()
    {
        bool is_male = AvatarMaleBody.gameObject.activeSelf;
        AvatarMaleBody.gameObject.SetActive(!is_male);
        AvatarFemaleBody.gameObject.SetActive(is_male);
    }
}
