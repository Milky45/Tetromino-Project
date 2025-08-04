using UnityEngine;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class LoadP2Stats : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI Highscore;
    public TextMeshProUGUI SurvivalTime;
    public TextMeshProUGUI Matches;
    public TextMeshProUGUI Wins;
    public TextMeshProUGUI Losses;
    public TextMeshProUGUI Winrate;

    private string dbPath;

    void Start()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "TetrominoDB.db");
        LoadPlayer1Stats();
    }

    public void LoadP2()
    {
        Start();
    }

    private IDbConnection GetConnection()
    {
        return new SqliteConnection("URI=file:" + dbPath);
    }

    private void LoadPlayer1Stats()
    {
        using (var connection = GetConnection())
        {
            connection.Open();

            string username = null;
            int vsMatches = 0, wins = 0, losses = 0, highscore = 0;
            float survivalTimeSeconds = 0f;

            // Step 1: Get the username of the active Player 2
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT userInfo.username 
                    FROM userInfo 
                    INNER JOIN UserAccState ON userInfo.ID = UserAccState.ID 
                    WHERE UserAccState.IsActive = 2";

                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    Debug.LogWarning("No Player 2 is currently logged in.");
                    usernameText.text = "Not logged in";
                    return;
                }

                username = result.ToString();
            }

            usernameText.text = $"{username}";

            // Step 2: Get stats from userStatistics table
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT VsMatches, Wins, Losses, SurvivalTime, HighScore
                    FROM userstatistics
                    INNER JOIN userInfo ON userInfo.ID = userstatistics.ID
                    WHERE userInfo.username = @username";

                cmd.Parameters.Add(new SqliteParameter("@username", username));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vsMatches = reader.GetInt32(0);
                        wins = reader.GetInt32(1);
                        losses = reader.GetInt32(2);
                        survivalTimeSeconds = reader.GetFloat(3);
                        highscore = reader.GetInt32(4);
                    }
                }
            }

            // Step 3: Display stats
            Matches.text = $"Matches: {vsMatches}";
            Wins.text = $"Wins: {wins}";
            Losses.text = $"Losses: {losses}";
            Highscore.text = $"Highscore: {highscore}";
            SurvivalTime.text = $"Time of survival: {Mathf.FloorToInt(survivalTimeSeconds / 60)} min";

            float winRate = CalculateWinRate(wins, vsMatches);
            Winrate.text = $"Winrate: {winRate:0.##}%";
        }
    }

    private float CalculateWinRate(int wins, int totalMatches)
    {
        if (totalMatches == 0) return 0f;
        return (wins / (float)totalMatches) * 100f;
    }
}
