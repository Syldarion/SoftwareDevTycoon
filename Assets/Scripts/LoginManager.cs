using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Security.Permissions;
using System.Threading;
using UnityEngine.SceneManagement;

public class LoginManager : Singleton<LoginManager>
{
    public Canvas LoginCanvas;
    
    public InputField UsernameInput;
    public InputField PasswordInput;

    public string AccountID;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void TryLogin()
    {
        string username_text = UsernameInput.text;
        string password_text = PasswordInput.text;

        username_text = username_text.Trim();
        password_text = password_text.Trim();

        if (username_text.Length <= 0)
        {
            StartCoroutine(FlashInputField(UsernameInput));
            return;
        }
        if (password_text.Length <= 0)
        {
            StartCoroutine(FlashInputField(PasswordInput));
            return;
        }

        StartCoroutine(Login(username_text, password_text));
    }

    public void TryCreateAccount()
    {
        string username_text = UsernameInput.text;
        string password_text = PasswordInput.text;

        username_text = username_text.Trim();
        password_text = password_text.Trim();

        if (username_text.Length <= 0)
        {
            StartCoroutine(FlashInputField(UsernameInput));
            return;
        }
        if (password_text.Length <= 0)
        {
            StartCoroutine(FlashInputField(PasswordInput));
            return;
        }

        StartCoroutine(CreateAccount(username_text, password_text));
    }

    IEnumerator Login(string un, string pw)
    {
        StopCoroutine("Login");

        string message = string.Format(
            "LOGIN:{0}:{1}<EOF>",
            un,
            pw);

        SocketController login_controller = new SocketController();
        login_controller.StartClient(SocketController.RequestType.Login, message);

        while (login_controller.Executing)
            yield return null;

        int response_code = ServerMessageCodes.GetServerMessageCode(login_controller.Response);

        if (response_code == 0)
        {
            AccountID = login_controller.Response.Split(':')[1];
            SceneManager.LoadScene("main_menu");
            yield break;
        }
        else
        {
            //output error message or whatever
        }

        UsernameInput.text = string.Empty;
        PasswordInput.text = string.Empty;
    }

    IEnumerator CreateAccount(string un, string pw)
    {
        StopCoroutine("CreateAccount");

        string message = string.Format(
            "CREATE:{0}:{1}<EOF>",
            un,
            pw);

        SocketController create_controller = new SocketController();
        create_controller.StartClient(SocketController.RequestType.Login, message);

        while (create_controller.Executing)
            yield return null;

        UsernameInput.text = string.Empty;
        PasswordInput.text = string.Empty;
    }

    IEnumerator FlashInputField(InputField field)
    {
        int ticks = 10;

        ColorBlock block;

        while (ticks > 0)
        {
            block = field.colors;
            block.normalColor = (block.normalColor == Color.white ? Color.red : Color.white);
            field.colors = block;

            ticks--;
            yield return new WaitForSeconds(0.2f);
        }

        block = field.colors;
        block.normalColor = Color.white;
        field.colors = block;
    }
}