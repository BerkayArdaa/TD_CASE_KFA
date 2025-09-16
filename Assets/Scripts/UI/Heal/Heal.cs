using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Heal : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth player;        // PlayerHealth referansı
    public ScoreManager scoreManager;  // Varsa kullanılır (Score property + Add(int delta))
    public TMP_Text scoreText;         // Score UI’dan okunacaksa (ScoreManager yoksa)
    public Image cooldownFill;         // Type=Filled, FillMethod=Radial360 olmalı (opsiyonel)

    [Header("Settings")]
    public KeyCode key = KeyCode.E;
    public int healAmount = 20;        // Verilecek can
    public int scoreCost = 200;        // Skordan düşülecek miktar
    public float cooldownSeconds = 10f;// Cooldown süresi

    [Header("Events (opsiyonel)")]
    public UnityEngine.Events.UnityEvent onHealSuccess;
    public UnityEngine.Events.UnityEvent onNotReady;   // cooldown, skor yetersiz, hp full vb.

    float cdLeft = 0f;

    void Awake()
    {
        if (cooldownFill) cooldownFill.fillAmount = 0f; // 0 = hazır
    }

    void Update()
    {
        // Cooldown sayaç
        if (cdLeft > 0f)
        {
            cdLeft -= Time.deltaTime;
            if (cdLeft < 0f) cdLeft = 0f;
        }

        // UI dolgu (1 → yeni basıldı, 0 → hazır)
        if (cooldownFill)
        {
            cooldownFill.type = Image.Type.Filled;
            cooldownFill.fillMethod = Image.FillMethod.Radial360;
            cooldownFill.fillAmount = (cooldownSeconds > 0f) ? (cdLeft / cooldownSeconds) : 0f;
        }

        // Girdi
        if (Input.GetKeyDown(key))
            TryHeal();
    }

    void TryHeal()
    {
        if (!player) { onNotReady?.Invoke(); return; }

        // HP ful ise hiçbir şey yapma
        if (player.currentHP >= player.maxHP)
        {
            onNotReady?.Invoke();
            return;
        }

        // Cooldown devam ediyor mu?
        if (cdLeft > 0f)
        {
            onNotReady?.Invoke();
            return;
        }

        // Skor yeterli mi ve düşebiliyor muyuz?
        if (!TrySpend(scoreCost))
        {
            onNotReady?.Invoke();
            return;
        }

        // Heal + cooldown başlat
        player.Heal(healAmount);
        cdLeft = cooldownSeconds;
        if (cooldownFill) cooldownFill.fillAmount = 1f;

        onHealSuccess?.Invoke();
    }

    // ————————————————————————————————————————
    // Skor yardımcıları (ScoreManager varsa onu, yoksa UI metnini kullanır)
    int GetScore()
    {
        if (scoreManager != null) return scoreManager.Score;

        if (scoreText != null)
        {
            // Tam sayıysa direkt al
            if (int.TryParse(scoreText.text, out int s)) return s;

            // Karışık metinden ilk rakamları çek
            int acc = 0; bool found = false;
            foreach (char c in scoreText.text)
            {
                if (char.IsDigit(c)) { acc = acc * 10 + (c - '0'); found = true; }
                else if (found) break;
            }
            return found ? acc : 0;
        }
        return 0;
    }

    bool TrySpend(int amount)
    {
        if (scoreManager != null)
        {
            if (scoreManager.Score < amount) return false;
            scoreManager.Add(-amount);
            return true;
        }

        if (scoreText != null)
        {
            int cur = GetScore();
            if (cur < amount) return false;
            scoreText.text = (cur - amount).ToString();
            return true;
        }

        return false;
    }
}
