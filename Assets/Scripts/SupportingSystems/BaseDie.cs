using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This class is responsible for the death of an object.
    /// </summary>
    public abstract class BaseDie : MonoBehaviour, IDie
    {
        /// <summary>
        /// Handles the death of the object.
        /// </summary>
        public abstract void Die();

        /// <summary>
        /// Handles the deletion of the object.
        /// </summary>
        protected void DeleteSelf()
        {
            Destroy(gameObject);
        }
    }
}