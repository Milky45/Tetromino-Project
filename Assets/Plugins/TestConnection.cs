using System;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "TetrominoDB.db");
        CreateDatabase();
        CreateStatisticsViewIfNotExists(); // Create the view
    }

    void CreateDatabase()
    {
        if (!File.Exists(dbPath))
        {
            Debug.Log("Creating new database...");

            string connectionString = "URI=file:" + dbPath;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Example tables (adjust fields as needed)
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS userInfo (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT,
                            Password TEXT,
                            Highscore INTEGER
                        );

                        CREATE TABLE IF NOT EXISTS userStatistics (
                            ID INTEGER,
                            VSmatches INTEGER,
                            Wins INTEGER,
                            Losses INTEGER,
                            FOREIGN KEY(ID) REFERENCES userInfo(ID)
                        );";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        Debug.Log("Database Path: " + dbPath);
    }

    void CreateStatisticsViewIfNotExists()
    {
        string connectionString = "URI=file:" + dbPath;
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Check if the view already exists
                command.CommandText = @"
                    SELECT name FROM sqlite_master 
                    WHERE type = 'view' AND name = 'UserStatsView';";

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    Debug.Log("Creating UserStatsView...");

                    command.CommandText = @"
                        CREATE VIEW UserStatsView AS
                        SELECT 
                            userInfo.ID,
                            userInfo.Username,
                            userInfo.Highscore,
                            userStatistics.VSmatches,
                            userStatistics.Wins,
                            userStatistics.Losses
                        FROM 
                            userInfo
                        INNER JOIN 
                            userStatistics ON userInfo.ID = userStatistics.ID;";
                    command.ExecuteNonQuery();
                }
                else
                {
                    Debug.Log("View 'UserStatsView' already exists.");
                    Debug.Log("Database Path: " + dbPath);
                }
            }
            connection.Close();
        }
    }
}
