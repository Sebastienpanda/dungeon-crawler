using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask doorLayer;

    public void OnInteract(InputAction.CallbackContext context)
    {

        if (!context.performed) return;

        Debug.Log("ok");

        // Utiliser la direction de la caméra pour viser
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = Camera.main.transform.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.red, 5f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactDistance /*, doorLayer */))
        {
            Debug.Log("Touched: " + hit.collider.name);
            if (hit.collider.TryGetComponent<Door>(out var door))
            {
                door.ToggleDoor();
                Debug.Log("Porte togglée !");
            }
        }
    }
}
