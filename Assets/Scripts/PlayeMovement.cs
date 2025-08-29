using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedOffset = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private float verticalVelocity;
    private bool isGrounded;

    private Camera cam;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (!controller.enabled) return;

        CheckGround();

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        Vector3 direction = moveInput.x * camRight + moveInput.y * camForward;
        if (direction.magnitude > 1f)
            direction.Normalize();

        ApplyGravity();

        Vector3 velocity = direction * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        RotatePlayer(direction);
        UpdateAnimation(direction.magnitude);
    }


    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * groundedOffset, 0.2f, groundLayer);
    }


    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimation(float speed)
    {
        if (animator != null)
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }
}
