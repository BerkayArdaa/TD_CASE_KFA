using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Score'u UI'dan okuyabilmek için

public class ArrowRainManager : MonoBehaviour
{
    [Header("Arrows")]
    [Tooltip("Boş bırakılırsa sahnedeki tüm ArrowProjectile'lar otomatik bulunur.")]
    public List<ArrowProjectile> arrows;

    [Tooltip("Sahneden otomatik bulmak için True yap. (Tag/Component araması)")]
    public bool autoFindOnStart = true;

    [Tooltip("Reset sonrası ilk düşüş hızı (aşağı doğru)")]
    public float initialDownSpeed = -1f;

    [Header("Score Gate")]
    [Tooltip("Q ile tetiklemek için gereken minimum skor (tüketilecek miktar).")]
    public int scoreThreshold = 30;

    [Tooltip("ScoreManager yoksa buraya Score TMP_Text'i sürükleyin.")]
    public TMP_Text scoreText;

    void Start()
    {
        if (autoFindOnStart)
            arrows = FindObjectsOfType<ArrowProjectile>(includeInactive: true).ToList();

        foreach (var a in arrows)
        {
            if (a == null) continue;
            a.CaptureStart();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && TryConsumeScore())
        {
            DropAllFromSamePlace();
        }
    }

    // Skoru yeterliyse threshold kadar düşürür ve true döner; yetmezse false.
    bool TryConsumeScore()
    {
        // 1) ScoreManager üzerinden
        if (ScoreManager.Instance != null)
        {
            int current = ScoreManager.Instance.Score;
            if (current >= scoreThreshold)               // >= istersen > yapabilirsin
            {
                ScoreManager.Instance.Add(-scoreThreshold);
                return true;
            }
            return false;
        }

        // 2) UI metninden (Fallback)
        if (scoreText != null && int.TryParse(scoreText.text, out int s))
        {
            if (s >= scoreThreshold)
            {
                int newScore = Mathf.Max(0, s - scoreThreshold);
                scoreText.text = newScore.ToString();
                return true;
            }
            return false;
        }

        Debug.LogWarning("ArrowRainManager: Skor kaynağı bulunamadı (ScoreManager ya da scoreText atayın).");
        return false;
    }

    public void DropAllFromSamePlace()
    {
        foreach (var a in arrows)
        {
            if (a == null) continue;

            if (!a.gameObject.activeSelf)
                a.gameObject.SetActive(true);

            a.ResetToStart();
            a.EnableDrop(initialDownSpeed);
        }
    }
}
