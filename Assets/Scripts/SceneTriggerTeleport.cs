using UnityEngine;

public class SceneTriggerTeleport : MonoBehaviour
{
    public string targetSceneName;
    public string destinationPortalID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && TeleportManager.Instance != null)
        {
            TeleportManager.Instance.TeleportToScene(targetSceneName, destinationPortalID);
        }
        else
        {
            Debug.LogWarning("TeleportManager.Instance is null � did you forget to add it to the scene?");
        }
    }
}
