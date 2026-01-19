using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BombExplosion : MonoBehaviour
{
    [SerializeField]private float explosionRadius;
    [SerializeField]private float explosionDamage;
[SerializeField]private ParticleSystem explosionParticles;

private void Awake()
    {
        explosionParticles.gameObject.SetActive(false);
    }
    public void Explode()
    {
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.Play();
        Collider[] colliders= Physics.OverlapSphere(transform.position, explosionRadius );
        foreach (var item in colliders)
        {
            if(item.TryGetComponent(out Wall wall))
            {
                wall.TakeDamage(explosionDamage);
            }
        }    
        
    }
}