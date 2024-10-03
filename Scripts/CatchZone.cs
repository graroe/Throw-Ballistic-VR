using ChaoticFormula;
using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;
using static SteamVR_Utils;
using static UnityEngine.UI.Image;

public class CatchZone : MonoBehaviour
{
    public Paddle paddle;
    public Paddle paddleOther;
    public bool BeamControl;
    public GameObject beamOrigin;
    public AudioSource beamSFX;
    
    bool beamActive;
    private void Start()
    {
        beamActive = false;
        if(BeamControl)
        {
            beamOrigin.SetActive(false);
        }
    }
    private void Update()
    {
    
        if (beamActive)
        {
            beam();
        }
        else if (BeamControl)
        {
            beamSFX.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("projectile"))
        {
            Projectile p = other.GetComponent<Projectile>();
            if (p.isCatchable())
            {
                paddle.handleCatch(other.attachedRigidbody);
            }
        }

        if (other.gameObject.GetComponent<CatchZone>() != null)
        {
            if (BeamControl && paddle.caught != null && paddleOther.caught != null)
            {
                GameObject ownProj = paddle.caught;
                GameObject otherProj = paddleOther.caught;
                if (ownProj.GetComponent<Projectile>().isEnergyWeapon() &&
                    otherProj.GetComponent<Projectile>().isEnergyWeapon())
                {
                    if (ownProj.GetComponent<energyWeapon>().isSelector())
                    {
                        GameObject.FindObjectOfType<GameState>().gameStart();
                    }
                    else
                    {
                        beamActive = true;
                        beamOrigin.SetActive(true);
                        StartCoroutine(beamTimer(4.0f));
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CatchZone>() != null)
        {
            beamActive = false;
            if (BeamControl)
            {
                beamOrigin.SetActive(false);
            }
        }
    }

    private void beam()
    {
        if (!beamSFX.isPlaying)
        {
            beamSFX.Play();
        }

        LightningBoltScript beamcontrol = beamOrigin.GetComponent<LightningBoltScript>();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * -1, out hit, 100))
        {

            Rigidbody rb = hit.rigidbody;

            if (rb != null)
            {
                if (rb.CompareTag("enemy"))
                {
                    rb.GetComponent<Enemy>().shutdown();
                }
                if (rb.CompareTag("paddle"))
                {
                    beamcontrol.EndPosition = transform.position + (transform.forward * -30);
                }

            }
            else
            {
                beamcontrol.EndPosition = hit.point;
            }

        }
        else
        {
            beamcontrol.EndPosition = transform.position + (transform.forward * -30);
        }

    }

    IEnumerator beamTimer(float duration)
    {
        for (float i = 0f; i < duration; i += Time.deltaTime)
        {
            yield return null;
        }
        beamActive = false;
        beamOrigin.SetActive(false);
        beamSFX.Stop();
        paddle.destroyCaught();
        paddleOther.destroyCaught();
    }
    
}
