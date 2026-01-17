using System;
using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
   [SerializeField] private Animator animator;
   [SerializeField] private ParticleSystem UpgradeEffect;
private TowerAttackLogic towerAttackLogic;
private Tower tower;
    private readonly string AttackTrigger = "OnAttack";

    private void Awake()
    {
        if(animator == null)
        {
           Debug.LogError("Animator component is not assigned in TowerAnimator.");
        }
        towerAttackLogic = GetComponent<TowerAttackLogic>();
        tower = GetComponent<Tower>();
    }
    private void Start()
    {
        if (towerAttackLogic != null)
        {
            towerAttackLogic.OnAttack.AddListener(OnTowerAttack);
        }
        if(tower!=null)
        {
            tower.OnTowerUpgraded.AddListener(OnTowerUpgraded);
            tower.OnTowerBuilded.AddListener(OnTowerBuilt);
        }
    }

    private void OnTowerBuilt(Tower arg0)
    {
        if (UpgradeEffect != null)
        {
            UpgradeEffect.Play();
        }
    }

    private void OnTowerUpgraded(int arg0)
    {
        if (UpgradeEffect != null)
        {
            UpgradeEffect.Play();
        }
    }

    public void OnTowerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
        }
    }
}