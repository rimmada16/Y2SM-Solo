using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeChecker : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] private float mudMoveSpeed;
    [SerializeField] private float iceMoveSpeed;
    private float _defaultMovementSpeed;

    private void Start()
    {
        _defaultMovementSpeed = playerMovement.playerMovementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
       Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f);
        if (hit.collider != null)
        { 
            if (hit.collider.gameObject.layer == 0)
            { 
                playerMovement.playerMovementSpeed = _defaultMovementSpeed;
                Debug.Log("On Ground");
                
            }
            //Ice
            else if (hit.collider.gameObject.layer == 6)
            {
                playerMovement.playerMovementSpeed = iceMoveSpeed;
                Debug.Log("On Ice");
            }
            //Mud
            else if (hit.collider.gameObject.layer == 7)
            {
                playerMovement.playerMovementSpeed = mudMoveSpeed;
                Debug.Log("On Mud");
            }
        }
    }
}
