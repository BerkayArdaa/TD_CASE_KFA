using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BiteEffectFollower : MonoBehaviour
{
    [SerializeField] private Transform target;              // Takip edilecek obje
    [SerializeField] private Vector3 offset = new(0f, 1f, 0f); // Konum offseti

    private Animator animator;
    private float scheduledDestroyTime = -1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Aktif animasyon süresine göre yok olma zamanýný ayarla
        if (animator != null)
        {
            float len = animator.GetCurrentAnimatorStateInfo(0).length;
            scheduledDestroyTime = Time.time + len;
        }
    }

    void LateUpdate()
    {
        // Hedefi takip et
        if (target != null)
            transform.position = target.position + offset;

        // Animasyon süresi dolduysa objeyi yok et
        if (scheduledDestroyTime > 0f && Time.time >= scheduledDestroyTime)
            Destroy(gameObject);
    }

    // Alternatif: animasyonun son frame’ine event eklenerek çaðrýlabilir
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetTarget(Transform t) => target = t;
    public void SetOffset(Vector3 o) => offset = o;
}
