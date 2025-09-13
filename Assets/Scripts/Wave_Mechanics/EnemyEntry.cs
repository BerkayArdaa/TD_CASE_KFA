using UnityEngine;

[System.Serializable]
public class EnemyEntry
{
    public string name;
    public GameObject prefab;
    [Tooltip("Bu dalgadan itibaren se�im havuzuna dahil olur (1-indexed).")]
    public int unlockWave = 1;
    [Tooltip("Rastgele se�imde a��rl�k. Y�ksek olursa daha s�k gelir.")]
    public float weight = 1f;
    [Tooltip("Bu enemy boss mu? (boss wave'lerde kullanmak i�in)")]
    public bool isBoss = false;
}
