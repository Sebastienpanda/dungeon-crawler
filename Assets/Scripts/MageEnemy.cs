using UnityEngine;

public class MageEnemy : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject fireVfx;
    [SerializeField] private Transform gun;

    public void Fire()
    {
        // Création du projectile
        GameObject go = Instantiate(bullet, gun.position, gun.rotation);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(gun.forward * 2000);
        }

        if (fireVfx != null)
        {
            GameObject fx = Instantiate(fireVfx, go.transform.position, go.transform.rotation, go.transform);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        Destroy(go, 5);

        Debug.Log("Mage a tiré !");
    }
}
