using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform raycastOrigin; // assigné dans l’inspecteur
    [SerializeField] private float raycastDistance = 60f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Cooldown")]
    [SerializeField] private float interactCooldown = 0.5f;
    private float lastInteractTime = 0f;

    // Méthode appelée par l'action "Interact" (touche E)
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || Time.time < lastInteractTime + interactCooldown) return;
        lastInteractTime = Time.time;

        if (raycastOrigin == null)
        {
            Debug.LogWarning("Raycast origin non assigné !");
            return;
        }

        Vector3 origin = raycastOrigin.position;
        Vector3 direction = raycastOrigin.forward;

        Debug.DrawRay(origin, direction * raycastDistance, Color.red, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, raycastDistance, interactableLayer))
        {
            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                door.ToggleDoor();
                Debug.Log("Porte détectée et actionnée !");
            }
            else
            {
                Debug.Log("Objet touché mais pas de script Door.");
            }
        }
        else
        {
            Debug.Log("Aucun objet interactif détecté.");
        }
    }
}
