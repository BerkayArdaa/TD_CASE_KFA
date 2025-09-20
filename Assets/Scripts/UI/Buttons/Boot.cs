using UnityEngine;

public class WaveSpawnerBootstrap : MonoBehaviour
{
    [SerializeField] private WaveSpawner spawner;
    [SerializeField] private int defaultWave = 1;

    void Awake() // ÖNEMLÝ: Awake
    {
        int startWave = PlayerPrefs.GetInt("StartWave", defaultWave);
        spawner.startWave = Mathf.Max(1, startWave);
        Debug.Log("[Bootstrap] Applied StartWave=" + spawner.startWave);
    }
}
