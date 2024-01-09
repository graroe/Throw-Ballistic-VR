using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class projCollider : MonoBehaviour
{
    public AudioSource collisionSFX;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("projectile") && collision.gameObject)
        {
            collisionSFX.Play();
        }
    }
}
