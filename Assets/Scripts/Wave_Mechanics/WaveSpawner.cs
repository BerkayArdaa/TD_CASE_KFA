using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { } // Inspector’dan (0..1) progress dinlemek için

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    [Header("Wave Events")]
    public IntEvent onWaveStarted;   // Dalga baþladýðýnda (dalga no)
    public IntEvent onWaveCleared;   // Dalga bittiðinde (dalga no)

    [Header("Spawn Points & Path")]
    public Transform[] spawnPoints;

    [Header("Enemy Pool")]
    public List<EnemyEntry> enemies = new List<EnemyEntry>();

    [Header("Wave Settings")]
    public int startWave = 1;                // oyuna baþlama dalgasý
    public int baseCount = 5;                // 1. dalgadaki düþman sayýsý
    public int countGrowthPerWave = 2;       // her dalga baþýna eklenecek düþman
    public float spawnInterval = 0.4f;       // bir düþman ile diðeri arasýndaki süre
    public float timeBetweenWaves = 5f;      // dalgalar arasý bekleme
    public KeyCode earlyStartKey = KeyCode.F;// erken baþlatma tuþu

    [Header("Boss Settings (optional)")]
    public int bossWaveEvery = 5;            // 0 ise kapalý; 5 ise 5.,10.,15. dalga boss
    public int bossCount = 1;

    [Header("Debug/Info")]
    public int currentWave;
    public int aliveEnemies;

    // ---- UI & State Exposure ----
    [Header("UI Events")]
    public UnityEvent onBetweenWavesStart;   // bekleme baþladýðýnda (UI’yi açmak vs.)
    public FloatEvent onBetweenWavesTick;    // her frame 0..1 progress (doldurma için)
    public UnityEvent onBetweenWavesEnd;     // bekleme bittiðinde (UI’yi kapamak vs.)

    public bool IsWaitingNext => waitingNext;
    public float WaitDuration => timeBetweenWaves;
    public float WaitRemaining => Mathf.Max(0f, timeBetweenWaves - waitTimer);
    public float WaitProgress01 => (timeBetweenWaves <= 0f) ? 1f : Mathf.Clamp01(waitTimer / timeBetweenWaves);

    bool spawning;
    bool waitingNext;
    float waitTimer;                         // beklemede geçen süre
    Coroutine waveRoutine;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveSpawner: spawnPoints boþ.");
            enabled = false; return;
        }
        currentWave = Mathf.Max(1, startWave);
        waveRoutine = StartCoroutine(WaveLoop());
    }

    void Update()
    {
        // Klavye ile erken baþlatma
        if (waitingNext && Input.GetKeyDown(earlyStartKey))
            StartNextWaveEarly();
    }

    /// <summary>
    /// UI/tuþ çaðýrýmý için: beklemeyi kes ve bir sonraki dalgayý baþlat.
    /// Baþarýlýysa true döner (beklemede deðilse false).
    /// </summary>
    public bool StartNextWaveEarly()
    {
        if (!waitingNext) return false;
        waitingNext = false;          // WaveLoop beklemeyi sonlandýracak
        return true;
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            // Bu dalgaya uygun düþman havuzunu hazýrla
            var pool = BuildPoolForWave(currentWave);

            bool isBossWave = (bossWaveEvery > 0) &&
                              (currentWave % bossWaveEvery == 0) &&
                              pool.Any(e => e.isBoss);

            int countThisWave = baseCount + (currentWave - 1) * countGrowthPerWave;

            onWaveStarted?.Invoke(currentWave);

            if (isBossWave)
            {
                // Boss dalga: sadece boss(lar)
                yield return StartCoroutine(SpawnWave(pool.Where(e => e.isBoss).ToList(), bossCount));
            }
            else
            {
                // Normal dalga
                yield return StartCoroutine(SpawnWave(pool.Where(e => !e.isBoss).ToList(), countThisWave));
            }

            // Dalga bitiþi: tüm düþmanlarýn ölmesini bekle
            while (aliveEnemies > 0) yield return null;

            onWaveCleared?.Invoke(currentWave);

            // Sonraki dalga bekleme (erken baþlatmaya izin ver)
            waitingNext = true;
            waitTimer = 0f;
            onBetweenWavesStart?.Invoke();

            while (waitingNext && waitTimer < timeBetweenWaves)
            {
                waitTimer += Time.deltaTime;
                onBetweenWavesTick?.Invoke(WaitProgress01); // 0..1 arasý
                yield return null;
            }

            // Bekleme bitti (ya süre doldu ya da StartNextWaveEarly çaðrýldý)
            waitingNext = false;
            onBetweenWavesTick?.Invoke(1f);
            onBetweenWavesEnd?.Invoke();

            currentWave++;
        }
    }

    IEnumerator SpawnWave(List<EnemyEntry> pool, int totalToSpawn)
    {
        if (pool == null || pool.Count == 0)
        {
            Debug.LogWarning($"Wave {currentWave}: uygun düþman yok (pool boþ). Wave atlandý.");
            yield break;
        }

        spawning = true;

        for (int i = 0; i < totalToSpawn; i++)
        {
            // Rastgele spawn point ve rastgele düþman seç
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var entry = WeightedPick(pool);

            SpawnOne(entry, sp.position, sp.rotation);

            // aralýk bekle
            yield return new WaitForSeconds(spawnInterval);
        }

        spawning = false;
    }

    void SpawnOne(EnemyEntry entry, Vector3 pos, Quaternion rot)
    {
        var go = Instantiate(entry.prefab, pos, rot);
        aliveEnemies++;

        var deathRelay = go.AddComponent<DeathRelay>();
        deathRelay.Init(this, entry.isBoss); // <-- isBoss bilgisini pasla
    }

    // Bu dalga için açýlmýþ enemy’lerden havuz oluþtur
    List<EnemyEntry> BuildPoolForWave(int wave)
    {
        return enemies.Where(e => e.prefab != null &&
                                  wave >= Mathf.Max(1, e.unlockWave) &&
                                  e.weight > 0f).ToList();
    }

    // Aðýrlýklý seçim
    EnemyEntry WeightedPick(List<EnemyEntry> pool)
    {
        float total = pool.Sum(e => e.weight);
        float r = Random.value * total;
        float acc = 0f;
        foreach (var e in pool)
        {
            acc += e.weight;
            if (r <= acc) return e;
        }
        return pool[pool.Count - 1];
    }

    // Enemy ölüm geri çaðýrýmý
    public void OnEnemyDied()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}

// Her düþmana eklenen minik yardýmcý (otomatik)
public class DeathRelay : MonoBehaviour
{
    WaveSpawner spawner;
    bool isBoss;
    bool notified = false; // güvenlik: tek sefer artýr

    public void Init(WaveSpawner s, bool boss)
    {
        spawner = s;
        isBoss = boss;
    }

    void OnDestroy()
    {
        if (notified) return;
        notified = true;

        // Wave sayacý
        if (spawner != null)
            spawner.OnEnemyDied();

        // Skor
        if (ScoreManager.Instance != null)
        {
            int points = isBoss ? 100 : 10;
            ScoreManager.Instance.Add(points);
        }
    }
}
