using UnityEngine;

namespace Player
{
    /// <summary>
    /// This script is responsible for the player's movement.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        public float playerMovementSpeed = 5f;
        [SerializeField] private float jumpSpeed = 5f;
        [SerializeField] private float gravity = 9.81f;
        
        [SerializeField] private Camera playerCamera;
        
        private CharacterController _charController;
        private Vector3 _movementDirection = Vector3.zero;

        /// <summary>
        /// Grabs the CharacterController component.
        /// </summary>
        private void Start()
        {
            _charController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// This method is responsible for the player's movement and jumping.
        /// Gravity is also applied here.
        /// </summary>
        private void Update()
        {
            if (_charController.isGrounded)
            {
                Movement();
                Jumping();
            }

            // Gravity shenanigans
            _movementDirection.y -= gravity * Time.deltaTime;

            // Movement shenanigans 
            _charController.Move(_movementDirection * Time.deltaTime);
        }

        /// <summary>
        /// This method is responsible for the player's movement.
        /// </summary>
        private void Movement()
        {
            // Get the movement input and multiply it by the player's movement speed.
            Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            // Calculate the movement direction based on the camera's forward and right vectors.
            _movementDirection = cameraForward * movementInput.z + playerCamera.transform.right * movementInput.x;
            _movementDirection *= playerMovementSpeed;
        }

        /// <summary>
        /// This method is responsible for the player's jumping.
        /// When the jump key is pressed, the player's y movement direction is set to the jump speed.
        /// </summary>
        private void Jumping()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _movementDirection.y = jumpSpeed;
            }
        }
    }
}
