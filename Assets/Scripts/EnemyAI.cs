using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool isMage = false;

    [Header("Audio")]
    [SerializeField] private AudioClip mageAttackSound;
    private AudioSource audioSource;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int currentPoint;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 5f;
    [Range(0, 180)] public float fieldOfView = 240f; // Champ de vision plus large

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Movement")]
    public NavMeshAgent agent;
    private Animator animator;

    // États pour debug
    private bool isPlayerDetected = false;
    private float currentDistanceToPlayer = 0f;
    private float currentAngleToPlayer = 0f;

    [SerializeField] private MageEnemy mageScript;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Auto-assigner le player s'il n'est pas défini
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        GoToNextPoint();
    }

    private void Update()
    {
        if (agent == null) return;

        // Réinitialiser la détection
        isPlayerDetected = false;

        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            currentDistanceToPlayer = directionToPlayer.magnitude;

            // Améliorer le calcul d'angle
            Vector3 forward = transform.forward;
            currentAngleToPlayer = Vector3.Angle(forward, directionToPlayer.normalized);

            // Conditions de détection
            bool inRange = currentDistanceToPlayer <= detectionRange;
            bool inFieldOfView = currentAngleToPlayer <= (fieldOfView / 2f);

            if (inRange && inFieldOfView)
            {
                isPlayerDetected = true;
            }

            // Vérifie qu'il n'y a pas d'obstacle
            if (isPlayerDetected)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer.normalized, out hit, detectionRange))
                {
                    if (hit.transform != player)
                    {
                        isPlayerDetected = false;
                    }
                }
            }
        }
        else
        {
            // Tentative de recherche automatique du joueur
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Logique de comportement
        if (isPlayerDetected)
        {
            HandlePlayerDetected();
        }
        else
        {
            HandlePatrol();
        }

        // Mise à jour de l'animation
        UpdateAnimation();
    }

    private void HandlePlayerDetected()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Poursuivre le joueur
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Arrêter et attaquer
            agent.isStopped = true;

            // Regarder vers le joueur
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    private void HandlePatrol()
    {
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            float speedPercent = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speedPercent);
        }
    }

    private void FireMageAttack()
    {
        mageScript.Fire();
    }

    private void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.TakeDamage(1);
        }

        if (isMage)
        {
            // Tir du mage
            Invoke(nameof(FireMageAttack), 0.5f);

            // Jouer un son de sort
            if (audioSource != null && mageAttackSound != null)
            {
                audioSource.PlayOneShot(mageAttackSound);
            }
        }
    }

    private void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, patrolPoints.Length);
        } while (randomIndex == currentPoint && patrolPoints.Length > 1);

        currentPoint = randomIndex;
        agent.destination = patrolPoints[currentPoint].position;
    }
}
