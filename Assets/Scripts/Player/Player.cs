using UnityEngine;
[RequireComponent(typeof(UpgradableStats))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerAnimatorController))]
public class Player : MonoBehaviour
{
    private UpgradableStats playerStats;

    private PlayerAnimatorController playerAnimatorController;
    private PlayerMovementController playerMovementController;
    private PlayerAttackController playerAttackController;
    

    private void Awake()
    {
        playerStats = GetComponent<UpgradableStats>();
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerAttackController = GetComponent<PlayerAttackController>();
    }
}