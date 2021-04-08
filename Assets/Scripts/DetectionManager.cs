using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    //PUBLIC
    public List<GameObject> targets;
    public GameObject player;
    public LayerMask layerToView;

    //PRIVATE
    private Camera playerCamera;
    private Vector3 playerCameraPosition;

    // Start is called before the first frame update 
    void Start()
    {
        playerCamera = player.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        playerCameraPosition = playerCamera.transform.position;
        //Calcule le frustum de la caméra
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        foreach (GameObject target in targets)
        {
            if (target == null) // le test est oppose cette fois
            {
                continue; // on saute cette iteration 
            }

            Enemy enemy = target.GetComponent<Enemy>();
            //Pour chaque ennemi, s'il est dans le champs de la caméra
            if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Renderer>().bounds))
            {
                //On vérifie qu'il n'y ait pas d'obstacle entre la caméra et l'ennemi
                Vector3 rayDir = target.transform.position - playerCameraPosition;

                RaycastHit hit;
                Physics.Raycast(playerCameraPosition, rayDir, out hit);

                if (hit.transform.root.gameObject == target)
                {
                    //Debug.Log(target.name + " Visible");
                    enemy.Detected();
                }
                else
                {
                    //Debug.Log(target.name + " Not Visible");
                    enemy.NotDetected();
                }
                Debug.DrawRay(playerCameraPosition, hit.point - playerCameraPosition);
            }
            else
            {
                //Debug.Log(target.name + " Not In FOV");
                enemy.NotDetected();
            }
        }
    }
}
