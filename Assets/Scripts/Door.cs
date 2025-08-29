using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    [Header("Paramètres de la porte")]
    public float openAngle = 90f;
    public float speed = 2f;
    public bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Collider doorCollider;
    private bool lastState;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        doorCollider = GetComponent<Collider>();
        lastState = isOpen;
        UpdateCollider();
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);


        if (lastState != isOpen)
        {
            UpdateCollider();
            lastState = isOpen;
        }
    }


    private void UpdateCollider()
    {
        if (doorCollider != null)
            doorCollider.enabled = !isOpen;
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
