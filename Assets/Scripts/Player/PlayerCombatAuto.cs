using UnityEngine;
using System.Linq;

public enum AttackMode { Melee, Ranged }

public class PlayerCombatAuto : MonoBehaviour
{
    [Header("Targeting")]
    public float detectRadius = 12f;
    public LayerMask enemyMask;           // Enemy layer seç
    public Transform aimPivot;            // Sprite veya kafanýn baktýðý yer (opsiyonel)

    [Header("Attack")]
    public AttackMode attackMode = AttackMode.Ranged;
    public float attackRate = 2f;         // saniyede 2 atak
    public float meleeRange = 1.8f; 
    public int meleeDamage = 12;

    [Header("Ranged")]
    public Transform firePoint;           // mermi çýkýþ noktasý
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public int projectileDamage = 10;

    float nextAttackTime;
    /// <summary>
    /// ÖNEMLÝ NOT: TO DO--> Yakýn saldýrý modu mekaniksel anlamda mevcut. Ancak oyunda kullanýlmadý.
    /// </summary>
    void Update()
    {
        if (Time.time < nextAttackTime) return;

        Transform target = FindClosestEnemy();
        if (!target) return;

        //yüzünü hedefe çevir
        if (aimPivot)
        {
            Vector3 dir = target.position - aimPivot.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                aimPivot.rotation = Quaternion.LookRotation(dir);
        }

        if (attackMode == AttackMode.Melee)
        {
            float sqr = (target.position - transform.position).sqrMagnitude;
            if (sqr <= meleeRange * meleeRange)
            {
                var eh = target.GetComponent<EnemyHealth>();
                if (eh) eh.TakeDamage(meleeDamage);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else // Ranged
        {
            if (firePoint && projectilePrefab)
            {
                Vector3 dir = (target.position - firePoint.position).normalized;
                var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
                var proj = go.GetComponent<Projectile>();
                if (proj) proj.Init(projectileDamage, dir * projectileSpeed, enemyMask);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    Transform FindClosestEnemy()
    {
        var hits = Physics.OverlapSphere(transform.position, detectRadius, enemyMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return null;

        Collider best = hits
            .OrderBy(h => (h.transform.position - transform.position).sqrMagnitude)
            .First();

        return best.transform;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        if (attackMode == AttackMode.Melee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, meleeRange);
        }
    }
}
