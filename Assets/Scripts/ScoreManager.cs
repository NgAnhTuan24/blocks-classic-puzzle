using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score { get; private set; }
    public int level { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    public int pointsPerLevel = 1000;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ResetAll();
    }

    public void ResetAll()
    {
        score = 0;
        level = 1;
        RefreshUI();
    }

    public void AddScore(int amount)
    {
        score += amount;

        int newLevel = CalculateLevel(score);
        if (newLevel != level)
        {
            level = newLevel;
        }

        RefreshUI();
    }

    private int CalculateLevel(int currentScore)
    {
        int lvl = 1;
        int requiredScore = 0;

        while (true)
        {
            int nextRequirement = pointsPerLevel * lvl;

            if (currentScore < requiredScore + nextRequirement)
                break;

            requiredScore += nextRequirement;
            lvl++;
        }

        return lvl;
    }

    public void OnLinesCleared(int lines)
    {
        int points = 0;
        switch (lines)
        {
            case 1: points = 100; break;
            case 2: points = 300; break;
            case 3: points = 500; break;
            case 4: points = 800; break;
        }

        AddScore(points * level);
    }

    private void RefreshUI()
    {
        if (scoreText != null) scoreText.text = $"{score}";
        if (levelText != null) levelText.text = $"{level}";
    }

    public float GetFallSpeed()
    {
        return Mathf.Pow(0.8f, level - 1);
    }
}
