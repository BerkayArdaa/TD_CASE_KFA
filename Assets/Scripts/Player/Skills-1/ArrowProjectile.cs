using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Lifetime")]
    public bool reusable = true;            // tekrar kullanýlabilir (Destroy yerine disable)
    public float lifeTime = 8f;             // reusable=false ise emniyet ömrü
    public float destroyDelay = 0.1f;       // enemy'e vurunca
    public float groundDestroyDelay = 3f;   // yere saplanýnca

    [Header("Hit")]
    public float radius = 0.1f;
    public LayerMask hitMask;               // Enemy + Terrain
    public string terrainTag = "Terrain";

    [Header("Damage")]
    public int defaultDamage = 10;          // Init gelmezse bile 20 vursun

    [Header("Motion")]
    public Vector3 gravity = new Vector3(0, -25f, 0);
    public bool faceVelocity = true;

    // dahili
    int damage;                              // gerçek atýþ hasarý
    Vector3 velocity;
    Vector3 prevPos;
    bool stuckInGround = false;
    bool forceDrop = false;

    // tekrar kullaným için baþlangýç bilgisi
    Vector3 startPos;
    Quaternion startRot;

    // yönetici eriþimi isterse
    public static readonly List<ArrowProjectile> Active = new List<ArrowProjectile>();

    // ---- API ----
    public void CaptureStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void ResetToStart()
    {
        // collider'ý yeniden aç, state'i sýfýrla
        var col = GetComponent<Collider>();
        if (col) col.enabled = true;

        gameObject.SetActive(true);
        transform.SetPositionAndRotation(startPos, startRot);
        velocity = Vector3.zero;
        prevPos = transform.position;
        stuckInGround = false;
        forceDrop = false;
    }

    public void EnableDrop(float initialDownSpeed = -1f)
    {
        if (stuckInGround) return;
        forceDrop = true;
        velocity = new Vector3(0f, initialDownSpeed, 0f);
    }

    public void Init(int dmg, Vector3 initialVelocity, LayerMask mask)
    {
        damage = (dmg > 0) ? dmg : defaultDamage;   // <- garanti 20
        velocity = initialVelocity;
        hitMask = mask;

        prevPos = transform.position;

        if (!reusable)
            Destroy(gameObject, lifeTime);
    }
    // ---- /API ----

    void OnEnable()
    {
        if (!Active.Contains(this)) Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
    }

    void Awake()
    {
        prevPos = transform.position;
        CaptureStart(); // sahnedeyken baþlangýcý kaydet
    }

    void Update()
    {
        if (stuckInGround) return;

        float dt = Time.deltaTime;

        if (forceDrop)
            velocity = new Vector3(0f, velocity.y, 0f);

        velocity += gravity * dt;

        Vector3 step = velocity * dt;
        Vector3 nextPos = transform.position + step;

        if (Physics.SphereCast(prevPos, radius, (nextPos - prevPos).normalized,
                               out RaycastHit hit, (nextPos - prevPos).magnitude,
                               hitMask, QueryTriggerInteraction.Collide))
        {
            // Enemy
            var eh = hit.collider.GetComponentInParent<EnemyHealth>();
            if (eh)
            {
                int dmg = (damage > 0) ? damage : defaultDamage; // <- emniyet
                eh.TakeDamage(dmg);

                if (reusable) StartCoroutine(DisableAfter(destroyDelay));
                else Destroy(gameObject, destroyDelay);

                velocity = Vector3.zero;
                return;
            }

            // Terrain
            if (hit.collider.CompareTag(terrainTag))
            {
                transform.position = hit.point;
                Vector3 fwd = (forceDrop || velocity.sqrMagnitude <= 0.0001f)
                              ? -hit.normal
                              : velocity.normalized;
                transform.rotation = Quaternion.LookRotation(fwd, Vector3.up);

                velocity = Vector3.zero;
                stuckInGround = true;

                var col = GetComponent<Collider>();
                if (col) col.enabled = false;

                if (reusable) StartCoroutine(DisableAfter(groundDestroyDelay));
                else Destroy(gameObject, groundDestroyDelay);
                return;
            }

            // Diðer yüzey
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(
                (forceDrop || velocity.sqrMagnitude <= 0.0001f) ? -hit.normal : velocity.normalized,
                Vector3.up
            );
            velocity = Vector3.zero;
            stuckInGround = true;

            if (reusable) StartCoroutine(DisableAfter(groundDestroyDelay));
            else Destroy(gameObject, groundDestroyDelay);
            return;
        }
        else
        {
            // serbest hareket
            transform.position = nextPos;

            if (faceVelocity && velocity.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

            prevPos = transform.position;
        }
    }

    IEnumerator DisableAfter(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false); // reusable modda yok etmek yerine kapat
    }
}
