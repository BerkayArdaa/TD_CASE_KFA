using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;        // UI için
using TMPro;                // metin için

public class ArrowRainManager : MonoBehaviour
{
    [Header("Arrows")]
    [Tooltip("Boş bırakılırsa sahnedeki tüm ArrowProjectile'lar otomatik bulunur.")]
    public List<ArrowProjectile> arrows;
    public bool autoFindOnStart = true;
    public float initialDownSpeed = -1f;

    [Header("Trigger")]
    public KeyCode triggerKey = KeyCode.Q;

    [Header("Score Gate")]
    [Tooltip("Q ile tetiklemek için gereken minimum skor (tüketilecek miktar).")]
    public int scoreThreshold = 30;
    [Tooltip("ScoreManager yoksa buraya Score TMP_Text'i sürükleyin.")]
    public TMP_Text scoreText;

    [Header("Cooldown")]
    public float cooldown = 10f;     // 10 saniye
    float cdLeft = 0f;

    [Header("UI (opsiyonel)")]
    public Image iconImage;          // ana ikon (Arrow Rain simgesi)
    public Image cooldownFill;       // Filled/Radial 360; 1=full, 0=bos
    public TMP_Text cooldownLabel;   // kalan sn (örn: "8")
    public CanvasGroup cg;           // hazırken 1.0, değilken 0.6
    public bool hideTextWhenReady = true;

    void Start()
    {
        if (autoFindOnStart)
            arrows = FindObjectsOfType<ArrowProjectile>(includeInactive: true).ToList();

        foreach (var a in arrows)
        {
            if (a == null) continue;
            a.CaptureStart();
        }
        UpdateUI(force: true);
    }

    void Update()
    {
        // cooldown sayımı
        if (cdLeft > 0f)
        {
            cdLeft -= Time.deltaTime;
            if (cdLeft < 0f) cdLeft = 0f;
        }

        // tetik
        if (Input.GetKeyDown(triggerKey))
            TryUse();

        UpdateUI();
    }

    // UI butonuna bağlamak istersen:
    public void ClickButton() => TryUse();

    void TryUse()
    {
        if (cdLeft > 0f) return;                 // cooldown devam ediyorsa izin verme
        if (!TryConsumeScore()) return;          // skor yetmiyorsa izin verme

        // Aktifleştir
        DropAllFromSamePlace();
        cdLeft = cooldown;
        UpdateUI(force: true);
    }

    // Skoru yeterliyse threshold kadar düşürür ve true döner; yetmezse false.
    bool TryConsumeScore()
    {
        // 1) ScoreManager üzerinden
        if (ScoreManager.Instance != null)
        {
            int current = ScoreManager.Instance.Score;
            if (current >= scoreThreshold)
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

        Debug.LogWarning("ArrowRainManager: Skor kaynağı bulunamadı");
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

    // ---- UI yardımcıları ----
    bool HasEnoughScore()
    {
        if (ScoreManager.Instance != null) return ScoreManager.Instance.Score >= scoreThreshold;
        if (scoreText != null && int.TryParse(scoreText.text, out int s)) return s >= scoreThreshold;
        return true; // kaynak yoksa engelleme
    }

    void UpdateUI(bool force = false)
    {
        // fill
        if (cooldownFill && (force || cooldownFill.fillAmount != (cdLeft > 0 ? cdLeft / cooldown : 0f)))
            cooldownFill.fillAmount = (cooldown <= 0f) ? 0f : Mathf.Clamp01(cdLeft / cooldown);

        // sayı etiketi
        if (cooldownLabel)
        {
            if (cdLeft > 0f) cooldownLabel.text = Mathf.CeilToInt(cdLeft).ToString();
            else cooldownLabel.text = hideTextWhenReady ? "" : triggerKey.ToString();
        }

        // görsel durum
        if (cg)
        {
            bool ready = (cdLeft <= 0f) && HasEnoughScore();
            cg.alpha = ready ? 1f : 0.6f;
            // İstersen hazır değilken button raycast'ini de kapatabilirsin:
            if (iconImage) iconImage.raycastTarget = ready ? true : false;
        }
    }
}
