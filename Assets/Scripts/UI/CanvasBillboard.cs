using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private bool verticalOnly = false;
    
    private Transform mainCameraTransform;

    void Start()
    {
        // Кэшируем трансформ основной камеры для производительности
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    // Используем LateUpdate, чтобы поворот происходил ПОСЛЕ того, 
    // как камера переместилась в обычном Update
    void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        // Определяем позицию, на которую нужно смотреть
        Vector3 targetPosition = mainCameraTransform.position;

        if (verticalOnly)
        {
            // Игнорируем разницу в высоте, чтобы канвас вращался только по оси Y
            targetPosition.y = transform.position.y;
        }

        // Поворачиваем объект лицом к камере
        // Используем (transform.position - targetPosition), чтобы текст не был зеркальным
        transform.LookAt(transform.position + (transform.position - targetPosition));
    }
}