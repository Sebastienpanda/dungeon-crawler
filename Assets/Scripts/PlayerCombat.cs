using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform swordPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioSource swordAudio; // Le composant AudioSource
    [SerializeField] private AudioClip slashSound;   // Le son de l'épée

    [SerializeField] private float attackCooldown = 0.6f;

    private Collider[] hitResults = new Collider[10];

    private Animator animator;

    private float nextAttackTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            animator.SetTrigger("Attack");
            PlaySlashSound();
            Attack();
        }
    }

    private void PlaySlashSound()
    {
        if (swordAudio != null && slashSound != null)
        {
            swordAudio.PlayOneShot(slashSound, 0.3f);
        }
    }

    private void Attack()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            swordPoint.position, attackRange, hitResults, enemyLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            if (hitResults[i].TryGetComponent<Enemy>(out var enemyScript))
            {
                enemyScript.TakeDamage(10);
            }
        }
    }
}
