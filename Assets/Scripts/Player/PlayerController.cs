using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -20f;

    [Header("Dash")]
    public float dashSpeed = 20f;      // dash sýrasýnda hýz
    public float dashDuration = 0.2f;  // dash süresi
    public float dashCooldown = 1f;    // tekrar dash atabilmek için bekleme süresi

    [Header("Visual")]
    [SerializeField] private Transform visual;   // "Visual" child
    [SerializeField] private Animator animator;  // opsiyonel
    private Vector3 visualBaseScale;

    private CharacterController controller;
    private Vector3 velocity;

    private bool isDashing = false;
    private float dashEndTime;
    private float nextDashTime;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (visual == null) visual = transform.Find("Visual");
        if (visual != null)
        {
            visualBaseScale = visual.localScale;
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

        // Sprite yönü
        if (visual && Mathf.Abs(h) > 0.01f)
        {
            float sx = Mathf.Sign(h) * Mathf.Abs(visualBaseScale.x);
            visual.localScale = new Vector3(sx, visualBaseScale.y, visualBaseScale.z);
        }

        // Yerçekimi
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime && isMoving)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
        }

        // Hareket hesaplama
        Vector3 horizontal;
        if (isDashing)
        {
            horizontal = input * dashSpeed;
            if (Time.time >= dashEndTime) isDashing = false;
        }
        else
        {
            horizontal = input * moveSpeed;
        }

        Vector3 total = horizontal + velocity;
        controller.Move(total * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Item"))
        {
            Debug.Log("Iteme çarptý!");
        }
    }
}
