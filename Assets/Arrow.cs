using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float speed = 10f;
    public int damage = 50;
    public Vector3 direction;
    
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }

        if (other.gameObject.layer == 8)
        {
            Destroy(gameObject);
        }
        
        if (other.gameObject.layer == 3 && gameObject.transform.parent.gameObject.layer == 9 || other.gameObject.layer == 9 && gameObject.layer == 3)
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            } 
            Destroy(gameObject);
        }
    }
}
