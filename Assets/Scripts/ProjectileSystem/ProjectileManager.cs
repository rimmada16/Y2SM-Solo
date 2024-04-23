using UnityEngine;

namespace ProjectileSystem
{
    /// <summary>
    /// This class is responsible for managing the projectiles
    /// </summary>
    public class ProjectileManager : MonoBehaviour
    {
        // Singleton
        public static ProjectileManager Instance;
    
        [SerializeField] private GameObject[] projectiles;
    
        /// <summary>
        /// Ensure that there is only one instance of the ProjectileManager
        /// </summary>
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
        
        /// <summary>
        /// This method is responsible for firing the projectile
        /// </summary>
        /// <param name="projectile">int that represents the arrow to be shot</param>
        /// <param name="position">where to create the arrow at</param>
        /// <param name="direction">the direction fire in</param>
        /// <param name="projectileSource">GameObject</param>
        public void FireProjectile(int projectile, Vector3 position, Vector3 direction, GameObject projectileSource)
        {
            // Increment the Projectile by a singular unit
            position += Vector3.up;
        
            // Instantiate the Arrow
            GameObject newProjectile = Instantiate(projectiles[projectile], position, Quaternion.identity);
            newProjectile.GetComponent<Arrow>().direction = direction;
            newProjectile.layer = projectileSource.layer;
        }
    }
}