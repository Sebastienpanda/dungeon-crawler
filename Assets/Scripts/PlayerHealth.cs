using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // nombre de vies/cœurs
    private int currentHealth;

    private bool isDead = false;


    [SerializeField] GameObject[] hearts;

    private Animator animator;

    [Header("Respawn")]
    public Transform respawnPoint; // tu mets un empty GameObject dans ta scène = spawn
    public float respawnDelay = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHud();
    }

    // Appelé par l'ennemi quand il touche le joueur
    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Joueur touché ! Vies restantes : {currentHealth}");
        UpdateHud();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Joueur mort !");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        GetComponent<CharacterController>().enabled = false;

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void UpdateHud()
    {
        HideHearts();
        ShowActiveHearts();
    }

    private void HideHearts()
    {
        foreach (GameObject heart in hearts)
        {
            heart.SetActive(false);
        }
    }

    private void ShowActiveHearts()
    {
        for (int i = 0; i < currentHealth; i++)
        {
            hearts[i].SetActive(true);
        }
    }

    private void Respawn()
    {

        isDead = false;

        // Remet la vie
        currentHealth = maxHealth;
        UpdateHud();

        // Replace au spawn
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        // Réactive le joueur
        GetComponent<CharacterController>().enabled = true;

        if (animator != null)
        {
            animator.Rebind();   // remet tout à 0
            animator.Update(0f); // force la mise à jour
        }
    }
}
