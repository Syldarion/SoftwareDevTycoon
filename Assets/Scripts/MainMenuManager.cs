using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    public RectTransform GameSaveList;
    public SaveGameObject SaveGameObjectPrefab;
    public GameObject CharacterPrefab;

    public GameSave SelectedGameSave;

    private CanvasGroup activePanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SaveManager.Instance.LoadAllGames();
        PopulateSaveList();
    }

    void Update()
    {

    }

    public void PopulateSaveList()
    {
        foreach(Transform child in GameSaveList)
            Destroy(child.gameObject);

        foreach (var save in SaveManager.Instance.Saves)
        {
            SaveGameObject new_save_object = Instantiate(SaveGameObjectPrefab);
            new_save_object.transform.SetParent(GameSaveList, false);
            new_save_object.SetSave(save);
        }
    }

    public void SwitchActivePanel(CanvasGroup panel)
    {
        if(activePanel != null)
        {
            activePanel.alpha = 0.0f;
            activePanel.interactable = false;
            activePanel.blocksRaycasts = false;
        }
        activePanel = panel == activePanel ? null : panel;
        if(activePanel != null)
        {
            activePanel.alpha = 1.0f;
            activePanel.interactable = true;
            activePanel.blocksRaycasts = true;
        }
    }

    public void ExitGame()
    {
        if (Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}