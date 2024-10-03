using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    //SteamVR_Action_Vibration pulse;
    private Vector3 oldpos;
    private Vector3 velocity;
    private Queue<Vector3> sampledVelocities = new Queue<Vector3>();
    private int sampleCount = 10;   //number of samples taken
    public CatchZone catchZone;
    public GameObject caught;
    public AudioSource catchSFX;
    public AudioSource throwSFX;
    // Start is called before the first frame update
    void Start()
    {
        oldpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 newpos = gameObject.transform.position;
        velocity = newpos - oldpos;
        oldpos = newpos;
        sampledVelocities.Enqueue(velocity*Time.deltaTime * 100);
        if (sampledVelocities.Count > sampleCount)
        {
            sampledVelocities.Dequeue();
            if (caught != null)
            {
                caught.transform.position = catchZone.transform.position;
                tryThrow();
            }
        }

    }

    private Vector3 averageVectors(Queue<Vector3> samples)
    {
        float sumx = 0f;
        float sumy = 0f;
        float sumz = 0f;
        for (int i = 0; i < sampleCount; i++)
        {
            Vector3 v = samples.ElementAt(i);
            sumx+= v.x;
            sumy+= v.y;
            sumz+= v.z;
        }
        return new Vector3(sumx / sampleCount, sumy / sampleCount, sumz /sampleCount);
    }

    public void handleCatch(Rigidbody rb)
    {
        if (caught == null)
        {
            catchSFX.Play();
            rb.isKinematic = true;
            caught = rb.gameObject;
            Projectile p = caught.GetComponent<Projectile>();
            p.setCatchable(false);
            p.setDeflectable(false);
            p.setCaught(true);
            if (p.isEnergyWeapon()) {
                energyWeapon ew = p.GetComponent<energyWeapon>();
                ew.unsetTarget();
            }
            caught.transform.SetParent(gameObject.transform);
            caught.transform.position = catchZone.transform.position;
        }

    }

    private void tryThrow()
    {
        
        Vector3 avgVelo = averageVectors(sampledVelocities);

        if (avgVelo.magnitude * 10000 > 475 && avgVelo.z * transform.forward.z > 0) //don't throw through back of paddle
        {
            throwSFX.Play();
            Projectile p = caught.GetComponent<Projectile>();
            p.setCaught(false);
            caught.transform.SetParent(null);
            if (p.isEnergyWeapon())
            {
                energyWeapon ew = caught.GetComponent<energyWeapon>();
                ew.setTarget(avgVelo.normalized * 100);   //extend vector
            }
            else
            {
                Rigidbody rb = caught.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.velocity = avgVelo * 300;
            }
            caught = null;
        }
    }

    public void destroyCaught()
    {
        Destroy(caught);
        caught = null;
    }
}


