using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour 
{
    //Weapon
    public float fireRate = 15f;
    public GameObject shootSpawn;

    //Misc
    public Camera fpsCam;
    public ParticleSystem flash;
    [SerializeField]
    private float nextTimeToFire = 0f;

    //Ammo
    public GameObject prefabAmmo;

    [SerializeField]
    private float ammoLifetime_s = 3f;
    private float ammoDamage = 10f;
    private float ammoSpeed = 30f;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        flash.Play();

        //RaycastHit hit;
        //if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        //{
        //   Enemies enemy = hit.transform.GetComponent<Enemies>();
        //   if (enemy != null)
        //   {
        //       enemy.TakeDamage(damage);
        //   }
        //}

        GameObject ammo = Instantiate(prefabAmmo);
        ammo.transform.position = shootSpawn.transform.position;
        ammo.transform.rotation = shootSpawn.transform.rotation;

        Ammo ammoScript = ammo.GetComponent<Ammo>();
        ammoScript.Damage = ammoDamage;
        ammoScript.Speed = ammoSpeed;
        ammoScript.Lifetime_s = ammoLifetime_s;
        ammoScript.layerToHit = LayerMask.NameToLayer("Enemy");
    }
}
