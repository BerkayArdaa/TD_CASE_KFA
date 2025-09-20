using UnityEngine;

public class BiteEffect : MonoBehaviour
{
    [Header("Efekt Ayarlar�")]
    [SerializeField] private GameObject biteEffectPrefab; // �al��t�r�lacak prefab
    [SerializeField] private float yOffset = 1f;          // Oyuncu �zerinde konumland�rma
    [SerializeField] private bool preventSpam = true;     // �oklu spawn engeli
    [SerializeField] private float minInterval = 0.1f;    // Minimum aral�k (flood �nleme)

    private float lastSpawnTime = -999f;
    private Transform player;

    void Awake()
    {
        // Player referans�n� bul
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

 
    /// Oyuncuya ba�l� bite efektini olu�turur.
    /// PlayerHealth.onDamaged event�ine Inspector �zerinden ba�lanmal�.
  
    public void Spawn()
    {
        if (biteEffectPrefab == null || player == null) return;
        if (preventSpam && Time.time - lastSpawnTime < minInterval) return;

        lastSpawnTime = Time.time;

        Vector3 pos = player.position + new Vector3(0f, yOffset, 0f);
        GameObject fx = Instantiate(biteEffectPrefab, pos, Quaternion.identity);

        // Takip komponenti varsa oyuncuya ba�la
        var follow = fx.GetComponent<BiteEffectFollower>();
        if (follow != null)
        {
            follow.SetTarget(player);
            follow.SetOffset(new Vector3(0f, yOffset, 0f));
        }
    }
}
