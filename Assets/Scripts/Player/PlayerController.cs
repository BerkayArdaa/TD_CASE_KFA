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
    private Vector3 visualBaseScale;             // orijinal �l�ek

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (visual == null) visual = transform.Find("Visual");
        animator = visual.GetComponent<Animator>();

        visualBaseScale = visual.localScale;     // �rn. (5,5,5)
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

            // SA�/ SOL bak�� � sadece i�areti de�i�tir, b�y�kl�k ayn� kals�n
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
            Debug.Log("Iteme �arpt�!");
            // �rn. item al�ns�n:
            // Destroy(hit.gameObject);
        }
    }
}







