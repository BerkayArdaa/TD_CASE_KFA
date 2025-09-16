using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { } // Inspector�dan (0..1) progress dinlemek i�in

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    [Header("Wave Events")]
    public IntEvent onWaveStarted;   // Dalga ba�lad���nda (dalga no)
    public IntEvent onWaveCleared;   // Dalga bitti�inde (dalga no)

    [Header("Spawn Points & Path")]
    public Transform[] spawnPoints;

    [Header("Enemy Pool")]
    public List<EnemyEntry> enemies = new List<EnemyEntry>();

    [Header("Wave Settings")]
    public int startWave = 1;                // oyuna ba�lama dalgas�
    public int baseCount = 5;                // 1. dalgadaki d��man say�s�
    public int countGrowthPerWave = 2;       // her dalga ba��na eklenecek d��man
    public float spawnInterval = 0.4f;       // bir d��man ile di�eri aras�ndaki s�re
    public float timeBetweenWaves = 5f;      // dalgalar aras� bekleme
    public KeyCode earlyStartKey = KeyCode.F;// erken ba�latma tu�u

    [Header("Boss Settings (optional)")]
    public int bossWaveEvery = 5;            // 0 ise kapal�; 5 ise 5.,10.,15. dalga boss
    public int bossCount = 1;

    [Header("Debug/Info")]
    public int currentWave;
    public int aliveEnemies;

    // ---- UI & State Exposure ----
    [Header("UI Events")]
    public UnityEvent onBetweenWavesStart;   // bekleme ba�lad���nda (UI�yi a�mak vs.)
    public FloatEvent onBetweenWavesTick;    // her frame 0..1 progress (doldurma i�in)
    public UnityEvent onBetweenWavesEnd;     // bekleme bitti�inde (UI�yi kapamak vs.)

    public bool IsWaitingNext => waitingNext;
    public float WaitDuration => timeBetweenWaves;
    public float WaitRemaining => Mathf.Max(0f, timeBetweenWaves - waitTimer);
    public float WaitProgress01 => (timeBetweenWaves <= 0f) ? 1f : Mathf.Clamp01(waitTimer / timeBetweenWaves);

    bool spawning;
    bool waitingNext;
    float waitTimer;                         // beklemede ge�en s�re
    Coroutine waveRoutine;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveSpawner: spawnPoints bo�.");
            enabled = false; return;
        }
        currentWave = Mathf.Max(1, startWave);
        waveRoutine = StartCoroutine(WaveLoop());
    }

    void Update()
    {
        // Klavye ile erken ba�latma
        if (waitingNext && Input.GetKeyDown(earlyStartKey))
            StartNextWaveEarly();
    }

    /// <summary>
    /// UI/tu� �a��r�m� i�in: beklemeyi kes ve bir sonraki dalgay� ba�lat.
    /// Ba�ar�l�ysa true d�ner (beklemede de�ilse false).
    /// </summary>
    public bool StartNextWaveEarly()
    {
        if (!waitingNext) return false;
        waitingNext = false;          // WaveLoop beklemeyi sonland�racak
        return true;
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            // Bu dalgaya uygun d��man havuzunu haz�rla
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

            // Dalga biti�i: t�m d��manlar�n �lmesini bekle
            while (aliveEnemies > 0) yield return null;

            onWaveCleared?.Invoke(currentWave);

            // Sonraki dalga bekleme (erken ba�latmaya izin ver)
            waitingNext = true;
            waitTimer = 0f;
            onBetweenWavesStart?.Invoke();

            while (waitingNext && waitTimer < timeBetweenWaves)
            {
                waitTimer += Time.deltaTime;
                onBetweenWavesTick?.Invoke(WaitProgress01); // 0..1 aras�
                yield return null;
            }

            // Bekleme bitti (ya s�re doldu ya da StartNextWaveEarly �a�r�ld�)
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
            Debug.LogWarning($"Wave {currentWave}: uygun d��man yok (pool bo�). Wave atland�.");
            yield break;
        }

        spawning = true;

        for (int i = 0; i < totalToSpawn; i++)
        {
            // Rastgele spawn point ve rastgele d��man se�
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var entry = WeightedPick(pool);

            SpawnOne(entry, sp.position, sp.rotation);

            // aral�k bekle
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

    // Bu dalga i�in a��lm�� enemy�lerden havuz olu�tur
    List<EnemyEntry> BuildPoolForWave(int wave)
    {
        return enemies.Where(e => e.prefab != null &&
                                  wave >= Mathf.Max(1, e.unlockWave) &&
                                  e.weight > 0f).ToList();
    }

    // A��rl�kl� se�im
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

    // Enemy �l�m geri �a��r�m�
    public void OnEnemyDied()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}

// Her d��mana eklenen minik yard�mc� (otomatik)
public class DeathRelay : MonoBehaviour
{
    WaveSpawner spawner;
    bool isBoss;
    bool notified = false; // g�venlik: tek sefer art�r

    public void Init(WaveSpawner s, bool boss)
    {
        spawner = s;
        isBoss = boss;
    }

    void OnDestroy()
    {
        if (notified) return;
        notified = true;

        // Wave sayac�
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
