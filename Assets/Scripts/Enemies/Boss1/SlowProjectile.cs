using UnityEngine;

public class SlowProjectile : MonoBehaviour
{
    public float speed = 14f;
    public float lifeTime = 4f;
    public float hitRadius = 0.12f;       // SphereCast yarýçapý
    public LayerMask playerMask;          // Player layer'ý
    public float slowMultiplier = 0.5f;   // %50 hýz
    public float slowDuration = 2f;       // 2 sn

    Vector3 prev;

    void Start()
    {
        prev = transform.position;
        Destroy(gameObject, lifeTime);    // güvenlik için kendi kendini yok et
    }

    void Update()
    {
        Vector3 step = transform.forward * speed * Time.deltaTime;
        Vector3 next = transform.position + step;

        // prev -> next çizgisi boyunca var mý?
        if (Physics.SphereCast(prev, hitRadius, (next - prev).normalized,
                               out RaycastHit hit, (next - prev).magnitude,
                               playerMask, QueryTriggerInteraction.Collide))
        {
            // Player'ý bulup yavaþlat
            var slow = hit.collider.GetComponentInParent<PlayerSlow>();
            if (slow) slow.ApplySlow(slowMultiplier, slowDuration);

            // hemen yok ol
            Destroy(gameObject);
            return;
        }

        transform.position = next;
        prev = transform.position;
    }
}
