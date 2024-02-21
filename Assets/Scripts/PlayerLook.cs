using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    
    [SerializeField] private float playerMouseSensitivity;
    [SerializeField] private float lookXLimit;
    
    private float _rotationXaxis;
    
    // using https://www.youtube.com/watch?v=qQLvcS9FxnY
    // using https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        _rotationXaxis += -Input.GetAxis("Mouse Y") * playerMouseSensitivity;
        _rotationXaxis = Mathf.Clamp(_rotationXaxis, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(_rotationXaxis, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * playerMouseSensitivity, 0);
    }
}
