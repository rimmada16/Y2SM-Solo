using UnityEngine;

namespace Scriptable_Objects
{
    // This scriptable object is used to store the attributes of the enemy
    [CreateAssetMenu(fileName = "Enemy Attribute Data", menuName = "ScriptableObjects/EnemyAttributeScriptableObject", order = 1)]
    
    public class SpawnManagerScriptableObject : ScriptableObject
    {
        public float attackSpeed;
        public float attackRange;
        public float attackDamage;
        public float movementSpeed;
    }
}