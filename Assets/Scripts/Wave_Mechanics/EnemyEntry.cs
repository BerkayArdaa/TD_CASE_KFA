using UnityEngine;

[System.Serializable]
public class EnemyEntry
{
    public string name;
    public GameObject prefab;
    [Tooltip("Bu dalgadan itibaren seçim havuzuna dahil olur (1-indexed).")]
    public int unlockWave = 1;
    [Tooltip("Rastgele seçimde aðýrlýk. Yüksek olursa daha sýk gelir.")]
    public float weight = 1f;
    [Tooltip("Bu enemy boss mu? (boss wave'lerde kullanmak için)")]
    public bool isBoss = false;
}
