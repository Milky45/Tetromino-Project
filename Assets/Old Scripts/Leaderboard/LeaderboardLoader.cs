using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class LeaderboardLoader : MonoBehaviour
{
    private LeaderboardEntry templateScript;
    public GameObject entryTemplate;           // UI template
    public Transform leaderboardContainer;     // container
    public float verticalSpacing = 60f;        // gap between entries

    private string dbPath;

    public void Start()
    {
        dbPath = Path.Combine(Application.streamingAssetsPath, "TetrominoDB.db");
        LoadLeaderboardFromView();
    }

    public void LoadLB()
    {
        Start();
    }

    public void LoadLeaderboardFromView()
    {
        string conn = "URI=file:" + dbPath;
        entryTemplate.SetActive(false);
        Debug.Log("Database Path: " + dbPath);

        using (var connection = new SqliteConnection(conn))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Username, Highscore, SurvivalTime FROM LeaderboardView ORDER BY Highscore DESC;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    int index = 0;

                    while (reader.Read())
                    {
                        templateScript = entryTemplate.GetComponent<LeaderboardEntry>();
                        if (templateScript != null)
                        {
                            templateScript.SetEntry(
                                index + 1, // Position number (1-based)
                                reader["Username"].ToString(),
                                int.Parse(reader["Highscore"].ToString()),
                                int.Parse(reader["SurvivalTime"].ToString())
                            );
                        }

                        GameObject clone = Instantiate(entryTemplate, leaderboardContainer);
                        clone.SetActive(true);
                        clone.name = "Entry_" + index;

                        RectTransform rt = clone.GetComponent<RectTransform>();
                        rt.anchoredPosition = new Vector2(0, -index * verticalSpacing);

                        index++;
                    }
                }
            }

            connection.Close();
        }
    }

    public void ClearLeaderboardEntries()
    {
        for (int i = leaderboardContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = leaderboardContainer.GetChild(i);

            // Only skip the original hidden template
            if (child.gameObject != entryTemplate)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
