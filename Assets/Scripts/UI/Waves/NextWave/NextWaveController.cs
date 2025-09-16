using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NextWaveController : MonoBehaviour
{
    [Header("Refs")]
    public WaveSpawner spawner;   // WaveSpawner referansý
    public Image buttonImage;    // UI Image (ikon)
    public Image cooldownFill;   // Filled Image (Radial 360)
    public CanvasGroup cg;        // butonu kýsýk yapmak için (opsiyonel)

    [Header("Input")]
    public KeyCode hotkey = KeyCode.F;

    [Header("Logic")]
    public float useCooldown = 3f;   // tekrar kullanmadan önce bekleme
    float cdLeft = 0f;

    [Header("Events")]
    public UnityEvent onUsed;        // SFX/anim baðlayabilirsin
    public UnityEvent onRejected;    // CD varken, vs.

    void Update()
    {
        // Tuþ
        if (Input.GetKeyDown(hotkey))
            TryCall();

        // Cooldown geriye say
        if (cdLeft > 0f)
            cdLeft -= Time.deltaTime;

        // UI güncelle
        float t = Mathf.Clamp01(cdLeft / useCooldown);
        if (cooldownFill) cooldownFill.fillAmount = t;
        if (cg) cg.alpha = Mathf.Lerp(1f, 0.55f, t); // CD'de biraz soluk
        if (buttonImage) buttonImage.raycastTarget = (cdLeft <= 0f); // týklanmasýn
    }

    // UI Button OnClick() buraya baðla
    public void ClickButton() => TryCall();

    void TryCall()
    {
        if (cdLeft > 0f) { onRejected?.Invoke(); return; }

        if (spawner && spawner.StartNextWaveEarly())
        {
            cdLeft = useCooldown;
            onUsed?.Invoke();
        }
        else
        {
            onRejected?.Invoke();
        }
    }
}
