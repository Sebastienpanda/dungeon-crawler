using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance;

    public string incomingPortalID;
    public bool teleportCooldown = false;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TeleportToScene(string targetSceneName, string destinationPortalID)
    {
        if (teleportCooldown) return;

        StartCoroutine(DoTeleportWithFade(targetSceneName, destinationPortalID));
    }

    private IEnumerator DoTeleportWithFade(string targetSceneName, string destinationPortalID)
    {
        teleportCooldown = true;
        incomingPortalID = destinationPortalID;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Charger la scène
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(targetSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(HandleTeleportWithFade());
    }

    private IEnumerator HandleTeleportWithFade()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Portal[] portals = FindObjectsOfType<Portal>();

        foreach (Portal portal in portals)
        {
            if (portal.portalID == incomingPortalID)
            {
                CharacterController controller = player.GetComponent<CharacterController>();
                if (controller != null) controller.enabled = false;

                player.transform.position = portal.transform.position;

                if (controller != null) controller.enabled = true;

                break;
            }
        }

        // Petite pause pour être sûr que tout est en place
        yield return null;

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));

        teleportCooldown = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeCanvas == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        fadeCanvas.alpha = endAlpha;
    }
}
