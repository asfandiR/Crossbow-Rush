using UnityEngine;

public class ArrowProjectile : Projectile
{
    protected override void OnTargetSet()
    {
        if (target != null)
        {
            RotateTowards(new Vector2(target.position.x, target.position.z));
        }
    }

    protected override void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            target.position, 
            speed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.position, target.position) <= damageRadius)
        {
            HitTarget();
        }
    }

    private void RotateTowards(Vector2 targetPos)
    {
        Vector2 direction = targetPos - new Vector2(transform.position.x, transform.position.z);
        float roty = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Vector3 currentRotEu = transform.rotation.eulerAngles;
        Vector3 newRotEu = new Vector3(currentRotEu.x, roty, currentRotEu.z);
        transform.rotation = Quaternion.Euler(newRotEu);
    }
}