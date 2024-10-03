using ChaoticFormula;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Enemy : MonoBehaviour
{
    public GameObject[] projectileTypes;
    public Transform playerPos;
    public Transform launchZone;
    public float attackRate;
    public float movementSpeed;
    public AudioSource rotorSFX;
    public AudioSource basicShotSFX;
    public AudioSource energyShotSFX;
    public AudioSource fireballShotSFX;
    public AudioSource destroyedSFX;
    public AudioSource meleeSFX;
    DroneLogic droneLogic;
    bool moving;
    bool rising;
    bool canAttack;
    bool meleeMode;
    float attackTimer;
    float riseHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        droneLogic= GetComponent<DroneLogic>();
        playerPos = GameObject.FindObjectOfType<Player>().transform;
        lookAtPlayerXZ();
        riseHeight = UnityEngine.Random.Range(1.5f, 9.0f);
        rising= true;
        moving = false;
        canAttack = false;
        meleeMode = false;
        attackTimer = UnityEngine.Random.Range(0, attackRate);  //attack cycles are not synched
    }

    // Update is called once per frame
    void Update()
    {

        lookAtPlayerXZ();
        if (rising)
        {
            
            Vector3 pos = transform.position;
            pos.y += movementSpeed * Time.deltaTime * 5.0f;
            transform.position = pos;
            
            if (pos.y >= riseHeight) {
                rising = false;
                moving = true;
                canAttack = true;
            }


        }

        if (moving)
        {
            Vector3 dir = playerPos.position - transform.position;
            if (dir.magnitude < 2.0)
            {
                moving = false;
                meleeMode= true;
            }
            transform.position += dir.normalized * movementSpeed *Time.deltaTime;
            checkShootPath();
        }

        if (canAttack) { 
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                if (meleeMode)
                {
                    meleeAttack();
                }
                else
                {
                    projectileAttack();
                }
                attackTimer = 0;
            }
        }

        

    }

    void checkShootPath()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb != null && rb.CompareTag("enemy"))
            {
                transform.position += Vector3.Cross(transform.forward, transform.up).normalized * Time.deltaTime * movementSpeed;
                canAttack = false;
                Debug.Log("in the way");
            }
            else
            {
                canAttack= true;
            }
        }
    }

    void projectileAttack()
    {
        GameObject proj = Instantiate(rollForProjectile(), launchZone.position, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (!proj.GetComponent<Projectile>().isEnergyWeapon())
        {
            rb.velocity = calculateAttackVelocity(1.0f);
        }
    }

    Vector3 calculateAttackVelocity(float time)
    {
        Vector3 startPos = launchZone.transform.position;
        Vector3 endPos = playerPos.position;
        endPos = new Vector3(endPos.x * UnityEngine.Random.Range(0.9f, 1.1f), 
                            endPos.y * UnityEngine.Random.Range(0.75f, 1.2f), 
                            endPos.z * UnityEngine.Random.Range(0.9f, 1.1f));


        Vector3 d = endPos - startPos;
        time *= d.magnitude / 20.0f;
        
        float dY = d.y;
        d.y = 0.0f;
        float dXZ = d.magnitude;

        float velocityXZ = dXZ / time;
        float velocityY = dY / time + Physics.gravity.y * -0.5f * time;

        Vector3 result = d.normalized;
        result *= velocityXZ;
        result.y = velocityY;

        return result;
    }


    void meleeAttack()
    {
        droneLogic.FireAttack();
        meleeSFX.Play();
        playerPos.GetComponent<Player>().handleHit();
        Vector3 dir = playerPos.position - transform.position;
        if (dir.magnitude > 2.0)
        {
            meleeMode = false;
            moving = true;
        }
    }

    void lookAtPlayerXZ()
    {
        Vector3 playerXZ = playerPos.position;
        playerXZ.y = transform.position.y;
        transform.LookAt(playerXZ);
    }

    GameObject rollForProjectile()
    {
        int roll = UnityEngine.Random.Range(1, 100);
        if (roll <= 60)
        {
            basicShotSFX.Play();
            return projectileTypes[0];
        }
        if (roll <= 85)
        {
            energyShotSFX.Play();
            return projectileTypes[1];
        }
        fireballShotSFX.Play();
        return projectileTypes[2];
    }

    public void shutdown()
    {
        canAttack = false;
        moving = false;
        rising= false;
        rotorSFX.Stop();
        destroyedSFX.Play();
        droneLogic.PlayDroneDeath();
    }

}
