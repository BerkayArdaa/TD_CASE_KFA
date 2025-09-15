using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -20f;

    [Header("Visual")]
    [SerializeField] private Transform visual;   // "Visual" child
    [SerializeField] private Animator animator;  // opsiyonel
    private Vector3 visualBaseScale;

    private CharacterController controller;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (visual == null) visual = transform.Find("Visual");
        if (visual != null)
        {
            visualBaseScale = visual.localScale;               // �rn. (5,5,5)
            if (animator == null) animator = visual.GetComponent<Animator>();
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        bool isMoving = input.sqrMagnitude > 0.01f;
        if (animator) animator.SetBool("IsMove", isMoving);

        // Sprite'� sola/sa�a �evir (sadece X i�areti)
        if (visual && Mathf.Abs(h) > 0.01f)
        {
            float sx = Mathf.Sign(h) * Mathf.Abs(visualBaseScale.x);
            visual.localScale = new Vector3(sx, visualBaseScale.y, visualBaseScale.z);
        }

        // Yer�ekimi: �nce grounded reset
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // Tek Move �a�r�s� (yatay + d��ey)
        Vector3 horizontal = input * moveSpeed;
        Vector3 total = horizontal + velocity;
        controller.Move(total * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Item"))
        {
            Debug.Log("Iteme �arpt�!");
            // �rn: Destroy(hit.gameObject);
        }
    }
}
