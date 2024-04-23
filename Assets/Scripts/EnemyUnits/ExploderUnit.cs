using System.Collections;
using AStarPathfinding;
using SupportingSystems;
using UnityEngine;

namespace EnemyUnits
{
    /// <summary>
    /// This class is responsible for the Exploder Unit.
    /// It will subscribe to the event when the object is created and unsubscribe when the object is destroyed.
    /// It will also start the explosion Coroutine when the event is triggered
    /// </summary>
    public class ExploderUnit : MonoBehaviour
    {
        public static event ExploderBlewItselfUpEvent OnExploderBlewItselfUp;
    
        public int baseDamage; 
        public float explosionRadius;
        private bool _explosionEventNotInvoked = true;

        /// <summary>
        /// Subscribes to the event when the object is created
        /// </summary>
        private void Start()
        {
            Pathfinding.Explode += ExplodeEvent;
        }

        /// <summary>
        /// Unsubscribes from the event when the object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            Pathfinding.Explode -= ExplodeEvent;
        }

        /// <summary>
        /// This method is called when the event is triggered.
        /// It will check if the source of the event is the same as the gameObject and if it is,
        /// it will start the explosion Coroutine
        /// </summary>
        /// <param name="source">GameObject</param>
        private void ExplodeEvent(GameObject source)
        {
            if (source == gameObject)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
                
                // Can kill other enemies as well with tactical explosion trigger
                // Like Minecraft creepers
                foreach (Collider col in colliders)
                {
                    Health health = col.GetComponent<Health>();
                    StartCoroutine(AnimateExplosion(col.gameObject, health));
                }
            }
        }
    
        /// <summary>
        /// This Coroutine is called when the Exploder unit is exploding.
        /// It will animate the explosion and apply damage to the units in the explosion radius
        /// </summary>
        /// <param name="unit">GameObject</param>
        /// <param name="health">GameObject Health</param>
        /// <returns></returns>
        private IEnumerator AnimateExplosion(GameObject unit, Health health)
        {
            float duration = 1f;
            var localScale = gameObject.transform.localScale;
            Vector3 startScale = localScale; 
            Vector3 endScale = localScale * 1.5f;
            float currentTime = 0;
        
            // Increase the scale of the ExploderUnit for the set duration
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                // Scaling of the ExploderUnit
                gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
                yield return null;
            }

            // Apply damage after the explosion animation
            float falloffFactor = 1 - Mathf.Clamp01(Vector3.Distance(transform.position, unit.transform.position) / explosionRadius);
            int damage = Mathf.RoundToInt(baseDamage * falloffFactor);
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Destroy the unit after the explosion animation and increase the "kill count"
            if (_explosionEventNotInvoked)
            {
                OnExploderBlewItselfUp?.Invoke();
                _explosionEventNotInvoked = false;
            }
            Destroy(gameObject);
        }
    }
}