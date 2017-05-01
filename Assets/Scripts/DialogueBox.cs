using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public Text MessageText;
    public Button[] ActionButtons;

    void Start()
    {

    }

    void Update()
    {

    }
    
    public void CloseDialogueBox()
    {
        Destroy(gameObject);
    }
}
