using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerTeleport : MonoBehaviour
{
    public string targetSceneName;
    public string spawnPointNameInTargetScene;
    public static string previousScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            previousScene = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.Find(spawnPointNameInTargetScene);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void ReturnToPreviousScene(string spawnPointName)
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            GameObject spawnPoint = GameObject.Find(spawnPointName);
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (spawnPoint != null && player != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
        };

        SceneManager.LoadScene(previousScene);
    }
}