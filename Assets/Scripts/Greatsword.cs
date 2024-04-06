using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword : MonoBehaviour
{
    private Animation _anim;
    
    [SerializeField] private float maxAttackCooldown = 2f;
    private float _currentAttackCooldown;
    
    private void Start()
    {
        _anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _currentAttackCooldown <= 0)
        {
            _anim.Play();
            _currentAttackCooldown = maxAttackCooldown;
        }
        else
        {
            _currentAttackCooldown -= Time.deltaTime;
        }
    }
}
