using System;
using System.Collections;
using UnityEngine;
using Valve.VR;

public class Player : MonoBehaviour
{
    public bool vulnerable;
    public Color painColor;
    public int hitPoints;
    public AudioSource painSFX;
    void Start()
    {
        vulnerable = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 feetPos = transform.position;
        feetPos.y = 0.1f;
        Collider[] hits = Physics.OverlapCapsule(transform.position, feetPos, 0.01f);
        if (hits.Length > 0)
        {
            Debug.Log(hits.Length);
            Debug.Log(hits);
            Collider dmgCollider = Array.Find(hits, 
                coll => coll.gameObject.CompareTag("projectile") && !coll.gameObject.GetComponent<Projectile>().isCaught());
            if (dmgCollider!= null)
            {
                Destroy(dmgCollider.gameObject);
                handleHit();
            }  
        }
    }

    public void handleHit()
    {
        if (vulnerable)
        {
            painSFX.Play();
            hitPoints -= 1;
            if (hitPoints <= 0)
            {
                die();
            }
            else
            {
                vulnerable = false;
                SteamVR_Fade.Start(painColor, 0.2f);
                StartCoroutine(fade(0.3f));
                StartCoroutine(invincibleFrames(0.6f));
                Debug.Log("pain");
            }
        }
        else
        {
            Debug.Log("invincible frame");
        }
    }

    IEnumerator fade(float fadeTime)
    {
        for (float i = 0f; i < fadeTime; i += Time.deltaTime)
        {
            yield return null;
        }
        SteamVR_Fade.Start(Color.clear, fadeTime);
    }

    IEnumerator invincibleFrames(float duration)
    {
        for (float i = 0f; i < duration; i += Time.deltaTime)
        {
            yield return null;
        }
        vulnerable = true;
    }

    IEnumerator dieAfterWait(float duration)
    {
        for (float i = 0f; i < duration; i += Time.deltaTime)
        {
            yield return null;
        }
        GameObject.FindObjectOfType<GameState>().gameOver(duration);
    }

    void die()
    {
        SteamVR_Fade.Start(Color.black, 0.5f);
        StartCoroutine(dieAfterWait(0.5f));
        
    }


}
