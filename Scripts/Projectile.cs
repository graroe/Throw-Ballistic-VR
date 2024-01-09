using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public bool catchable;
    public bool deflectable;
    public bool caught;
    public bool energyWeapon;
    public AudioSource collisionSFX;

    // Start is called before the first frame update
    void Start()
    {
        caught = false;

    }

    public bool isCatchable()
    {
        return catchable;
    }

    public bool isCaught()
    {
        return caught;
    }

    public bool isDeflectable()
    {
        return deflectable;
    }

    public bool isEnergyWeapon()
    {
        return energyWeapon;
    }

    public void setCatchable(bool value)
    {
        this.catchable = value;
    }

    public void setCaught(bool value)
    {
        this.caught = value;
    }

    public void setDeflectable(bool value)
    {
        this.deflectable = value;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("projectile")){
            if (collisionSFX != null) {
                collisionSFX.Play();
            }
            
        }
    }

}
