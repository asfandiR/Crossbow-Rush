using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    public static Transform mainBaseTransform;
    
    // Pre-allocate a buffer. Adjust size (20) based on max expected enemies in range.
    private static readonly Collider[] _colliderBuffer = new Collider[20]; 

    public static Enemy FindBestEnemyTarget(Vector3 position, float range)
    {
        if (mainBaseTransform == null) 
        mainBaseTransform = GameObject.FindGameObjectWithTag("Base").transform;

        // NonAlloc avoids garbage collection
        int count = Physics.OverlapSphereNonAlloc(position, range, _colliderBuffer);
        
        Enemy bestTarget = null;
        float closestToBaseDist = Mathf.Infinity;
        Vector3 basePos = mainBaseTransform.position;

        for (int i = 0; i < count; i++)
        {
            // Check for Enemy component without allocating
            if (_colliderBuffer[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                // Check if alive (assuming HealthSystem property)
                if (enemy.IsAlive) 
                {
                    // Logic: Pick enemy closest to the Main Base
                    float distToBase = Vector3.SqrMagnitude(enemy.transform.position - basePos);
                    
                    if (distToBase < closestToBaseDist)
                    {
                        closestToBaseDist = distToBase;
                        bestTarget = enemy;
                    }
                }
            }
        }
        
        // Clear references to allow GC if needed, though for Colliders it's usually fine to overwrite
        for(int i = 0; i < count; i++) _colliderBuffer[i] = null; 

        return bestTarget;
    }
     public static Wall FindBestWallTarget(Vector3 position, float range)
    {
        // Используем тот же буфер для оптимизации
        int count = Physics.OverlapSphereNonAlloc(position, range, _colliderBuffer);
        
        Wall bestTarget = null;
        float closestDist = Mathf.Infinity;

        for (int i = 0; i < count; i++)
        {
            if (_colliderBuffer[i].TryGetComponent(out Wall wall))
            {
                if (wall.IsAlive) 
                {
                    // Логика: Выбираем стену, ближайшую к самому Врагу (препятствие на пути)
                    float dist = Vector3.SqrMagnitude(wall.transform.position - position);
                    
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        bestTarget = wall;
                    }
                }
            }
        }
        
        // Очищаем ссылки
        for(int i = 0; i < count; i++) _colliderBuffer[i] = null; 

        return bestTarget;
    }

    public static Wall FindWallBlockingPath(Vector3 position, Vector3 direction, float range)
    {
        // Используем SphereCast (объемный луч), чтобы найти препятствие перед собой
        // Поднимаем точку начала (Vector3.up), чтобы луч шел от центра тела, а не от ног
        Vector3 origin = position + Vector3.up * 1.0f; 
        float radius = 0.5f; // Радиус проверки (чуть меньше ширины врага)

        if (Physics.SphereCast(origin, radius,origin, out RaycastHit hit, range))
        {
            if (hit.collider.TryGetComponent(out Wall wall) && wall.IsAlive)
            {
                return wall;
            }
        }
        return null;
    }
}