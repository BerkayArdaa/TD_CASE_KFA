using UnityEngine;

public class BiteEffect : MonoBehaviour
{
    [Header("Efekt Ayarlarý")]
    [SerializeField] private GameObject biteEffectPrefab; // Çalýþtýrýlacak prefab
    [SerializeField] private float yOffset = 1f;          // Oyuncu üzerinde konumlandýrma
    [SerializeField] private bool preventSpam = true;     // Çoklu spawn engeli
    [SerializeField] private float minInterval = 0.1f;    // Minimum aralýk (flood önleme)

    private float lastSpawnTime = -999f;
    private Transform player;

    void Awake()
    {
        // Player referansýný bul
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

 
    /// Oyuncuya baðlý bite efektini oluþturur.
    /// PlayerHealth.onDamaged event’ine Inspector üzerinden baðlanmalý.
  
    public void Spawn()
    {
        if (biteEffectPrefab == null || player == null) return;
        if (preventSpam && Time.time - lastSpawnTime < minInterval) return;

        lastSpawnTime = Time.time;

        Vector3 pos = player.position + new Vector3(0f, yOffset, 0f);
        GameObject fx = Instantiate(biteEffectPrefab, pos, Quaternion.identity);

        // Takip komponenti varsa oyuncuya baðla
        var follow = fx.GetComponent<BiteEffectFollower>();
        if (follow != null)
        {
            follow.SetTarget(player);
            follow.SetOffset(new Vector3(0f, yOffset, 0f));
        }
    }
}
