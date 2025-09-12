using UnityEngine;
using UnityEngine.AI;

public enum PathMode { OnceStop, Loop, PingPong }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathAgent : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;          // sırayla gidilecek noktalar
    public float reachThreshold = 0.25f;   // noktayı varmış sayma mesafesi
    public float waitAtPoint = 0f;         // her noktada bekleme süresi (sn)
    public PathMode mode = PathMode.OnceStop;

    [Header("2D Sprite Kullanıyorsan")]
    public bool faceVelocity2D = true;     // sadece Y ekseninde dönsün

    NavMeshAgent agent;
    int index = 0;
    int dir = 1;                           // ping-pong için yön
    float waitUntil = -1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // 2D billboard görünüm için genelde:
        agent.updateRotation = !faceVelocity2D ? true : false;
        agent.updateUpAxis = !faceVelocity2D ? true : false;
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
        // Noktada bekleme
        if (waitUntil > 0f)
        {
            if (Time.time < waitUntil) return;
            waitUntil = -1f;
            SetNextDestination(); // bekleme bitince ilerle
            return;
        }

        if (agent.pathPending) return;

        // Ulaştı mı?
        if (agent.remainingDistance <= reachThreshold)
        {
            // Noktada bekleme istiyorsan
            if (waitAtPoint > 0f) { waitUntil = Time.time + waitAtPoint; agent.isStopped = true; return; }

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

        // Sonraki index hesapla
        if (mode == PathMode.OnceStop)
        {
            index++;
            if (index >= waypoints.Length)
            {
                agent.isStopped = true;    // son noktada dur
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
