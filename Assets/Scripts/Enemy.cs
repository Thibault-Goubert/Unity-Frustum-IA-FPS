using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{
    public GameObject player;
    public Camera playerCamera;
    public GameObject playerBehind;
   
    private NavMeshAgent agent;
    private float speedRotation = 1.0f;
    private float movementSpeed = 100f;
    private float life = 20f;    
    private bool detected = false;

    //Shoot
    public GameObject prefabAmmo;
    public Transform shootSpawn;
    //public GameObject gohit = null;
    
    private float ammoLifetime_s = 3f;
    private float ammoDamage = 1f;
    private float ammoSpeed = 15f;
    private float fireRate = 1f;
    private float nextTimeToFire = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (detected)
        {
            escapeFromFrustum();
        }
        else
        {
            if (player != null)
            {
                RunBehind();
                RotateTo(player.transform);
                Attack();
            }
        }

    }

    private void RunBehind()
    {
        //Vector3 newPosition = transform.position;
        //float timeSinceLastFrame_s = Time.deltaTime;

        //Vector3 positionToReach = playerBehind.transform.position;

        ////Ne pas trop s'approcher
        //float distanceMini = 20f;
        //float distanceActual = (transform.position - player.transform.position).magnitude;
        //if(distanceActual <= distanceMini)
        //{
        //    Vector3 directionToRun1 = player.transform.position - transform.position;

        //    float distance_01 = Mathf.Cos(1 - (Mathf.Clamp(distanceActual, 0, distanceMini) / distanceMini));
        //    float force_quadratique = Mathf.Pow(distance_01, 2.0f);
        //    float speed_us = force_quadratique * movementSpeed;
        //    float distance_u = speed_us * timeSinceLastFrame_s;

        //    Vector3 lMouvement = directionToRun1 * distance_u;
        //    newPosition += lMouvement;
        //    agent.SetDestination(newPosition);
        //}
        //else
        //{
        //    //Viser le dos
        //    agent.SetDestination(positionToReach);
        //}

        Vector3 positionToReach = playerBehind.transform.position;
        agent.SetDestination(positionToReach);
    }

    public void TakeDamage(float damage)
    {
        life -= damage;
        if (life <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
    
    void escapeFromFrustum()
    {
        //on récupère l'angle de la camera pour savoir l'angle max à partir duquel l'ennemie peut être dans le frustum
        float maxView_Angle = playerCamera.fieldOfView/2;

        // timeSinceLastFrame_s contient le temps ecoule en secondes depuis la derniere frame
        float timeSinceLastFrame_s = Time.deltaTime;
               
        //On calcule le vecteur entre l'ennemie et le point le plus proche du forward du joueur (aligné avec le centre du frustum)
        Vector3 playerPos = player.transform.position;
        Vector3 playerForward = player.transform.forward;
        Vector3 enemyPosition = transform.position;

        Vector3 nearestPoint = NearestPointOnLine(playerPos, playerForward, enemyPosition);
        Vector3 directionToEscape = 
            new Vector3(enemyPosition.x,0,enemyPosition.z) 
          - new Vector3(nearestPoint.x,0,nearestPoint.z);

        // on veut une direction : un vecteur de taille 1
        Vector3 lDirection_OutOfFrustum = directionToEscape.normalized;

        // je choisi une vitesse en "unites par secondes"

        // je veux exprimer la distance entre la vue et l'ennemie entre 0 et 1.
        float actualAngle = Mathf.Abs(Vector3.Angle(playerForward, enemyPosition- playerPos));
        float distance_01 = Mathf.Cos((actualAngle / maxView_Angle));

        // je veux que plus on est proche, plus la vitesse est grande, de facon quadratique
        float force_quadratique = Mathf.Pow(distance_01, 2.0f);

        float speed_us = movementSpeed + (force_quadratique * movementSpeed);

        // la distance c'est :   vitesse (en unite par secondes) * temps (en secondes)
        float distance_u = speed_us * timeSinceLastFrame_s;

        Vector3 lMouvement = lDirection_OutOfFrustum * distance_u;
        Debug.DrawLine(transform.position, transform.position + (lDirection_OutOfFrustum * distance_u * 5), Color.green);

        agent.SetDestination(transform.position + lMouvement);        
    }
    Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize(); //this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    void Attack()
    {
        if (Time.time >= nextTimeToFire)
        {
            RaycastHit hit;
            Physics.Raycast(shootSpawn.transform.position, player.transform.position - shootSpawn.transform.position, out hit);
            Debug.DrawRay(shootSpawn.transform.position, hit.point - shootSpawn.transform.position, Color.red);

            if (hit.transform != null && hit.transform.root.gameObject == player)
            {
                Shoot();
                nextTimeToFire = Time.time + 1f / fireRate;                
            }
        }
    }

    private void RotateTo(Transform target)
    {
        Vector3 targetDir = target.position - transform.position;
        //Debug.DrawRay(transform.position, targetDir, Color.magenta);

        float maxRadiansDelta = speedRotation * Time.deltaTime;
        Vector3 current = transform.forward;
        Vector3 newDirection = Vector3.RotateTowards(current, targetDir, maxRadiansDelta, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.blue);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void Shoot()
    {
        //Debug.Log("Shoot");

        GameObject ammo = Instantiate(prefabAmmo);
        ammo.transform.position = shootSpawn.transform.position;
        ammo.transform.forward = player.transform.position - shootSpawn.transform.position;
        ammo.tag = "AmmoEnemy";
        //Debug.DrawRay(playerCamera.transform.position, hit.point - playerCamera.transform.position);

        Ammo ammoScript = ammo.GetComponent<Ammo>();
        ammoScript.Damage = ammoDamage;
        ammoScript.Speed = ammoSpeed;
        ammoScript.Lifetime_s = ammoLifetime_s;
        ammoScript.layerToHit = LayerMask.NameToLayer("Player");
    }

    public void Detected()
    {
        //Debug.Log("Detected");
        detected = true;
    }

    public void NotDetected()
    {
        //Debug.Log("NotDetected");
        detected = false;
    }
}
