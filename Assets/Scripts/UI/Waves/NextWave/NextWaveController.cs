using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NextWaveController : MonoBehaviour
{
    [Header("Refs")]
    public WaveSpawner spawner;   // WaveSpawner referans�
    public Image buttonImage;    // UI Image (ikon)
    public Image cooldownFill;   // Filled Image (Radial 360)
    public CanvasGroup cg;        // butonu k�s�k yapmak i�in (opsiyonel)

    [Header("Input")]
    public KeyCode hotkey = KeyCode.F;

    [Header("Logic")]
    public float useCooldown = 3f;   // tekrar kullanmadan �nce bekleme
    float cdLeft = 0f;

    [Header("Events")]
    public UnityEvent onUsed;        // SFX/anim ba�layabilirsin
    public UnityEvent onRejected;    // CD varken, vs.

    void Update()
    {
        // Tu�
        if (Input.GetKeyDown(hotkey))
            TryCall();

        // Cooldown geriye say
        if (cdLeft > 0f)
            cdLeft -= Time.deltaTime;

        // UI g�ncelle
        float t = Mathf.Clamp01(cdLeft / useCooldown);
        if (cooldownFill) cooldownFill.fillAmount = t;
        if (cg) cg.alpha = Mathf.Lerp(1f, 0.55f, t); // CD'de biraz soluk
        if (buttonImage) buttonImage.raycastTarget = (cdLeft <= 0f); // t�klanmas�n
    }

    // UI Button OnClick() buraya ba�la
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
