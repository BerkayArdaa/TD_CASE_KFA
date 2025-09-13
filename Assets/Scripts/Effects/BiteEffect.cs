using UnityEngine;

public class BiteEffect : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField] private GameObject biteEffectPrefab; // Animatörlü prefab
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private bool preventSpam = true; // ayný anda birden fazla olmasýn
    [SerializeField] private float minInterval = 0.1f; // çok hýzlý hasarda flood önle

    private float lastSpawnTime = -999f;
    private Transform player;

    void Awake()
    {
        // Player tagli hedefi bul
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    // Bunu PlayerHealth.onDamaged event’ine baðla (Inspector)
    public void Spawn()
    {
        if (biteEffectPrefab == null || player == null) return;
        if (preventSpam && Time.time - lastSpawnTime < minInterval) return;

        lastSpawnTime = Time.time;

        Vector3 pos = player.position + new Vector3(0f, yOffset, 0f);
        GameObject fx = Instantiate(biteEffectPrefab, pos, Quaternion.identity);

        // Takip ve offset için bilgiyi ilet
        var follow = fx.GetComponent<BiteEffectFollower>();
        if (follow != null)
        {
            follow.SetTarget(player);
            follow.SetOffset(new Vector3(0f, yOffset, 0f));
        }
    }
}
