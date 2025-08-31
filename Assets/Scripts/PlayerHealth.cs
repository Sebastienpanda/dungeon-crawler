using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [SerializeField] GameObject[] hearts;
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    private Animator animator;

    private Transform respawnPoint;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Assigner le respawn point selon la scène
        AssignRespawnPoint();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHud();
    }

    private void AssignRespawnPoint()
    {
        string spawnName = "SpawnPoint_" + SceneManager.GetActiveScene().name;
        GameObject spawnObj = GameObject.Find(spawnName);
        if (spawnObj != null)
        {
            respawnPoint = spawnObj.transform;
        }
        else
        {
            Debug.LogWarning("Spawn point introuvable pour cette scène : " + SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHud();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        if (animator != null)
            animator.SetTrigger("Die");

        // On ne stocke plus respawnPoint.position ici car il peut être détruit
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        GetComponent<CharacterController>().enabled = false;

        // Fade Out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = t / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1;

        // ⚡ Réassigner le spawn pour la scène courante
        AssignRespawnPoint();

        if (respawnPoint != null)
        {
            // Reset vie + déplacer joueur
            currentHealth = maxHealth;
            UpdateHud();
            transform.position = respawnPoint.position;
        }
        else
        {
            Debug.LogError("⚠ Aucun spawn trouvé dans la scène !");
        }

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

        GetComponent<CharacterController>().enabled = true;
        isDead = false;
    }

    private void UpdateHud()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].SetActive(i < currentHealth);
    }
}
