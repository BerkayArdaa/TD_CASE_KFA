using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private WaveSelector waveSelector;
    [SerializeField] private string gameSceneName = "GameScene";

    public void PlayGame()
    {
        int startWave = waveSelector != null ? waveSelector.Value : 1;
        PlayerPrefs.SetInt("StartWave", startWave);
        PlayerPrefs.Save();
        Debug.Log("[MainMenu] Saved StartWave=" + startWave);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Çýkýþ yapýlýyor...");
        Application.Quit();
    }
}
