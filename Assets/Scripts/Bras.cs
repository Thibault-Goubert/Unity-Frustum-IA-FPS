using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bras : MonoBehaviour
{
    /// le but que l'on souhaite suivre.
    public Transform mTargetToFollow;
    [SerializeField]
    private Transform projectileMenacant = null; 

    /// chaine cinematique, du root vers les fils.
    /// elle est initialisee lors du PLAY par la fonction Start().
    /// MISE PUBLIC juste pour du debug par le biais de l'inspecteur de Unity3D
    public List<Transform> mChain;

    /// Temps ecoule en secondes depuis le dernier calcul de minimisation.
    public float mTimeSinceLastStep_s;

    /// Temps a attendre en secondes entre chaque calcul de minimisation.
    public float mTimeBetweenAutomaticUpdates_s;

    [SerializeField]
    private float mRange;
    private GameObject[] ammos = null;

    /// Initialisation : 
    ///   recuperer tous les "non-Cylinder" fils qui forment la chaine.
    void Start()
    {
        // positionner la camera 3D
        // CameraHlp.setup3DCamera();
        // Camera.main.gameObject.transform.Translate(0, 0, -7.0f);

        mChain = new List<Transform>();

        // creer une pyramide
        float widthPyramide = 0.15f;
        float lengthPyramide = 1.0f;
        Vector3 p1 = new Vector3(0, -widthPyramide, -widthPyramide);
        Vector3 p2 = new Vector3(0, 0, widthPyramide);
        Vector3 p3 = new Vector3(0, widthPyramide, -widthPyramide);
        Vector3 p4 = new Vector3(lengthPyramide, 0, 0);
        Mesh lMesh = CreateMeshUnity.sCreateMeshFromTriangles("Tetrahedre", new List<Triangle3D>(new Triangle3D[]{
            new Triangle3D(p1,p4,p2),
            new Triangle3D(p3,p4,p1),
            new Triangle3D(p2,p4,p3),
            new Triangle3D(p2,p3,p1)
        }));

        mChain.Add(this.transform);
        int nbElements = 8;
        for (int i = 0; i < nbElements; ++i)
        {
            var lChild = new GameObject("Fils_" + i.ToString());
            var lChildTrans = lChild.transform;

            lChildTrans.parent = mChain[mChain.Count - 1];
            lChildTrans.localPosition = Vector3.right * lengthPyramide;
            lChild.AddComponent<MeshFilter>().sharedMesh = lMesh;
            var lRenderer = lChild.AddComponent<MeshRenderer>();
            lRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            lRenderer.sharedMaterial = Resources.Load<Material>("Bras");
            //Create collider
            var lCollider = lChild.AddComponent<MeshCollider>();
            lCollider.sharedMesh = lMesh;

            mChain.Add(lChildTrans);
        }
        mChain.RemoveAt(0);
        mChain[mChain.Count - 1].localScale = Vector3.one * 0.1f; // on se moque de l'orientation du dernier element

        Repos(); //Set initial position for the chain
        mRange = lengthPyramide * nbElements;
    }

    /// Update() est appelee a chaque frame.
    /// On l'utilise pour executer la boucle de minimisation une fois de temps en temps.
    void Update()
    {
        mTimeSinceLastStep_s += Time.deltaTime;
        if (mTimeSinceLastStep_s > mTimeBetweenAutomaticUpdates_s)
        {
            if(projectileMenacant == null || projectileMenacant!=null && (projectileMenacant.position - gameObject.transform.position).magnitude > mRange)
            {
                projectileMenacant = null;
                ammos = GameObject.FindGameObjectsWithTag("AmmoEnemy");
                foreach (GameObject ammo in ammos)
                {
                    //Calculer un point sur un plan entre le joueur et la trajectoire de la balle
                    //Utiliser tout le bras plutôt que la pointe
                    if ((ammo.transform.position - gameObject.transform.position).magnitude <= mRange)
                    {
                        projectileMenacant = ammo.transform;
                        break;
                    }
                }
            }

            // on effectue le calcul toutes les 0.2 secondes
            doOneCycle(); // juste une boucle, pas une infinite
            mTimeSinceLastStep_s = 0;
        }
    }

    /// fonction que l'on cherche a minimiser
    /// doit renvoyer une valeur qui est 0 quand on a parfaitement reussi, et sinon une valeur plus grande que 0.
    public float fonctionDeCout()
    {
        Vector3 end_rs = getLastBone().position;
        Vector3 target_rs = projectileMenacant != null ? projectileMenacant.position : mTargetToFollow.position;
        float longueur = (end_rs - target_rs).magnitude;
        return longueur;
    }

    /// renvoie l'extremite de la chaine
    public Transform getLastBone()
    {
        if (mChain == null || mChain.Count == 0)
        {
            return null;
        }
        return mChain[mChain.Count - 1];
    }

    /// Execution du cycle de minimisation juste 1 fois pour chaque joint de la chaine cinematique,
    /// en commencant par la derniere (extremite), et en remontant a la racine de la chaine.
    public void doOneCycle()
    {
        //TOREMOVEFOREXERCISES_BEGIN
        Vector3[] angleVariations = new Vector3[]{
            new Vector3(0.5f,0,0),  // +x
			new Vector3(-0.5f,0,0), // -x
			new Vector3(0,0.5f,0),  // +y 
			new Vector3(0,-0.5f,0), // -y
			new Vector3(0,0,0.5f),  // +z
			new Vector3(0,0,-0.5f)  // -z
		};

        // part de la fin, et tente de minimiser la fonction de cout
        for (int i = mChain.Count - 1; i >= 0; --i)
        {
            var lTransformCurrent = mChain[i];

            float currentBestValue = fonctionDeCout();
            Vector3 currentAngles = lTransformCurrent.localEulerAngles;

            for (int e = 0; e < angleVariations.Length; ++e)
            {
                Vector3 angleVariation = angleVariations[e];
                lTransformCurrent.Rotate(angleVariation);

                float newValue = fonctionDeCout();
                if (newValue < currentBestValue)
                {
                    // on garde cette modification, et on prend note de la valeur
                    currentBestValue = newValue;
                    currentAngles = lTransformCurrent.localEulerAngles;
                }
                else
                {
                    // on retourne sur l'angle sur lequel on etait
                    lTransformCurrent.localEulerAngles = currentAngles;
                }
            }
        }

        //TOREMOVEFOREXERCISES_END
    }

    public void Repos()
    {
        mTargetToFollow = GameObject.Find("ReposBras").transform;
    }
}
