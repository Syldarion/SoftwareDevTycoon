using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    public CanvasGroup MenuOptionsPanel;

    private InputField saveNameField;

    void Awake()
    {
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ToggleMenu()
    {
        SDTUIController.Instance.OpenCanvas(MenuOptionsPanel, false, true);
    }

    public void ToggleButtons(RectTransform mainSibling)
    {
        Button[] sibling_buttons = UIUtilities.GetSiblingsOfType<Button>(mainSibling.gameObject);
        if (sibling_buttons == null || sibling_buttons.Length == 0)
            return;
        bool is_active = sibling_buttons[0].gameObject.activeSelf;
        foreach(Button sibling in sibling_buttons)
            sibling.gameObject.SetActive(!is_active);
    }

    public void TrySaveGame()
    {
        DialogueBox.Instance.CreateNewDialogue("Save Game");
        saveNameField = DialogueBox.Instance.AddInputField("Enter save name...");
        DialogueBox.Instance.AddButton("Save", SaveGame);
    }

    private void SaveGame()
    {
        SaveManager.Instance.SaveGame(saveNameField.text);
        DialogueBox.Instance.Cleanup();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("main_menu");
    }
}
