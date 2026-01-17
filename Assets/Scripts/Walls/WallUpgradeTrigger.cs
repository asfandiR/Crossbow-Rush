using UnityEngine;
using UnityEngine.Events;

public class WallUpgradeTrigger:MonoBehaviour
{
    [SerializeField] private float paymentStartDelay = 2f;
    public UnityEvent OnWallUpgradeTriggered;
    private float localTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            localTimer += Time.deltaTime;
            if (localTimer >= paymentStartDelay )
            {
                localTimer = 0f;
               OnWallUpgradeTriggered.Invoke();
            }
        }
    }
}