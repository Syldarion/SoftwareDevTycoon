using UnityEngine;
using System.Collections;

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

    public void OpenContractManager()
    {
        ContractManager.Instance.OpenContractForm();
    }
}