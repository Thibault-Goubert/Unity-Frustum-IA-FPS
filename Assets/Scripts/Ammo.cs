using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{ 
    public float Speed;
    public float Lifetime_s;
    public float Damage;
    public LayerMask layerToHit;

    // Update is called once per frame 
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        Destroy(this.gameObject, Lifetime_s);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision!");
        GameObject go = collision.gameObject;
        //Debug.Log(LayerMask.LayerToName(go.layer) + "hit!");

        if (go.layer == layerToHit)
        {
            if (go.layer == LayerMask.NameToLayer("Player"))
            {
                go.GetComponent<PlayerCharacter>().TakeDamage(Damage);
            }
            else if (go.layer == LayerMask.NameToLayer("Enemy"))
            {
                go.GetComponent<Enemy>().TakeDamage(Damage);
            }
        }
        Destroy(this.gameObject);
    }    
}
