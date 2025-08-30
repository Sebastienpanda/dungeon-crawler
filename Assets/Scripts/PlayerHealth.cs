using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // nombre de vies/cœurs
    private int currentHealth;

    private bool isDead = false;


    [SerializeField] GameObject[] hearts;

    [SerializeField] private CanvasGroup fadePanel; // panel UI noir couvrant l'écran
    [SerializeField] private float fadeDuration = 0.5f;

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

        StartCoroutine(RespawnCoroutine(respawnPoint.position));
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

    private IEnumerator RespawnCoroutine(Vector3 respawnPos)
    {
        // Désactiver le contrôleur pendant le fade
        GetComponent<CharacterController>().enabled = false;

        // Fade Out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = t / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1;

        // Déplacer joueur + reset vie
        currentHealth = maxHealth;
        UpdateHud();
        transform.position = respawnPos;

        // Reset animation
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        // Fade In
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = 1 - t / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 0;

        // Réactiver le contrôleur
        GetComponent<CharacterController>().enabled = true;
        isDead = false;
    }
}