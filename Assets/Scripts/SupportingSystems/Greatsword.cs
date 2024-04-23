using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This script is used to handle the Greatsword
    /// </summary>
    public class Greatsword : MonoBehaviour
    {
        [SerializeField] private float maxAttackCooldown = 2f;
    
        private Animation _anim;
        private float _currentAttackCooldown;
    
        /// <summary>
        /// Grabs the animation component
        /// </summary>
        private void Start()
        {
            _anim = GetComponent<Animation>();
        }

        /// <summary>
        /// This method is used to check if the player has pressed the attack button and they are not on cooldown
        /// If they have then the attack animation will play whilst resetting the cooldown
        /// </summary>
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
}
