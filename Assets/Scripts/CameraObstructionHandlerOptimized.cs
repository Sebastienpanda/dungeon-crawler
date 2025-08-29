using System.Collections.Generic;
using UnityEngine;

public class CameraObstruction : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    [SerializeField] private Material transparentMat;

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private List<Renderer> currentObstacles = new List<Renderer>();

    void Update()
    {
        HandleObstructions();
    }

    private void HandleObstructions()
    {
        // Restaurer tous les obstacles de la frame précédente
        foreach (Renderer rend in currentObstacles)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.material = originalMaterials[rend];
            }
        }
        currentObstacles.Clear();

        // Raycast entre la caméra et le joueur
        Vector3 dir = player.position - cam.transform.position;
        float dist = Vector3.Distance(cam.transform.position, player.position);

        RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, dir, dist);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Wall")) // Assure-toi que tes murs ont le tag "Wall"
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    // Sauvegarde le matériau original une seule fois
                    if (!originalMaterials.ContainsKey(rend))
                        originalMaterials[rend] = rend.material;

                    // Applique le transparent
                    rend.material = transparentMat;
                    currentObstacles.Add(rend);
                }
            }
        }
    }
}
