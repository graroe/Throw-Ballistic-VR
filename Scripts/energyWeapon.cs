using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energyWeapon : MonoBehaviour
{
    public Vector3? target;
    public float speed;
    public bool selector;
    // Start is called before the first frame update
    void Start()
    {
        if (!selector)
        {
            Vector3 aim = GameObject.FindObjectOfType<Player>().transform.position;
            target = (aim - transform.position) * 200;
        }
    }

    // Update is called once per frame

    void Update()
    {
        if (target.HasValue)
        {
            Vector3 dir = target.Value - transform.position;
            transform.position += dir.normalized * speed * Time.deltaTime;
        }
    }

    public void setTarget(Vector3 target)
    { 
        this.target = target; 
    }

    public void unsetTarget()
    {
        this.target = null;
    }

    public bool isSelector()
    {
        return this.selector;
    }
}
