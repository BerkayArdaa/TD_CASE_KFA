using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 4f;
    public float radius = 0.1f;     // yakýn vuruþ kaçaklarýný azaltýr
    public float destroyDelay = 0.1f; // çarpýnca yok olma süresi

    int damage;
    Vector3 velocity;
    LayerMask hitMask;

    Vector3 prevPos;

    public void Init(int dmg, Vector3 vel, LayerMask mask)
    {
        damage = dmg;
        velocity = vel;
        hitMask = mask;
        prevPos = transform.position;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        Vector3 step = velocity * Time.deltaTime;
        Vector3 nextPos = transform.position + step;

        if (Physics.SphereCast(prevPos, radius, (nextPos - prevPos).normalized,
                               out RaycastHit hit, (nextPos - prevPos).magnitude,
                               hitMask, QueryTriggerInteraction.Collide))
        {
            var eh = hit.collider.GetComponentInParent<EnemyHealth>();
            if (eh) eh.TakeDamage(damage);

            // hemen yok etmek yerine destroyDelay kadar beklet
            Destroy(gameObject, destroyDelay);

            // Ýstersen mermiyi hareket ettirmeyi býrak:
            velocity = Vector3.zero;
        }
        else
        {
            transform.position = nextPos;
            prevPos = transform.position;
        }
    }
}
