using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] public float playerMovementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float gravity = 9.81f;
    private CharacterController _charController;
    private Vector3 _movementDirection = Vector3.zero;

    private void Start()
    {
        _charController = GetComponent<CharacterController>();
    }

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

    private void Movement()
    {
        Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        _movementDirection = cameraForward * movementInput.z + playerCamera.transform.right * movementInput.x;
        _movementDirection *= playerMovementSpeed;
    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _movementDirection.y = jumpSpeed;
        }
    }
}
