using UnityEngine;
using TMPro;

public class AccountSettingsUI : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI statusText; // Add this to show status messages
    public GameObject confirmPanel; // Uncomment this for confirmation dialog
    public LoginLoader loginLoader; // Add reference to LoginLoader

    private LoginValidator validator;
    private int currentPlayerNumber = 1; // Default to Player 1, set this in Inspector

    void Start()
    {
        validator = new LoginValidator("TetrominoDB.db");
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    public void OnDeleteAccountButtonPressed()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter both username and password.";
            return;
        }

        // First verify credentials
        if (!validator.VerifyCredentials(username, password))
        {
            statusText.text = "Invalid username or password.";
            return;
        }

        // Then verify user ownership
        if (!validator.VerifyUserOwnership(username, currentPlayerNumber))
        {
            statusText.text = "You can only delete your own account.";
            return;
        }

        // Show confirmation dialog
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
        }
        else
        {
            // If no confirmation panel, proceed with deletion
            OnConfirmDelete();
        }
    }

    public void OnConfirmDelete()
    {
        string username = usernameField.text.Trim();
        
        if (validator.DeleteAccount(username))
        {
            statusText.text = "Account deleted successfully.";
            // Clear the input fields
            usernameField.text = "";
            passwordField.text = "";
            
            // Update the UI to reflect the changes
            if (loginLoader != null)
                loginLoader.LoadLoggedInUsers();
        }
        else
        {
            statusText.text = "Failed to delete account. Please try again.";
        }

        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    public void OnCancelDelete()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
        statusText.text = "Account deletion cancelled.";
    }
}
