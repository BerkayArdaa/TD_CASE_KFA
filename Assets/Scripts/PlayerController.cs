using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A-D veya Sol-Sað ok
        float v = Input.GetAxisRaw("Vertical");   // W-S veya Yukarý-Aþaðý ok

        Vector3 move = new Vector3(h, 0f, v).normalized;

        if (move.sqrMagnitude > 0.01f)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        // Yerçekimi
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
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
