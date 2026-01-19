using UnityEngine;

public class CannonballProjectile : Projectile
{
    [Header("Cannonball Settings")]
    [SerializeField] private float arcHeight = 2.0f;
    [SerializeField] private float explosionRadius = 3.0f;
    [SerializeField] private ParticleSystem explosionEffect;

    private Vector3 startPosition;
    private float progress = 0f;

    protected override void OnTargetSet()
    {
        // Cannonballs are usually slower
        if (speed > 15f) speed = 15f; 
        startPosition = transform.position;
    }

    protected override void Move()
    {
        // Calculate distance to cover
        float totalDistance = Vector3.Distance(startPosition, target.position);
        
        // Increment progress based on speed and estimated distance
        // We use totalDistance to normalize speed, ensuring constant horizontal velocity
        if (totalDistance > 0)
            progress += (speed * Time.deltaTime) / totalDistance;
        else
            progress = 1f;

        // Quadratic Bezier Curve: P0 (start), P1 (control), P2 (end)
        Vector3 p0 = startPosition;
        Vector3 p2 = target.position;
        // Control point is halfway between start and end, but elevated
        Vector3 p1 = (p0 + p2) / 2f + Vector3.up * arcHeight;

        // Bezier formula: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float t = Mathf.Clamp01(progress);
        Vector3 nextPos = Mathf.Pow(1 - t, 2) * p0 + 
                          2 * (1 - t) * t * p1 + 
                          Mathf.Pow(t, 2) * p2;

        transform.position = nextPos;
        transform.LookAt(nextPos); // Rotate to face movement direction

        if (t >= 1f || Vector3.Distance(transform.position, target.position) <= damageRadius)
        {
            HitTarget();
        }
    }

    protected override void HitTarget()
    {
        // Area Damage
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }

        if (explosionEffect != null)
        {
           explosionEffect.gameObject.SetActive(true);
explosionEffect.Play();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}