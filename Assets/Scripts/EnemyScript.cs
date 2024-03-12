using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //[SerializeField] public SpawnManagerScriptableObject enemyAttributes;

    public float attackSpeed;
    public float attackRange;
    public float attackDamage;
    public float movementSpeed;

    public bool isShortsword, isLongsword, isGreatsword;
    
    // Start is called before the first frame update

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(attackSpeed);
            Debug.Log(attackRange);
            Debug.Log(attackDamage);
            Debug.Log(movementSpeed);
        }
    }
}
