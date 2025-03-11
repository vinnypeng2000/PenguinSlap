using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;  // Singleton instance
    public int points;
    public int penalty;
    public TextMeshProUGUI scoreText;
    public int score = 0;

    public string assignedPenguin;
    public string assignedTiger;
    public string assignedHorse;
    private HashSet<int> slappedInstanceIDs = new HashSet<int>(); 

    void Awake()
    {
        // Singleton pattern to ensure only one ScoreManager exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AssignAnimals(string penguin, string tiger, string horse)
    {
        assignedPenguin = penguin;
        assignedTiger = tiger;
        assignedHorse = horse;
    }

    public void AddScore(GameObject enemy)
    {
        int enemyID = enemy.GetInstanceID();
        string enemyTag = enemy.tag;

        Debug.Log($"Slapped enemy: {enemy.name}, Tag: {enemyTag}, ID: {enemyID}");

        if (slappedInstanceIDs.Contains(enemyID))  
        {
            Debug.Log("Repeated slap detected! Deducting points.");
            score -= penalty;
        }
        else
        {
            if (enemyTag == assignedPenguin || enemyTag == assignedTiger || enemyTag == assignedHorse)
            {
                Debug.Log("Correct slap! Adding points.");
                score += points; 
            }
            else
            {
                Debug.Log("Incorrect slap! Deducting points.");
                score -= penalty; 
            }

            slappedInstanceIDs.Add(enemyID);
            Debug.Log($"Added ID {enemyID} to slapped list. Current slapped IDs: {string.Join(", ", slappedInstanceIDs)}");
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}
