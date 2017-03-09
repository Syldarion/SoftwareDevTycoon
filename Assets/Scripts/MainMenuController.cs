using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
    private const float TIME_PER_CHARACTER = 0.05f;
    private const float CURSOR_FLASH_RATE = 0.2f;
    private const float WAIT_FOR_EXECUTE = 0.5f;

    public Button ConsoleMessagePrefab;

    [Header("New Game Panels")]
    public CanvasGroup InformationPanel;
    public CanvasGroup SkillsPanel;
    public CanvasGroup AvatarPanel;

    [Header("New Game Information UI")]
    public InputField CharacterNameInput;
    public InputField CharacterBirthdayDayInput;
    public InputField CharacterBirthdayMonthInput;
    public InputField CharacterBirthdayYearInput;
    public Text CharacterLocationText;

    [Header("New Game Skills UI")]
    public SkillAllocationControl ProgrammingSkillAllocator;
    public SkillAllocationControl UserInterfacesSkillAllocator;
    public SkillAllocationControl DatabasesSkillAllocator;
    public SkillAllocationControl NetworkingSkillAllocator;
    public SkillAllocationControl WebDevelopmentSkillAllocator;

    [Header("New Game Avatar UI")]
    public CharacterAvatar NewCharacterAvatar;

    [Header("Settings Panels")]
    public CanvasGroup AudioSettingsPanel;
    public CanvasGroup VideoSettingsPanel;
    public CanvasGroup ControlsSettingsPanel;
    public CanvasGroup GameplaySettingsPanel;

    [Header("Console UI")]
    public Text ConsoleText;
    public RectTransform ConsoleMessageList;
    public Image CursorImage;
    public Animator ConsoleScreenAnimator;

    private Queue<MessageItem> messageQueue;
    private bool cursorFlashing;
    private CanvasGroup activePanel;
    private DateTime latestBirthday;
    private int currentLocationIndex;

    void Awake()
    {
        Instance = this;

        messageQueue = new Queue<MessageItem>();
        cursorFlashing = false;
        activePanel = null;
        latestBirthday = DateTime.Today - new TimeSpan(365 * 18, 0, 0, 0);
        currentLocationIndex = 0;
    }

    void Start()
    {
        StartCoroutine(FlashCursor());
        StartCoroutine(CheckQueue());

        LoadCurrentSettings();
        CharacterLocationText.text = Location.Locations[currentLocationIndex].Name;

        AddMessageToQueue(new MessageItem("software_dev_tycoon", "exec", OpenBaseMenu));
    }

    void Update()
    {
        
    }

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
        AddMessageToQueue(new MessageItem("information", "modify", OpenInformationPanel));
        AddMessageToQueue(new MessageItem("skills", "modify", OpenSkillsPanel));
        AddMessageToQueue(new MessageItem("avatar", "modify", OpenAvatarPanel));
        AddMessageToQueue(new MessageItem("start_game", "exec", StartNewGame));
        AddMessageToQueue(new MessageItem("main_menu", "exec", () =>
        {
            StartCoroutine(SwitchConsoleScreenPanel(null));
            OpenBaseMenu();
        }));
    }

    public void OpenInformationPanel()
    {
        StartCoroutine(SwitchConsoleScreenPanel(InformationPanel));
    }

    public void CheckBirthdayInput()
    {
        DateTime entered_date = ConstructDateFromInput();
        if(entered_date > latestBirthday)
            entered_date = latestBirthday;

        CharacterBirthdayDayInput.text = entered_date.Day.ToString();
        CharacterBirthdayMonthInput.text = entered_date.Month.ToString();
        CharacterBirthdayYearInput.text = entered_date.Year.ToString();
    }

    public DateTime ConstructDateFromInput()
    {
        int day, month, year;
        if (!int.TryParse(CharacterBirthdayDayInput.text, out day)) day = DateTime.Today.Day;
        if (!int.TryParse(CharacterBirthdayMonthInput.text, out month)) month = DateTime.Today.Month;
        if (!int.TryParse(CharacterBirthdayYearInput.text, out year)) year = DateTime.Today.Year;

        day = Mathf.Clamp(day, 1, 31);
        month = Mathf.Clamp(month, 1, 12);
        year = Mathf.Clamp(year, 1970, 9999);

        var entered_date = new DateTime(year, month, day);

        return entered_date;
    }

    public void SwitchLocationSelection(int direction)
    {
        int temp = currentLocationIndex + direction;
        if(temp >= Location.Locations.Count)
            temp = 0;
        if(temp < 0)
            temp = Location.Locations.Count - 1;
        currentLocationIndex = temp;
        CharacterLocationText.text = Location.Locations[currentLocationIndex].Name;
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
        SaveManager.Instance.ActiveSave = null;

        DateTime birthday = ConstructDateFromInput();

        var new_character = new Character {
            Name = string.IsNullOrEmpty(CharacterNameInput.text) ? PersonNames.GetRandomName() : CharacterNameInput.text,
            Age = DateTime.Today.Month > birthday.Month ||
                  (DateTime.Today.Month == birthday.Month && DateTime.Today.Day > birthday.Day)
                      ? DateTime.Today.Year - birthday.Year
                      : DateTime.Today.Year - birthday.Year - 1,
            PersonGender = NewCharacterAvatar.AvatarMaleBody.gameObject.activeSelf
                               ? Person.Gender.Male
                               : Person.Gender.Female,
            Birthday = birthday.ToString("dd-MM-yyyy"),
            CurrentLocation = Location.Locations[currentLocationIndex]
        };

        new_character.SetHeadColor(NewCharacterAvatar.AvatarHead.color);
        new_character.SetBodyColor(NewCharacterAvatar.AvatarMaleBody.color);
        new_character.SetLegsColor(NewCharacterAvatar.AvatarLegs.color);

        new_character.AdjustMoney(5000);
        new_character.AdjustReputation(50);

        new_character.Skills[Skill.Programming].Level = ProgrammingSkillAllocator.CurrentSkillLevel;
        new_character.Skills[Skill.UserInterfaces].Level = UserInterfacesSkillAllocator.CurrentSkillLevel;
        new_character.Skills[Skill.Databases].Level = DatabasesSkillAllocator.CurrentSkillLevel;
        new_character.Skills[Skill.Networking].Level = NetworkingSkillAllocator.CurrentSkillLevel;
        new_character.Skills[Skill.WebDevelopment].Level = WebDevelopmentSkillAllocator.CurrentSkillLevel;

        SceneManager.LoadScene("in_game");
        new_character.SetupEvents();
    }

    public void OpenLoadGameMenu()
    {
        ClearMenu();
        foreach(GameSave save in SaveManager.Instance.Saves)
        {
            GameSave save1 = save;
            AddMessageToQueue(new MessageItem(save1.Name, "load", save1.LoadGame));
        }
        AddMessageToQueue(new MessageItem("main_menu", "exec", OpenBaseMenu));
    }

    public void OpenBaseSettingsMenu()
    {
        ClearMenu();
        AddMessageToQueue(new MessageItem("audio", "modify", OpenAudioSettingsMenu));
        AddMessageToQueue(new MessageItem("video", "modify", OpenVideoSettingsMenu));
        AddMessageToQueue(new MessageItem("controls", "modify", OpenControlsSettingsMenu));
        AddMessageToQueue(new MessageItem("gameplay", "modify", OpenGameplaySettingsMenu));
        AddMessageToQueue(new MessageItem("save", "exec", SaveCurrentSettings));
        AddMessageToQueue(new MessageItem("main_menu", "exec", () =>
        {
            StartCoroutine(SwitchConsoleScreenPanel(null));
            OpenBaseMenu();
        }));
    }

    public void OpenAudioSettingsMenu()
    {
        StartCoroutine(SwitchConsoleScreenPanel(AudioSettingsPanel));
    }

    public void OpenVideoSettingsMenu()
    {
        StartCoroutine(SwitchConsoleScreenPanel(VideoSettingsPanel));
    }

    public void OpenControlsSettingsMenu()
    {
        StartCoroutine(SwitchConsoleScreenPanel(ControlsSettingsPanel));
    }

    public void OpenGameplaySettingsMenu()
    {
        StartCoroutine(SwitchConsoleScreenPanel(GameplaySettingsPanel));
    }

    public void LoadCurrentSettings()
    {
        AudioController.Instance.LoadAudioSettings();
    }

    public void SaveCurrentSettings()
    {
        AudioController.Instance.SaveAudioSettings();
    }

    public void Terminate()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
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
            while (!ConsoleScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("CloseAnimation"))
                yield return null;
            while (!ConsoleScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("WaitState"))
                yield return null;
        }

        activePanel = newPanel;

        if(activePanel != null)
        {
            OpenConsoleScreen();
            while (!ConsoleScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("OpenAnimation"))
                yield return null;
            while (!ConsoleScreenAnimator.GetCurrentAnimatorStateInfo(0).IsName("WaitState"))
                yield return null;
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
