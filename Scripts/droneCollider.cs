using ChaoticFormula;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droneCollider : MonoBehaviour
{
    public Enemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("projectile") || other.CompareTag("paddle"))
        {
            enemy.shutdown();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        
    }

}
