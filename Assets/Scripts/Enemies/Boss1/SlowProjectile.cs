using UnityEngine;

public class SlowProjectile : MonoBehaviour
{
    public float speed = 14f;
    public float lifeTime = 4f;
    public float hitRadius = 0.12f;       // SphereCast yar��ap�
    public LayerMask playerMask;          // Player layer'�
    public float slowMultiplier = 0.5f;   // %50 h�z
    public float slowDuration = 2f;       // 2 sn

    Vector3 prev;

    void Start()
    {
        prev = transform.position;
        Destroy(gameObject, lifeTime);    // g�venlik i�in kendi kendini yok et
    }

    void Update()
    {
        Vector3 step = transform.forward * speed * Time.deltaTime;
        Vector3 next = transform.position + step;

        // prev -> next �izgisi boyunca var m�?
        if (Physics.SphereCast(prev, hitRadius, (next - prev).normalized,
                               out RaycastHit hit, (next - prev).magnitude,
                               playerMask, QueryTriggerInteraction.Collide))
        {
            // Player'� bulup yava�lat
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
