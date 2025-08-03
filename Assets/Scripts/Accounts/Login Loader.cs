using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class LoginLoader : MonoBehaviour
{
    public TMP_Text player1Text; // Drag this in the Inspector
    public TMP_Text player2Text; // Drag this in the Inspector
    public GameObject LoginP1btn;
    public GameObject LoginP2btn;
    public GameObject P1AccBtn;
    public GameObject P2AccBtn;

    private string dbPath;

    void Awake()
    {
        dbPath = System.IO.Path.Combine(Application.streamingAssetsPath, "TetrominoDB.db");
        LoadLoggedInUsers();
    }

    private IDbConnection GetConnection()
    {
        return new SqliteConnection("URI=file:" + dbPath);
    }

    internal void LoadLoggedInUsers()
    {
        using (var connection = GetConnection())
        {
            connection.Open();

            string p1Username = null;
            string p2Username = null;

            // Get Player 1 username
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                SELECT userInfo.username 
                FROM userInfo
                INNER JOIN UserAccState ON userInfo.ID = UserAccState.ID
                WHERE UserAccState.IsActive = 1";

                var result = command.ExecuteScalar();
                if (result != null) p1Username = result.ToString();
            }

            // Get Player 2 username
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                SELECT userInfo.username 
                FROM userInfo
                INNER JOIN UserAccState ON userInfo.ID = UserAccState.ID
                WHERE UserAccState.IsActive = 2";

                var result = command.ExecuteScalar();
                if (result != null) p2Username = result.ToString();
            }

            // Update UI for Player 1
            if (player1Text != null)
                player1Text.text = p1Username != null ? $"Player 1: {p1Username}" : "P1: Guest";

            // Update UI for Player 2
            if (player2Text != null)
                player2Text.text = p2Username != null ? $"Player 2: {p2Username}" : "P2: Guest";

            // Show Login buttons or Account buttons
            bool isP1LoggedIn = !string.IsNullOrEmpty(p1Username);
            bool isP2LoggedIn = !string.IsNullOrEmpty(p2Username);

            // Toggle buttons
            if (LoginP1btn != null) LoginP1btn.SetActive(!isP1LoggedIn);
            if (LoginP2btn != null) LoginP2btn.SetActive(!isP2LoggedIn);

            // Show only one AccBtn if only one player is logged in
            if (P1AccBtn != null) P1AccBtn.SetActive(isP1LoggedIn && !isP2LoggedIn);
            if (P2AccBtn != null) P2AccBtn.SetActive(isP2LoggedIn && !isP1LoggedIn);

            // If both are logged in, show both AccBtns
            if (isP1LoggedIn && isP2LoggedIn)
            {
                if (P1AccBtn != null) P1AccBtn.SetActive(true);
                if (P2AccBtn != null) P2AccBtn.SetActive(true);
            }
        }
    }

}
