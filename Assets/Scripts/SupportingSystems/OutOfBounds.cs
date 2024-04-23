using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// Handles the out of bounds trigger for the player if they jump off the map
    /// </summary>
    public class OutOfBounds : MonoBehaviour
    {
        /// <summary>
        /// Call the PlayerDeath method in the GameManager if the player goes out of bounds
        /// </summary>
        /// <param name="other">other is the GameObject that was collided with</param>
        private void OnTriggerEnter(Collider other)
        {
            GameManager.Instance.PlayerDeath();
        }
    }
}
