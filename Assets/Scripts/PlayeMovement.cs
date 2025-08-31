using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f; // rotation douce
    [SerializeField] private float gravity = -9.81f; // gravité
    [SerializeField] private float groundedOffset = 0.1f; // distance pour vérifier le sol
    [SerializeField] private LayerMask groundLayer; // layer du sol

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private float verticalVelocity;
    private bool isGrounded;

    private void Start()
    {
        // Lock et cache le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Cherche tous les Player dans la scène
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 1)
        {
            // Il y a déjà un Player, donc on détruit celui-ci
            Destroy(gameObject);
            return;
        }

        // Sinon on le garde entre les scènes
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {

        if (!controller.enabled) return;
        CheckGround();

        // Récupérer la direction de la caméra
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        // Calculer direction relative à la caméra
        Vector3 direction = moveInput.x * camRight + moveInput.y * camForward;
        if (direction.magnitude > 1f)
            direction.Normalize();

        // Appliquer gravité
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        // Déplacement + gravité
        Vector3 velocity = direction * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Rotation douce
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Animation Walk / Idle
        float speedValue = direction.magnitude;
        animator.SetFloat("Speed", speedValue);
    }

    private void CheckGround()
    {
        // Vérifie si le personnage touche le sol
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * groundedOffset, 0.2f, groundLayer);
    }

    // Fonction appelée par PlayerInput
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
