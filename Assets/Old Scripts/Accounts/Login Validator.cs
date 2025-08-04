using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;

public class LoginValidator
{
    private string dbPath;
    public LoginLoader loginLoader;

    public LoginValidator(string dbFileName)
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, dbFileName);
    }

    private IDbConnection GetConnection()
    {
        return new SqliteConnection("URI=file:" + dbPath);
    }

    public bool TryLogin(string username, string password, int playerNumber)
    {
        if (playerNumber != 1 && playerNumber != 2)
        {
            Debug.LogWarning("Invalid player number. Must be 1 or 2.");
            return false;
        }

        using (var connection = GetConnection())
        {
            connection.Open();

            // Check if the player slot is already taken
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM UserAccState WHERE IsActive = @playerNumber";
                checkCmd.Parameters.Add(new SqliteParameter("@playerNumber", playerNumber));
                int activeUserCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (activeUserCount > 0)
                {
                    Debug.LogWarning($"Login failed: Player {playerNumber} is already logged in.");
                    return false;
                }
            }

            // Check if user is already logged in on another slot
            using (var checkDuplicateCmd = connection.CreateCommand())
            {
                checkDuplicateCmd.CommandText = "SELECT IsActive FROM UserAccState WHERE ID = (SELECT ID FROM userInfo WHERE username = @username)";
                checkDuplicateCmd.Parameters.Add(new SqliteParameter("@username", username));
                var existingStatus = checkDuplicateCmd.ExecuteScalar();
                if (existingStatus != null && Convert.ToInt32(existingStatus) > 0)
                {
                    Debug.LogWarning($"Login failed: User '{username}' is already logged in as Player {existingStatus}.");
                    return false;
                }
            }

            // Proceed to login
            using (var loginCmd = connection.CreateCommand())
            {
                loginCmd.CommandText = "SELECT Userpassword FROM userInfo WHERE username = @username";
                loginCmd.Parameters.Add(new SqliteParameter("@username", username));

                using (var reader = loginCmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Debug.LogWarning($"Login failed: Username '{username}' does not exist.");
                        return false;
                    }

                    string dbPassword = reader.GetString(0);

                    if (dbPassword == password)
                    {
                        Debug.Log($"Login success: Welcome, {username}! (Player {playerNumber})");

                        reader.Close();
                        SetUserActive(connection, username, playerNumber);

                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"Login failed: Incorrect password for user '{username}'.");
                        return false;
                    }
                }
            }
        }
    }

    public bool AccountExists(string username)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM userInfo WHERE username = @username";
                command.Parameters.Add(new SqliteParameter("@username", username));
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    Debug.Log($"Account check: '{username}' exists.");
                    return true;
                }
                else
                {
                    Debug.Log($"Account check: '{username}' does not exist.");
                    return false;
                }
            }
        }
    }

    // Method to check if there's an active user (IsActive = 1)
    public bool IsUserLoggedIn()
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM UserAccState WHERE IsActive = 1";
                int count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0; // If any user is active, return true
            }
        }
    }
    private void SetUserActive(IDbConnection connection, string username, int playerNumber)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            UPDATE UserAccState 
            SET IsActive = @playerNumber
            WHERE ID = (SELECT ID FROM userInfo WHERE username = @username);";

            command.Parameters.Add(new SqliteParameter("@playerNumber", playerNumber));
            command.Parameters.Add(new SqliteParameter("@username", username));
            command.ExecuteNonQuery();
        }

        Debug.Log($"User {username} is now active as Player {playerNumber}.");
    }


    public void LogOutPlayer(int playerNumber)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                UPDATE UserAccState
                SET IsActive = 0
                WHERE IsActive = @playerNumber";
                command.Parameters.Add(new SqliteParameter("@playerNumber", playerNumber));

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Debug.Log($"Player {playerNumber} successfully logged out.");
                }
                else
                {
                    Debug.LogWarning($"No active user for Player {playerNumber} to log out.");
                }
            }
        }
    }


    public bool VerifyCredentials(string username, string password)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT userpassword FROM userInfo WHERE username = @username";
                command.Parameters.Add(new SqliteParameter("@username", username));

                object result = command.ExecuteScalar();

                if (result != null && result.ToString() == password)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool VerifyUserOwnership(string username, int playerNumber)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(*) 
                    FROM userInfo 
                    INNER JOIN UserAccState ON userInfo.ID = UserAccState.ID 
                    WHERE userInfo.username = @username AND UserAccState.IsActive = @playerNumber";
                command.Parameters.Add(new SqliteParameter("@username", username));
                command.Parameters.Add(new SqliteParameter("@playerNumber", playerNumber));
                
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }

    public bool DeleteAccount(string username)
    {
        using (var connection = GetConnection())
        {
            connection.Open();

            // Get ID from userInfo
            long userId = -1;
            using (var idCmd = connection.CreateCommand())
            {
                idCmd.CommandText = "SELECT ID FROM userInfo WHERE username = @username";
                idCmd.Parameters.Add(new SqliteParameter("@username", username));
                var result = idCmd.ExecuteScalar();
                if (result == null)
                {
                    Debug.LogWarning("User not found for deletion.");
                    return false;
                }
                userId = (long)result;
            }

            try
            {
                // Delete from all tables
                string[] tables = { "userInfo", "UserAccState", "userStatistics" };
                foreach (string table in tables)
                {
                    using (var deleteCmd = connection.CreateCommand())
                    {
                        deleteCmd.CommandText = $"DELETE FROM {table} WHERE ID = @id";
                        deleteCmd.Parameters.Add(new SqliteParameter("@id", userId));
                        deleteCmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error deleting account: {ex.Message}");
                return false;
            }
        }
    }

}
