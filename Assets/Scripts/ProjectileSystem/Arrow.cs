using SupportingSystems;
using UnityEngine;

namespace ProjectileSystem
{
    /// <summary>
    /// This class is responsible for the arrow's behavior
    /// </summary>
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float speed = 10f;
        
        /// <summary>
        /// The damage of the Arrow
        /// </summary>
        public int damage = 50;
        
        /// <summary>
        /// The direction that the arrow will head in
        /// </summary>
        public Vector3 direction;
    
        /// <summary>
        /// Destroy the arrow after lifetime seconds
        /// </summary>
        private void Awake()
        {
            Destroy(gameObject, lifetime);
        }

        /// <summary>
        /// Move the arrow in the direction it was shot
        /// </summary>
        private void Update()
        {
            transform.Translate(direction * (speed * Time.deltaTime));
        }
    
        /// <summary>
        /// If the arrow hits a player, deal damage to the player
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // Validation
            if (other.gameObject.layer == gameObject.layer)
            {
                return;
            }

            if (other.gameObject.layer == 8)
            {
                Destroy(gameObject);
            }
        
            // Dealing damage to the player logic
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
}
