using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [Header("Refs")]
    public WaveSpawner spawner;

    [Header("Banner (Dalga Baþladý)")]
    public GameObject bannerRoot;      // Panel/Group
    public TMP_Text bannerText;        // "Dalga X baþladý!"
    public float bannerShowSeconds = 2f;

    [Header("Toast (Dalga Bitti)")]
    public GameObject endRoot;         // Panel/Group
    public TMP_Text endText;           // "Dalga X bitti!"
    public float endShowSeconds = 1.5f;

    [Header("Countdown (Between Waves)")]
    public GameObject countdownRoot;   // Panel/Group
    public TMP_Text countdownText;     // "Y. dalga Z sn sonra... [F ile erken]"
    public Image countdownFill;        // Image type = Filled (Horizontal/Radial)
    public bool invertFill = false;    // dolum ters olsun mu

    Coroutine bannerCo;
    Coroutine endCo;

    void Awake()
    {
        // Varsayýlan kapalý baþlat
        if (bannerRoot) bannerRoot.SetActive(false);
        if (endRoot) endRoot.SetActive(false);
        if (countdownRoot) countdownRoot.SetActive(false);
    }

    void OnEnable()
    {
        if (!spawner) spawner = FindObjectOfType<WaveSpawner>();
        if (!spawner)
        {
            Debug.LogError("WaveUI: WaveSpawner bulunamadý.");
            enabled = false; return;
        }

        // Event abonelikleri
        spawner.onWaveStarted.AddListener(HandleWaveStarted);
        spawner.onWaveCleared.AddListener(HandleWaveCleared);
        spawner.onBetweenWavesStart.AddListener(HandleBetweenStart);
        spawner.onBetweenWavesTick.AddListener(HandleBetweenTick);
        spawner.onBetweenWavesEnd.AddListener(HandleBetweenEnd);
    }

    void OnDisable()
    {
        if (!spawner) return;
        spawner.onWaveStarted.RemoveListener(HandleWaveStarted);
        spawner.onWaveCleared.RemoveListener(HandleWaveCleared);
        spawner.onBetweenWavesStart.RemoveListener(HandleBetweenStart);
        spawner.onBetweenWavesTick.RemoveListener(HandleBetweenTick);
        spawner.onBetweenWavesEnd.RemoveListener(HandleBetweenEnd);
    }

    // --- Event Handlers ---

    void HandleWaveStarted(int wave)
    {
        // Countdown & End tostu kapat
        if (countdownRoot) countdownRoot.SetActive(false);
        if (endRoot) endRoot.SetActive(false);

        // Banner göster
        if (bannerCo != null) StopCoroutine(bannerCo);
        bannerCo = StartCoroutine(CoShowTemp(bannerRoot, bannerText, $"{wave}. Wave Start!!", bannerShowSeconds));
    }

    void HandleWaveCleared(int wave)
    {
        // Banner kapat
        if (bannerRoot) bannerRoot.SetActive(false);

        // “Dalga bitti” kýsa tost
        if (endCo != null) StopCoroutine(endCo);
        endCo = StartCoroutine(CoShowTemp(endRoot, endText, $"{wave}. Wave Done", endShowSeconds));
    }

    void HandleBetweenStart()
    {
        if (countdownRoot) countdownRoot.SetActive(true);
        UpdateCountdownUI(0f); // baþlangýçta 0 ilerleme
    }

    void HandleBetweenTick(float progress01)
    {
        UpdateCountdownUI(progress01);
    }

    void HandleBetweenEnd()
    {
        if (countdownRoot) countdownRoot.SetActive(false);
    }

    // --- Helpers ---

    System.Collections.IEnumerator CoShowTemp(GameObject root, TMP_Text txt, string message, float seconds)
    {
        if (root)
        {
            root.SetActive(true);
            if (txt) txt.text = message;
        }
        yield return new WaitForSeconds(seconds);
        if (root) root.SetActive(false);
    }

    void UpdateCountdownUI(float progress01)
    {
        if (!spawner) return;

        // WaitRemaining, WaitDuration ve currentWave zaten WaveSpawner’da expose ediliyor
        float remain = spawner.WaitRemaining;           // saniye
        int nextWave = spawner.currentWave + 1;         // bekleme sýrasýnda currentWave biten dalgayý gösterir

        string earlyKey = spawner.earlyStartKey.ToString();
        string msg = $"{nextWave}. Wave Will Start {remain:0.0}";
        msg += $"   [{earlyKey}] Early Start";

        if (countdownText) countdownText.text = msg;

        if (countdownFill)
        {
            float f = Mathf.Clamp01(progress01);
            countdownFill.fillAmount = invertFill ? (1f - f) : f;
        }
    }
}
