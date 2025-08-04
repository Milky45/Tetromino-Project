using UnityEngine;
using TMPro;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class AccountCreator : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;

    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "TetrominoDB.db");
    }

    public void OnSubmitPressed()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            Debug.LogWarning("One or more fields are empty.");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogWarning("Passwords do not match.");
            return;
        }

        using (var connection = new SqliteConnection("URI=file:" + dbPath))
        {
            connection.Open();

            // username unique
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM userInfo WHERE Username = @username";
                checkCmd.Parameters.Add(new SqliteParameter("@username", username));

                int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (userExists > 0)
                {
                    Debug.LogWarning("Username already exists.");
                    return;
                }
            }

            // Insert
            using (var insertCmd = connection.CreateCommand())
            {
                insertCmd.CommandText = @"
                    INSERT INTO userInfo (Username, Userpassword, Highscore)
                    VALUES (@username, @password, 0);";
                insertCmd.Parameters.Add(new SqliteParameter("@username", username));
                insertCmd.Parameters.Add(new SqliteParameter("@password", password));
                insertCmd.ExecuteNonQuery();
            }

            long newUserId;
            // insert ID
            using (var idCmd = connection.CreateCommand())
            {
                idCmd.CommandText = "SELECT last_insert_rowid();";
                newUserId = (long)idCmd.ExecuteScalar();
            }

            // insert into UserAccState
            using (var stateCmd = connection.CreateCommand())
            {
                stateCmd.CommandText = @"
                    INSERT INTO UserAccState (ID, IsActive)
                    VALUES (@id, 0);";
                stateCmd.Parameters.Add(new SqliteParameter("@id", newUserId));
                stateCmd.ExecuteNonQuery();
            }

            // insert into userStatistics
            using (var statsCmd = connection.CreateCommand())
            {
                statsCmd.CommandText = @"
                    INSERT INTO userStatistics (ID, VSmatches, Wins, Losses, SurvivalTime)
                    VALUES (@id, 0, 0, 0, 0);";
                statsCmd.Parameters.Add(new SqliteParameter("@id", newUserId));
                statsCmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        Debug.Log($"Account created successfully for '{username}'!");
    }
}
