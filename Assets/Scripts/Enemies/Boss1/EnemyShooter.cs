// EnemyShooter.cs
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Tespit")]
    public float detectRadius = 10f;       // Oyuncuyu algýlama yarýçapý
    public LayerMask playerMask;           // Oyuncu layer'ý
    public LayerMask obstructionMask;      // Engel layer'ý (opsiyonel)

    [Header("Saldýrý")]
    public float fireRate = 1f;            // Saniyedeki atýþ sayýsý
    public Transform firePoint;            // Merminin çýkýþ noktasý
    public GameObject slowProjectilePrefab;

    private float nextFireTime;

    void Update()
    {
        // Oyuncu menzilde mi?
        Collider[] hits = Physics.OverlapSphere(
            transform.position, detectRadius, playerMask, QueryTriggerInteraction.Collide
        );
        if (hits.Length == 0) return;

        Transform target = hits[0].transform;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        // Arada engel varsa atýþ yapma
        if (obstructionMask.value != 0)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir.normalized,
                                out RaycastHit block, detectRadius, obstructionMask))
                return;
        }

        // Sprite yönünü hedefe çevir
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

        // Ateþ etme kontrolü
        if (Time.time >= nextFireTime)
        {
            Fire(target.position);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Fire(Vector3 targetPos)
    {
        if (!slowProjectilePrefab || !firePoint) return;

        Vector3 dir = (targetPos - firePoint.position).normalized;
        Vector3 spawn = firePoint.position + dir * 0.2f;

        Instantiate(slowProjectilePrefab, spawn, Quaternion.LookRotation(dir));
    }

    // Editörde menzil çizimi
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
