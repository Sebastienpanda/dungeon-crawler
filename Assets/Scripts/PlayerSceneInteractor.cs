using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSceneInteractor : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer; // La layer du Empty avec SceneTeleport

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("ok0");
        if (!context.performed) return;
        Debug.Log("ok0");
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;
        Debug.Log("ok1");
        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactDistance, interactableLayer))
        {
            Debug.Log("ok2");

        }
    }
}
