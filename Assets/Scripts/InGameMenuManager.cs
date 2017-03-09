using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    public RectTransform MenuPanel;

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

    public void ToggleMenu()
    {
        StartCoroutine(menuOpen ? CloseMenu() : OpenMenu());
    }

    private IEnumerator OpenMenu()
    {
        while(MenuPanel.anchoredPosition.x < 0)
        {
            Vector2 pos = MenuPanel.anchoredPosition;
            pos.x += 500.0f * Time.deltaTime;
            MenuPanel.anchoredPosition = pos;
            yield return null;
        }
        MenuPanel.anchoredPosition.Set(0.0f, MenuPanel.anchoredPosition.y);
        menuOpen = true;
    }

    private IEnumerator CloseMenu()
    {
        while (MenuPanel.anchoredPosition.x > -500)
        {
            Vector2 pos = MenuPanel.anchoredPosition;
            pos.x -= 500.0f * Time.deltaTime;
            MenuPanel.anchoredPosition = pos;
            yield return null;
        }
        MenuPanel.anchoredPosition.Set(-500.0f, MenuPanel.anchoredPosition.y);
        menuOpen = false;
    }

    private void SaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("main_menu");
    }
}
