using System.Collections;
using UnityEngine;
using AStarPathfinding;

public class ExploderUnit : MonoBehaviour
{
    public int baseDamage; // Base damage of the explosion
    public float explosionRadius;

    private void Start()
    {
        Pathfinding.Explode += ExplodeEvent;
    }

    private void OnDestroy()
    {
        Pathfinding.Explode -= ExplodeEvent;
    }

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
    
    private IEnumerator AnimateExplosion(GameObject unit, Health health)
    {
        float duration = 1f;
        var localScale = gameObject.transform.localScale;
        Vector3 startScale = localScale; // Use unit's scale
        Vector3 endScale = localScale * 1.5f; // Scale the unit by 1.5
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration); // Scale the unit
            yield return null;
        }

        // Apply damage after the explosion animation
        float falloffFactor = 1 - Mathf.Clamp01(Vector3.Distance(transform.position, unit.transform.position) / explosionRadius);
        int damage = Mathf.RoundToInt(baseDamage * falloffFactor);
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Destroy the unit after the explosion animation
        Destroy(gameObject);
    }

}