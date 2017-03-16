using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SaveGameObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button LoadGameButton;
    public Button DeleteGameButton;

    public Text SaveNameText;
    public Text SaveDateText;

    private GameSave objectSave;

    void Start()
    {
        LoadGameButton.gameObject.SetActive(false);
        DeleteGameButton.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public void SetSave(GameSave save)
    {
        if (save == null) return;

        objectSave = save;

        SaveNameText.text = save.Name;
        SaveDateText.text = save.LastModDate;
    }

    public void LoadGame()
    {
        objectSave.LoadGame();
    }

    public void TryDeleteGame()
    {
        DialogueBox.Instance.CreateYesNoDialogue(
            "Are you sure you want to delete this save?",
            DeleteGame,
            DialogueBox.Instance.Cleanup);
    }

    public void DeleteGame()
    {
        SaveManager.Instance.DeleteGame(objectSave);
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LoadGameButton.gameObject.SetActive(true);
        DeleteGameButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LoadGameButton.gameObject.SetActive(false);
        DeleteGameButton.gameObject.SetActive(false);
    }
}
