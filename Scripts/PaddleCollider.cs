using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleCollider: MonoBehaviour
{
    private Vector3 oldpos;
    private Vector3 velocity;
    public int deflectionModifier;
    public AudioSource whackSFX;
    public AudioSource throwSFX;
    // Start is called before the first frame update
    void Start()
    {
        oldpos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newpos = gameObject.transform.position;
        velocity = newpos - oldpos;
        oldpos = newpos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("projectile"))
        {
            Projectile p = other.gameObject.GetComponent<Projectile>();
            if (p.isDeflectable())
            {
                if(p.isEnergyWeapon())
                {
                    energyWeapon ew = p.GetComponent<energyWeapon>();
                    ew.target = velocity.normalized * 100;
                }
                other.attachedRigidbody.velocity = velocity * deflectionModifier;
            }
            if (!throwSFX.isPlaying && !whackSFX.isPlaying)
            {
                whackSFX.Play();
            }
        }
    }

}
