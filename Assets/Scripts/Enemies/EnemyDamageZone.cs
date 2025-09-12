using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageZone : MonoBehaviour
{
    [Header("Damage")]
    public int dps = 8;                  // saniyedeki hasar
    public float tickInterval = 0.5f;    // 0.5 sn’de bir uygula (dps * 0.5 kadar)
    public bool allowStack = false;      // ayný framede çifte vurma yok

    [Header("Filter")]
    public string playerTag = "Player";

    private readonly Dictionary<Transform, float> lastHit = new();

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var ph = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
        if (!ph) return;

        float now = Time.time;
        if (!lastHit.TryGetValue(ph.transform, out var tLast)) tLast = -999f;
        if (now - tLast < tickInterval) return;

        int dmg = Mathf.Max(1, Mathf.RoundToInt(dps * tickInterval));
        ph.TakeDamage(dmg);

        lastHit[ph.transform] = now;
    }
}
