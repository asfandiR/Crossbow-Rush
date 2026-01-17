using UnityEngine;

public class TowerStates : MonoBehaviour
{
    public enum BuildingState { NotBuilded, Builded, MaxLevel }
    public enum AttackState { Idle, Attacking, Charging }

    public BuildingState CurrentBuildingState = BuildingState.NotBuilded;
    public AttackState CurrentAttackState = AttackState.Idle;
}