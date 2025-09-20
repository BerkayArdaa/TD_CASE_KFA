using UnityEngine;
using UnityEngine.AI;

public enum PathMode { OnceStop, Loop, PingPong }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathAgent : MonoBehaviour
{
    [Header("Rota")]
    public Transform[] waypoints;          // Ziyaret edilecek noktalar
    public float reachThreshold = 0.25f;   // Noktaya varmış sayılma mesafesi
    public float waitAtPoint = 0f;         // Noktada bekleme süresi
    public PathMode mode = PathMode.OnceStop;

    [Header("Hareket")]
    public float moveSpeed = 3.5f;         // Ajan hızı

    [Header("2D Sprite")]
    public bool faceVelocity2D = true;     // Hız yönüne bak (2D görünüm)

    NavMeshAgent agent;
    int index = 0;
    int dir = 1;
    float waitUntil = -1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        // 2D billboard için rotasyon/upAxis'i NavMeshAgent'a bırakma
        agent.updateRotation = !faceVelocity2D;
        agent.updateUpAxis = !faceVelocity2D;
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: waypoints boş.");
            enabled = false;
            return;
        }
        agent.SetDestination(waypoints[index].position);
    }

    void Update()
    {
        // Bekleme süresi aktifse
        if (waitUntil > 0f)
        {
            if (Time.time < waitUntil) return;
            waitUntil = -1f;
            SetNextDestination();
            return;
        }

        if (agent.pathPending) return;

        // Hedefe yeterince yaklaştıysa
        if (agent.remainingDistance <= reachThreshold)
        {
            if (waitAtPoint > 0f)
            {
                waitUntil = Time.time + waitAtPoint;
                agent.isStopped = true;
                return;
            }
            SetNextDestination();
        }
        else if (faceVelocity2D)
        {
            FaceVelocity2D();
        }
    }

    void SetNextDestination()
    {
        agent.isStopped = false;

        if (mode == PathMode.OnceStop)
        {
            index++;
            if (index >= waypoints.Length)
            {
                agent.isStopped = true; // Rota bitti
                return;
            }
        }
        else if (mode == PathMode.Loop)
        {
            index = (index + 1) % waypoints.Length;
        }
        else // PingPong
        {
            if (index == waypoints.Length - 1) dir = -1;
            else if (index == 0) dir = 1;
            index += dir;
        }

        agent.SetDestination(waypoints[index].position);
    }

    void FaceVelocity2D()
    {
        Vector3 v = agent.velocity; v.y = 0f;
        if (v.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(v);
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (!waypoints[i]) continue;
            Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);
            if (i + 1 < waypoints.Length && waypoints[i + 1])
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
