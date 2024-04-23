using Player;
using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This script is used to check the ground type the player is on
    /// Depending on the ground type the player will move at different speeds
    /// </summary>
    public class GroundTypeChecker : MonoBehaviour
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private float mudMoveSpeed;
        [SerializeField] private float iceMoveSpeed;
        
        private float _defaultMovementSpeed;

        /// <summary>
        /// Assigns the default movement variable to be the player's movement speed
        /// </summary>
        private void Start()
        {
            _defaultMovementSpeed = playerMovement.playerMovementSpeed;
        }

        /// <summary>
        /// Calls the GroundCheck method
        /// </summary>
        private void Update()
        {
            GroundCheck();
        }

        /// <summary>
        /// This method is used to check the ground type the player is on
        /// Depending on the ground type it will assign the player's movement speed to be the appropriate speed
        /// </summary>
        private void GroundCheck()
        {
            if (playerMovement == null)
            {
                return;
            }
        
            // Check if the player is on the ground
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f);
            if (hit.collider != null)
            {
                // Default
                if (hit.collider.gameObject.layer == 0)
                {
                    playerMovement.playerMovementSpeed = _defaultMovementSpeed;
                    //Debug.Log("On Ground");
                }
                // Ice
                else if (hit.collider.gameObject.layer == 6)
                {
                    playerMovement.playerMovementSpeed = iceMoveSpeed;
                    //Debug.Log("On Ice");
                }
                // Mud
                else if (hit.collider.gameObject.layer == 7)
                {
                    playerMovement.playerMovementSpeed = mudMoveSpeed;
                    //Debug.Log("On Mud");
                }
            }
        }
    }
}
