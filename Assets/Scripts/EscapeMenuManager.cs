using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EscapeMenuManager : MonoBehaviour
{
    private InputField saveNameField;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            open = !open;

            if (open)
            {
                TimeManager.Pause();
                TimeManager.Lock();
                UIUtilities.ActivateCanvasGroup(GetComponent<CanvasGroup>());
            }
            else
            {
                UIUtilities.DeactivateCanvasGroup(GetComponent<CanvasGroup>());
                TimeManager.Unlock();
                TimeManager.Unpause();
            }
        }
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