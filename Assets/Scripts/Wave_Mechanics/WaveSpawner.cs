using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Points & Path")]
    public Transform[] spawnPoints;          // d��manlar�n do�aca�� noktalar
    

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

    bool spawning;
    bool waitingNext;
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
        // Erken ba�latma
        if (waitingNext && Input.GetKeyDown(earlyStartKey))
        {
            waitingNext = false;
        }
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            // Bu dalgaya uygun d��man havuzunu haz�rla
            var pool = BuildPoolForWave(currentWave);

            bool isBossWave = (bossWaveEvery > 0) && (currentWave % bossWaveEvery == 0) && pool.Any(e => e.isBoss);

            int countThisWave = baseCount + (currentWave - 1) * countGrowthPerWave;

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

            // Sonraki dalga bekleme
            waitingNext = true;
            float t = 0f;
            while (waitingNext && t < timeBetweenWaves)
            {
                t += Time.deltaTime;
                yield return null;
            }
            waitingNext = false;

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
        return enemies.Where(e => e.prefab != null && wave >= Mathf.Max(1, e.unlockWave) && e.weight > 0f).ToList();
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

