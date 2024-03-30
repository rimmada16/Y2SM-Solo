using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;
    
    [SerializeField] private GameObject[] projectiles;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add bow type to method requirements
    // Can remove speed at that point
    
    
    
    public void FireProjectile(int projectile, Vector3 position, Vector3 direction, GameObject projectileSource)
    {
        // Increment the Projectile by a singular unit
        position += Vector3.up;
        
        GameObject newProjectile = Instantiate(projectiles[projectile], position, Quaternion.identity);
        newProjectile.GetComponent<Arrow>().direction = direction;
        newProjectile.layer = projectileSource.layer;
        //newProjectile.GetComponent<Arrow>().damage = projectileDamage;
    }

}
