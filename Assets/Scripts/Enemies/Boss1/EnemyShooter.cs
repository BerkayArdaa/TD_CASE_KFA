// EnemyShooter.cs  (yeni enemy tipine ekle)
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    [Header("Detection")]
    public float detectRadius = 10f;
    public LayerMask playerMask;        // sadece Player layer'ý
    public LayerMask obstructionMask;   // (opsiyon) engeller (duvar, kaya) görüyü keser

    [Header("Attack")]
    public float fireRate = 1.0f;       // saniyede 1 atýþ
    public Transform firePoint;
    public GameObject slowProjectilePrefab;

    float nextFireTime;

    void Update()
    {
        // Menziðe giren player var mý?
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerMask, QueryTriggerInteraction.Collide);
        if (hits.Length == 0) return;

        Transform target = hits[0].transform; // en yakýn yerine basitçe ilkini al
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;

        // (Opsiyon) Görüþ kontrolü
        if (obstructionMask.value != 0)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir.normalized, out RaycastHit block, detectRadius, obstructionMask))
                return; // arada engel var
        }

        // sprite yönü (2D görünüm için)
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

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
        float spawnOffset = 0.2f;
        Vector3 spawn = firePoint.position + dir * spawnOffset;

        Instantiate(slowProjectilePrefab, spawn, Quaternion.LookRotation(dir));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
