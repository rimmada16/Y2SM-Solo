using UnityEngine;

namespace Scriptable_Objects
{
    // This scriptable object is used to store the attributes of the enemy
    [CreateAssetMenu(fileName = "Enemy Attribute Data", menuName = "ScriptableObjects/EnemyAttributeScriptableObject", order = 1)]
    
    public class SpawnManagerScriptableObject : ScriptableObject
    {
        /// <summary>
        /// The attack speed of the enemy
        /// </summary>
        public float attackSpeed;
        
        /// <summary>
        /// The attack range of the enemy
        /// </summary>
        public float attackRange;
        
        /// <summary>
        /// The attack damage of the enemy
        /// </summary>
        public float attackDamage;
        
        /// <summary>
        /// The movement speed of the enemy
        /// </summary>
        public float movementSpeed;
    }
}