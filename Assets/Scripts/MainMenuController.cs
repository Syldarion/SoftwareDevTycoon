using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageItem
{
    public string Message;
    public string ExecMessage;
    public UnityAction Action;

    public MessageItem(string msg, string execPre, UnityAction action)
    {
        Message = msg;
        ExecMessage = string.Format("{0} {1}", execPre, msg);
        Action = action;
    }

    public void Execute()
    {
        MainMenuController.Instance.StartCoroutine(
            MainMenuController.Instance.RunExecMessage(this));
    }
}

public class MainMenuController : Singleton<MainMenuController>
{
    private const float TIME_PER_CHARACTER = 0.07f;
    private const float CURSOR_FLASH_RATE = 0.2f;
    private const float WAIT_FOR_EXECUTE = 0.5f;

    public Button ConsoleMessagePrefab;

    [Header("New Game Panels")]
    public CanvasGroup InformationPanel;
    public CanvasGroup SkillsPanel;
    public CanvasGroup AvatarPanel;

    [Header("New Game Information UI")]
    public InputField CharacterName;

    [Header("New Game Skills UI")]
    public InputField ProgrammingSkillInput;
    public InputField UserInterfacesSkillInput;
    public InputField DatabasesSkillInput;
    public InputField NetworkingSkillInput;
    public InputField WebDevelopmentSkillInput;

    //[Header("New Game Avatar UI")]

    [Header("Console UI")]
    public Text ConsoleText;
    public RectTransform ConsoleMessageList;
    public Image CursorImage;
    public Animator ConsoleScreenAnimator;

    private Queue<MessageItem> messageQueue;
    private bool cursorFlashing;
    private CanvasGroup activePanel;

    void Awake()
    {
        Instance = this;

        messageQueue = new Queue<MessageItem>();
        cursorFlashing = false;
        activePanel = null;
    }

    void Start()
    {
        StartCoroutine(FlashCursor());
        StartCoroutine(CheckQueue());

        AddMessageToQueue(new MessageItem("software_dev_tycoon", "exec", OpenBaseMenu));
    }

    void Update() {}

    public void ClearMenu()
    {
        foreach(Transform child in ConsoleMessageList)
            Destroy(child.gameObject);
    }

    public void AddMessageToQueue(MessageItem message)
    {
        messageQueue.Enqueue(message);
    }

    public void AddMessageToList(MessageItem message)
    {
        Button new_list_item = Instantiate(ConsoleMessagePrefab);
        new_list_item.GetComponent<Button>().onClick.AddListener(message.Execute);
        new_list_item.GetComponentInChildren<Text>().text = message.Message;
        new_list_item.transform.SetParent(ConsoleMessageList, false);
    }

    public void OpenBaseMenu()
    {
        ClearMenu();
        AddMessageToQueue(new MessageItem("new_game", "exec", OpenNewGameMenu));
        AddMessageToQueue(new MessageItem("load_game", "exec", OpenLoadGameMenu));
        AddMessageToQueue(new MessageItem("settings", "exec", OpenBaseSettingsMenu));
        AddMessageToQueue(new MessageItem("terminate", "exec", Terminate));
    }

    public void OpenNewGameMenu()
    {
        ClearMenu();
        //open new game panel

        AddMessageToQueue(new MessageItem("information", "modify", OpenCustomizationPanel));
        AddMessageToQueue(new MessageItem("skills", "modify", OpenSkillsPanel));
        AddMessageToQueue(new MessageItem("avatar", "modify", OpenAvatarPanel));
        AddMessageToQueue(new MessageItem("start_game", "exec", StartNewGame));
        AddMessageToQueue(new MessageItem("main_menu", "exec", () =>
        {
            StartCoroutine(SwitchConsoleScreenPanel(null));
            OpenBaseMenu();
        }));
    }

    public void OpenCustomizationPanel()
    {
        StartCoroutine(SwitchConsoleScreenPanel(InformationPanel));
    }

    public void OpenSkillsPanel()
    {
        StartCoroutine(SwitchConsoleScreenPanel(SkillsPanel));
    }

    public void OpenAvatarPanel()
    {
        StartCoroutine(SwitchConsoleScreenPanel(AvatarPanel));
    }

    public void StartNewGame()
    {
        
    }

    public void OpenLoadGameMenu()
    {
        ClearMenu();
        foreach(var save in SaveManager.Instance.Saves)
        {
            var save1 = save;
            AddMessageToQueue(new MessageItem(save1.Name, "load", save1.LoadGame));
        }
        AddMessageToQueue(new MessageItem("main_menu", "exec", OpenBaseMenu));
    }

