using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Attribute Data", menuName = "ScriptableObjects/EnemyAttributeScriptableObject", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    public float attackSpeed;
    public float attackRange;
    public float attackDamage;
    public float movementSpeed;
}