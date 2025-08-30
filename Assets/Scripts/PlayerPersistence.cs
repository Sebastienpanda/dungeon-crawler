using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private void Awake()
    {
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
}
