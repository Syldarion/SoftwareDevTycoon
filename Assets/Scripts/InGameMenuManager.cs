using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

public class InGameMenuManager : MonoBehaviour
{
    public CanvasGroup MenuOptionsPanel;

    private bool open;

    void Awake()
    {
        open = false;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ToggleMenu()
    {
        open = !open;

        if(open)
            UIUtilities.ActivateCanvasGroup(MenuOptionsPanel);
        else
            UIUtilities.DeactivateCanvasGroup(MenuOptionsPanel);
    }

    public void ToggleButtons(Button toggleButton)
    {
        Button[] sibling_buttons = UIUtilities.GetSiblingsOfType<Button>(toggleButton.gameObject);
        if (sibling_buttons == null || sibling_buttons.Length == 0)
            return;
        bool is_active = sibling_buttons[0].gameObject.activeSelf;
        foreach(Button sibling in sibling_buttons)
            sibling.gameObject.SetActive(!is_active);
    }

    public void OpenContractManager()
    {
        ContractManager.Instance.OpenContractForm();
    }
}