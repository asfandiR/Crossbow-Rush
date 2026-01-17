// PlayerMovementController.cs
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
   private readonly string IsMovingAnimBoolTag="IsMoving";
   private readonly string AttackTriggerTag="Attack";

    [SerializeField] private Animator playerAnimator; 
    [SerializeField] private Animator playerCrossbowAnimator; 
  private PlayerMovementController playerMovementController;
     private PlayerAttackController playerAttackController;
    
   

    private void Awake()
    {
       playerMovementController=GetComponent<PlayerMovementController>();
       playerAttackController=GetComponent<PlayerAttackController>();
         if (playerMovementController == null || playerAttackController == null||playerAnimator==null||playerCrossbowAnimator==null)
        {
            Debug.LogError("Ты пидорасс");
            return;
        }
    }
    private void Start()
    {
        playerAttackController.OnAttack.AddListener(AnimateOnAttack);
    }
    private void Update()
    {
       
        if (playerMovementController.HasActiveInput())
        {
            playerAnimator.SetBool(IsMovingAnimBoolTag,true);
        }
        else{
        playerAnimator.SetBool(IsMovingAnimBoolTag,false);
        }
    }
    private void AnimateOnAttack()
    {
        playerAnimator.SetTrigger(AttackTriggerTag);
        playerCrossbowAnimator.SetTrigger(AttackTriggerTag);
    }
}