using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText; // "Score" ad�ndaki TMP Text'i buraya s�r�kle

    public int Score { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UpdateUI();
    }

    public void ResetScore()
    {
        Score = 0;
        UpdateUI();
    }

    public void Add(int points)
    {
        Score += points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = Score.ToString();
    }
}
