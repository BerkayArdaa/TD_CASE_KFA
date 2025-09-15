using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageZone : MonoBehaviour
{
    [Header("Damage")]
    public int dps = 8;                  // saniyedeki hasar
    public float tickInterval = 0.5f;    // kaç saniyede bir uygula
    public bool allowStack = false;      // ayný framede çifte vurma yok

    [Header("Filter")]
    public string playerTag = "Player";
    public string baseTag = "Base";

    // Son vurma zamanlarýný saklýyoruz
    private readonly Dictionary<Transform, float> lastHit = new();

    void OnTriggerStay(Collider other)
    {
        // Player mý Base mi?
        if (!(other.CompareTag(playerTag) || other.CompareTag(baseTag))) return;

        float now = Time.time;
        Transform target = other.transform;

        if (!lastHit.TryGetValue(target, out float tLast))
            tLast = -999f;

        if (now - tLast < tickInterval) return;

        // Hasarý hesapla (ör: dps * interval)
        int dmg = Mathf.Max(1, Mathf.RoundToInt(dps * tickInterval));

        if (other.CompareTag(playerTag))
        {
            var ph = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (ph) ph.TakeDamage(dmg);
        }
        else if (other.CompareTag(baseTag))
        {
            var bh = other.GetComponent<BaseHealth>() ?? other.GetComponentInParent<BaseHealth>();
            if (bh) bh.TakeDamage(dmg);
        }

        lastHit[target] = now;
    }
}
