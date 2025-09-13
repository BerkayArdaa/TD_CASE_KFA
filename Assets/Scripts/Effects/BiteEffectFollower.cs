using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BiteEffectFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    private Animator animator;
    private float scheduledDestroyTime = -1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Animasyon süresine göre kendini yok etmeyi planla
        if (animator != null)
        {
            // Geçerli state süresi (Layer 0)
            float len = animator.GetCurrentAnimatorStateInfo(0).length;
            scheduledDestroyTime = Time.time + len;
        }
    }

    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;

        // Güvenli kapatma (animasyon bitimi)
        if (scheduledDestroyTime > 0f && Time.time >= scheduledDestroyTime)
            Destroy(gameObject);
    }

    // Animation Event ile de çaðrýlabilir (animasyon son frame)
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetTarget(Transform t) => target = t;
    public void SetOffset(Vector3 o) => offset = o;
}
