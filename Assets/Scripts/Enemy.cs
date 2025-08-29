using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 20;
    private Animator animator;
    private bool isDead = false;

    public AudioClip clinkSound;

    public GameObject coinPrefab;
    public int coinDropCount = 3;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;


        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;


        animator.SetTrigger("Die");

        DropCoins(coinDropCount);

        Destroy(gameObject, 2f);
    }

    private void DropCoins(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Position un peu aléatoire autour de l'ennemi
            Vector3 dropPos = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                0.5f,
                Random.Range(-0.5f, 0.5f)
            );

            Instantiate(coinPrefab, dropPos, Quaternion.identity);

            if (clinkSound != null)
            {
                AudioSource.PlayClipAtPoint(clinkSound, dropPos, 0.9f);
            }
        }
    }
}
