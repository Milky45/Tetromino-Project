using UnityEngine;
using System.Data;
using TMPro;
using UnityEngine.UI;

public class LoginUser : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text UserState;
    public TextMeshProUGUI LogDisplay;
    public LoginLoader loginLoader;

    public int playerNumber;

    private LoginValidator validator;
    public static bool isLoggedIn = false;

    void Start()
    {
        validator = new LoginValidator("TetrominoDB.db");
    }

    public void EnterPressed()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Login failed: Username or password field is empty.");
            LogDisplay.text = "Login failed: Username or password field is empty.";
            return;
        }

        isLoggedIn = validator.TryLogin(username, password, playerNumber);

        if (isLoggedIn)
        {
            loginLoader.LoadLoggedInUsers();
            LogDisplay.text = $"Login successful: {username} is now Player {playerNumber}.";
        }
        else
        {
            LogDisplay.text = $"Login failed";
        }
        usernameField.text = "";
        passwordField.text = "";
    }

    public void OnLogOutPressed()
    {
        validator.LogOutPlayer(playerNumber);
        loginLoader.LoadLoggedInUsers();
        LogDisplay.text = $"Logged out Player {playerNumber}.";
    }

    public void OnDeleteAccountPressed()
    {
        if (!isLoggedIn)
        {
            LogDisplay.text = "You must be logged in to delete your account.";
            return;
        }

        string username = usernameField.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            LogDisplay.text = "Please enter your username to confirm deletion.";
            return;
        }

        if (validator.VerifyUserOwnership(username, playerNumber))
        {
            if (validator.DeleteAccount(username))
            {
                validator.LogOutPlayer(playerNumber);
                loginLoader.LoadLoggedInUsers();
                LogDisplay.text = "Account successfully deleted.";
                usernameField.text = "";
                passwordField.text = "";
            }
            else
            {
                LogDisplay.text = "Failed to delete account. Please try again.";
            }
        }
        else
        {
            LogDisplay.text = "You can only delete your own account.";
        }
    }
}
