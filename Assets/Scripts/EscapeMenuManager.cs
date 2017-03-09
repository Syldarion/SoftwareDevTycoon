using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EscapeMenuManager : MonoBehaviour
{
    private InputField saveNameField;

    void Awake()
    {
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SDTUIController.Instance.OpenCanvas(GetComponent<CanvasGroup>());
    }

    public void TrySaveGame()
    {
        DialogueBox.Instance.CreateNewDialogue("Save Game");
        saveNameField = DialogueBox.Instance.AddInputField("Enter save name...");
        DialogueBox.Instance.AddButton("Save", SaveGame);
    }

    private void SaveGame()
    {
        SaveManager.Instance.SaveGame();
        DialogueBox.Instance.Cleanup();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("main_menu");
    }
}
