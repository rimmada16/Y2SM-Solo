using UnityEngine;

namespace Player
{
    /// <summary>
    /// This script is responsible for the player's look movement.
    /// </summary>
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
    
        [SerializeField] private float playerMouseSensitivity;
        [SerializeField] private float lookXLimit;
    
        private float _rotationXAxis;

        /// <summary>
        /// Locks the cursor and makes it invisible.
        /// </summary>
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    
        /// <summary>
        /// Calls the Look method.
        /// </summary>
        private void Update()
        {
            Look();
        }

        /// <summary>
        /// Rotates the player and the camera based on the mouse movement.
        /// </summary>
        private void Look()
        {
            // Get the mouse movement and multiply it by the player's mouse sensitivity.
            _rotationXAxis += -Input.GetAxis("Mouse Y") * playerMouseSensitivity;
            
            // Clamp the rotation to the look limit.
            _rotationXAxis = Mathf.Clamp(_rotationXAxis, -lookXLimit, lookXLimit);
            
            // Rotate the camera based on the rotationXAxis.
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationXAxis, 0, 0);
            
            // Rotate the player based on the mouse movement.
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * playerMouseSensitivity, 0);
        }
    }
}