    public void OpenBaseSettingsMenu()
    {
        ClearMenu();
        AddMessageToQueue(new MessageItem("audio", "exec", OpenAudioSettingsMenu));
        AddMessageToQueue(new MessageItem("video", "exec", OpenVideoSettingsMenu));
        AddMessageToQueue(new MessageItem("controls", "exec", OpenControlsSettingsMenu));
        AddMessageToQueue(new MessageItem("gameplay", "exec", OpenGameplaySettingsMenu));
        AddMessageToQueue(new MessageItem("main_menu", "exec", OpenBaseMenu));
    }

    public void OpenAudioSettingsMenu()
    {
        ClearMenu();
        OpenConsoleScreen();
        //open audio settings panel
        AddMessageToQueue(new MessageItem("settings", "exec", () =>
        {
            CloseConsoleScreen();
            OpenBaseSettingsMenu();
        }));
    }

    public void OpenVideoSettingsMenu()
    {
        ClearMenu();
        //open video settings panel
        AddMessageToQueue(new MessageItem("settings", "exec", () =>
        {
            CloseConsoleScreen();
            OpenBaseSettingsMenu();
        }));
    }

    public void OpenControlsSettingsMenu()
    {
        ClearMenu();
        //open controls settings panel
        AddMessageToQueue(new MessageItem("settings", "exec", () =>
        {
            CloseConsoleScreen();
            OpenBaseSettingsMenu();
        }));
    }

    public void OpenGameplaySettingsMenu()
    {
        ClearMenu();
        //open gameplay settings panel
        AddMessageToQueue(new MessageItem("settings", "exec", () =>
        {
            CloseConsoleScreen();
            OpenBaseSettingsMenu();
        }));
    }

    public void Terminate()
    {
        if(Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }

    private void LoadSave(GameSave save)
    {
        
    }

    public void OpenConsoleScreen()
    {
        ConsoleScreenAnimator.SetTrigger("OpenTrigger");
    }

    public void CloseConsoleScreen()
    {
        ConsoleScreenAnimator.SetTrigger("CloseTrigger");
    }

    IEnumerator SwitchConsoleScreenPanel(CanvasGroup newPanel)
    {
        if(activePanel != null)
        {
            activePanel.interactable = false;
            activePanel.blocksRaycasts = false;
            while (activePanel.alpha > 0.0f)
            {
                activePanel.alpha -= 5.0f * Time.deltaTime;
                yield return null;
            }
            activePanel.alpha = 0.0f;
            CloseConsoleScreen();
            yield return new WaitForSeconds(1.0f);
        }

        activePanel = newPanel;

        if(activePanel != null)
        {
            OpenConsoleScreen();
            yield return new WaitForSeconds(1.0f);
            while(activePanel.alpha < 1.0f)
            {
                activePanel.alpha += 2.0f * Time.deltaTime;
                yield return null;
            }
            activePanel.alpha = 1.0f;
            activePanel.interactable = true;
            activePanel.blocksRaycasts = true;
        }
    }

    IEnumerator FlashCursor()
    {
        cursorFlashing = true;

        while(Application.isPlaying)
        {
            while(cursorFlashing)
            {
                CursorImage.gameObject.SetActive(!CursorImage.gameObject.activeSelf);
                yield return new WaitForSeconds(CURSOR_FLASH_RATE);
            }
            yield return null;
        }
    }

    IEnumerator CheckQueue()
    {
        while(Application.isPlaying)
        {
            if(messageQueue.Count > 0)
                yield return StartCoroutine(RunQueueMessage(messageQueue.Dequeue()));
            yield return null;
        }
    }

    IEnumerator RunQueueMessage(MessageItem message)
    {
        cursorFlashing = false;
        CursorImage.gameObject.SetActive(true);
        ConsoleText.text = string.Empty;

        int current_length = 0;
        int max_length = message.Message.Length;
        while(current_length <= max_length)
        {
            GetComponent<AudioSource>().Play();
            ConsoleText.text = message.Message.Substring(0, current_length);
            current_length++;
            yield return new WaitForSeconds(TIME_PER_CHARACTER);
        }
        AddMessageToList(message);

        ConsoleText.text = string.Empty;
        CursorImage.gameObject.SetActive(false);
        cursorFlashing = true;
    }

    public IEnumerator RunExecMessage(MessageItem message)
    {
        cursorFlashing = false;
        CursorImage.gameObject.SetActive(true);
        ConsoleText.text = string.Empty;

        int current_length = 0;
        int max_length = message.ExecMessage.Length;
        while(current_length <= max_length)
        {
            GetComponent<AudioSource>().Play();
            ConsoleText.text = message.ExecMessage.Substring(0, current_length);
            current_length++;
            yield return new WaitForSeconds(TIME_PER_CHARACTER);
        }

        yield return new WaitForSeconds(WAIT_FOR_EXECUTE);
        message.Action.Invoke();

        ConsoleText.text = string.Empty;
        CursorImage.gameObject.SetActive(false);
        cursorFlashing = true;
    }
}
