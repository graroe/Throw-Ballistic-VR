using ChaoticFormula;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droneCollider : MonoBehaviour
{
    public DroneLogic droneLogic;
    public Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy= droneLogic.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        /* droneLogic.SetPoweredOn(false);
         droneLogic.droneMeshRenderer.gameObject.SetActive(false);
         droneLogic.brokenDrone.gameObject.SetActive(true);*/
        if (other.CompareTag("projectile") || other.CompareTag("paddle"))
        {
            enemy.shutdown();
            droneLogic.PlayDroneDeath();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        
    }

}
