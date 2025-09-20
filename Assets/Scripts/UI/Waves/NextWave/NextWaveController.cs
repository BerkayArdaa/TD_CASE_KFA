using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NextWaveController : MonoBehaviour
{
    [Header("Refs")]
    public WaveSpawner spawner;   // WaveSpawner referansı
    public Image buttonImage;    // UI Image (ikon)
    public Image cooldownFill;   // Filled Image (Radial 360)
    public CanvasGroup cg;        // butonu kısık yapmak için (opsiyonel)

    [Header("Input")]
    public KeyCode hotkey = KeyCode.F;

    [Header("Logic")]
    public float useCooldown = 3f;   // tekrar kullanmadan önce bekleme
    float cdLeft = 0f;

    [Header("Events")]
    public UnityEvent onUsed;        // SFX/anim bağlayabilirsin
    public UnityEvent onRejected;    // CD varken, vs.

    void Update()
    {
        // Tuş
        if (Input.GetKeyDown(hotkey))
            TryCall();
        Debug.Log($"[NextWave] cdLeft={cdLeft:F2}, fill={cooldownFill.fillAmount:F2}");


        // Cooldown geriye say
        if (cdLeft > 0f)
            cdLeft -= Time.deltaTime;

        // UI güncelle
        float t = Mathf.Clamp01(cdLeft / useCooldown);
        if (cooldownFill) cooldownFill.fillAmount = t;
        if (cg) cg.alpha = Mathf.Lerp(1f, 0.55f, t); // CD'de biraz soluk
        if (buttonImage) buttonImage.raycastTarget = (cdLeft <= 0f); // tıklanmasın
    }

    // UI Button OnClick() buraya bağla
    public void ClickButton() => TryCall();

    void TryCall()
    {
        if (cdLeft > 0f) { onRejected?.Invoke(); return; }

        if (spawner && spawner.StartNextWaveEarly())
        {
            Debug.Log($"[NextWave] Cooldown started {useCooldown}s");
            cdLeft = useCooldown;
            onUsed?.Invoke();
        }
        else
        {
            onRejected?.Invoke();
        }
    }
}
