using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 velocity;

    [SerializeField] private Transform visual;   // Player/Visual child
    private Animator animator;
    private Vector3 visualBaseScale;             // orijinal ölçek

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (visual == null) visual = transform.Find("Visual");
        animator = visual.GetComponent<Animator>();

        visualBaseScale = visual.localScale;     // örn. (5,5,5)
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(h, 0f, v).normalized;

        bool isMoving = move.sqrMagnitude > 0.01f;
        animator.SetBool("IsMove", isMoving);

        if (isMoving)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);

            // SAÐ/ SOL bakýþ — sadece iþareti deðiþtir, büyüklük ayný kalsýn
            if (h > 0)
                visual.localScale = new Vector3(Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
            else if (h < 0)
                visual.localScale = new Vector3(-Mathf.Abs(visualBaseScale.x), visualBaseScale.y, visualBaseScale.z);
        }

        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Item"))
        {
            Debug.Log("Iteme çarptý!");
            // Örn. item alýnsýn:
            // Destroy(hit.gameObject);
        }
    }
}







