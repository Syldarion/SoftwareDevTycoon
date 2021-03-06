﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class InGameMenuController : MonoBehaviour
{
    public RectTransform MenuPanel;
    public Image ToggleMenuImage;

    public Canvas MainMenuCanvas;
    public Canvas InGameCanvas;

    private bool menuOpen;

    void Awake()
    {
        menuOpen = false;
    }

    void Start()
    {

    }

    void Update()
    {
    }

    public void ToggleSubMenu(RectTransform subMenu)
    {
        subMenu.gameObject.SetActive(!subMenu.gameObject.activeSelf);
    }

    public void ToggleMenu()
    {
        StartCoroutine(menuOpen ? CloseMenu() : OpenMenu());
    }

    private IEnumerator OpenMenu()
    {
        Vector2 pos;
        while(MenuPanel.anchoredPosition.x < 0)
        {
            pos = MenuPanel.anchoredPosition;
            pos.x += 1000.0f * Time.deltaTime;
            MenuPanel.anchoredPosition = pos;
            yield return null;
        }
        pos = MenuPanel.anchoredPosition;
        pos.x = 0.0f;
        MenuPanel.anchoredPosition = pos;
        ToggleMenuImage.rectTransform.Rotate(0.0f, 0.0f, 180.0f);
        menuOpen = true;
    }

    private IEnumerator CloseMenu()
    {
        Vector2 pos;
        while (MenuPanel.anchoredPosition.x > -500)
        {
            pos = MenuPanel.anchoredPosition;
            pos.x -= 1000.0f * Time.deltaTime;
            MenuPanel.anchoredPosition = pos;
            yield return null;
        }
        pos = MenuPanel.anchoredPosition;
        pos.x = -500.0f;
        MenuPanel.anchoredPosition = pos;
        ToggleMenuImage.rectTransform.Rotate(0.0f, 0.0f, 180.0f);
        menuOpen = false;
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    public void ExitGame()
    {
        CanvasGroup mm_canvas = SDTUIController.Instance.InGameCanvas.GetComponent<CanvasGroup>();
        CanvasGroup ig_canvas = SDTUIController.Instance.MainMenuCanvas.GetComponent<CanvasGroup>();
        mm_canvas.alpha = 0; mm_canvas.interactable = false; mm_canvas.blocksRaycasts = false;
        ig_canvas.alpha = 1; ig_canvas.interactable = true; ig_canvas.blocksRaycasts = true;
    }
}
