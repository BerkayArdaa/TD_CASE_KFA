using UnityEngine;
using UnityEngine.AI;

public enum PathMode { OnceStop, Loop, PingPong }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathAgent : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;
    public float reachThreshold = 0.25f;
    public float waitAtPoint = 0f;
    public PathMode mode = PathMode.OnceStop;

    [Header("Movement")]
    public float moveSpeed = 3.5f;   // Inspector’dan ayarlanabilir hız

    [Header("2D Sprite Kullanıyorsan")]
    public bool faceVelocity2D = true;

    NavMeshAgent agent;
    int index = 0;
    int dir = 1;
    float waitUntil = -1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Inspector’dan aldığımız hız
        agent.speed = moveSpeed;

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
        if (waitUntil > 0f)
        {
            if (Time.time < waitUntil) return;
            waitUntil = -1f;
            SetNextDestination();
            return;
        }

        if (agent.pathPending) return;

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
                agent.isStopped = true;
                return;
            }
        }
        else if (mode == PathMode.Loop)
        {
            index = (index + 1) % waypoints.Length;
        }
        else
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
