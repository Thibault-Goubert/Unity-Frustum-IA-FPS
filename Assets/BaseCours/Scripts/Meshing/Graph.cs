using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Un ensemble de points relies par des liens.
/// Par defaut les points sont crees principalement sur le plan X,Y (c'est a dire Z == 0).
public class Graph
{
	// ------------------------------------------------------------
	//---------------------------ATTRIBUTS-------------------------
	// ------------------------------------------------------------

	/// tableau de sommets.
	/// on peut ecrire ensuite: points[45].x  par exemple pour acceder au 46me element
	public List<GVector3> mPoints;

	/// indique les liens existants entre des points
	public List<Edge> mEdges;

	//--------------ATTRIBUTS POUR L'AFFICHAGE (pas important)

	/// je veux avoir toujours les memes valeurs de random
	private System.Random internal_random;

	/// pour la couleur des links.
	private Material linkMaterial;

	/// cette transforme sera cree lors de la creation du graph
	private Transform mTransform;

	//----------------------------------------------------
	// fonctions de creation------------------------------
	//----------------------------------------------------

	public Graph(string pName)
	{
		mPoints = new List<GVector3>();
		mEdges = new List<Edge>();
		internal_random = new System.Random(66);
		linkMaterial = new Material( Shader.Find("Legacy Shaders/Transparent/Diffuse"));
		linkMaterial.color = Color.green;
		linkMaterial.color = new Color(0,1,0,0.3f);
		//linkMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON" );
		//linkMaterial.shaderKeywords = new string[]{"_ALPHAPREMULTIPLY_ON"}; // semi transparent

		mTransform = new GameObject(pName).transform;
	}

	/// supprime les elements visuels (note je n'ai pas verifie que cela marche parfaitement)
	public void destroy()
	{
		mPoints.Clear();
		mEdges.Clear();
		Object.Destroy(mTransform.gameObject);
		Object.Destroy(linkMaterial);
	}


	/// ordered : true => teste si le link existe deja dans le sens a=>b
	/// ordered : false=> teste si le link existe deja dans le sens a=>b ou dans le sens b=>a
	/// return true s'il y a un ajout
	public bool addLinkIfNotThereBetween(int pA, int pB, bool ordered)
	{
		if (hasLinkBetween(pA, pB, ordered))
		{
			return false;
		}
		mEdges.Add(new Edge(pA, pB));
		return true;
	}

	/// cree une etoile/croix a N branches.
	public void initAsCrossGraph(Vector3 center, float pRadius, int pNbLeaf)
	{
		mEdges.Clear();
		mPoints.Clear();
		mPoints.Add(center);
		if (pNbLeaf <= 0)
		{
			return;
		}

		float toRad = 3.14f / 180.0f;
		float angle = toRad * 360.0f / (float) pNbLeaf;
		for (int i = 0; i < pNbLeaf; ++i)
		{
			float x = center.x + Mathf.Cos(angle * i) * pRadius;
			float y = center.y + Mathf.Sin(angle * i) * pRadius;
			mPoints.Add(new Vector3(x,y));
			mEdges.Add(new Edge( 0, i ));
		}
	}

	/// ajoute un certain nombre de point de facon aleatoire
	public void initAsRandomPoints(int nbPoints, int minX, int maxX, int minY, int maxY)
	{
		for (int i = 0; i < nbPoints; ++i)
		{
			//float vX = (float)(Random.value * maxX);
			//float vY = (float)(Random.value * maxY);
			float vX = (float) internal_random.Next(minX, maxX);
			float vY = (float) internal_random.Next(minY, maxY);
			mPoints.Add(new Vector3(vX, vY, 0.0f));
		}
	}

	/// ajoute un certain nombre de points sur un cercle de centre centerX, centerY
	/// createLine : false => on cree juste des points
	/// createLine : false => on cree les points + les liens
	public void initAsCirclePoints(int nbPoints, int centerX, int centerY, float radius, bool createLine)
	{
		if (nbPoints <= 0)
		{
			return;
		}

		float toRad = 3.14f / 180.0f;
		for (int i = 0; i < nbPoints; ++i)
		{
			float vX = Mathf.Cos(toRad * i * 360.0f / (float)nbPoints) * radius + centerX;
			float vY = Mathf.Sin(toRad * i * 360.0f / (float)nbPoints) * radius + centerY;
			mPoints.Add(new Vector3(vX, vY));
		}
		if (createLine)
		{
			for (int i = 0; i < nbPoints-1; ++i)
			{
				mEdges.Add(new Edge(i, i + 1));
			}
			mEdges.Add(new Edge(nbPoints-1,0));
		}
	}

	// pour chaque point, je le relie a ses 3 autres freres les plus proches (si ce n'est pas deja le cas)
	// (remarque : cela signifie qu'on aura parfois 4 ou plus liens sur 1 sommet)
	// ex:    A-B-C-D-----E
	//          les 3 plus proches de C sont B,D,A.
	//          les 3 plus proches de E sont D,C,B. => donc on aura 4 liens sur C en tout : A,B,D,E
	public void link3ClosestPoints()
	{
		for (int i = 0; i < mPoints.Count; ++i)
		{
			// calculer les 3 points les plus proches de ce point-ci
			float[] bestDistances= { 100000, 100000, 100000 };
			int[] bestIndex = { 0,1,2 };

			int numFoundDistances = 0;
			for (int j = 0; j < mPoints.Count; ++j)
			{
				if (j == i)
				{
					continue;
				}

				float tempDistance = (mPoints[j] - mPoints[i]).magnitude;
				if (tempDistance < bestDistances[0])
				{
					bestDistances[2] = bestDistances[1];
					bestDistances[1] = bestDistances[0];
					bestDistances[0] = tempDistance;

					bestIndex[2] = bestIndex[1];
					bestIndex[1] = bestIndex[0];
					bestIndex[0] = j;
					++numFoundDistances;
				}
				else if (tempDistance < bestDistances[1])
				{
					bestDistances[2] = bestDistances[1];
					bestDistances[1] = tempDistance;

					bestIndex[2] = bestIndex[1];
					bestIndex[1] = j;
					++numFoundDistances;
				}
				else if (tempDistance < bestDistances[2])
				{
					bestDistances[2] = tempDistance;
					bestIndex[2] = j;
					++numFoundDistances;
				}
			}
			if (numFoundDistances < 3)
			{
				//std::cout << "pb : on a trouve que " << numFoundDistances << " meilleures distances pour " << i << std::endl;
				continue;
			}
			// on ajoute nos 3 liens si possibles
			for (int k = 2; k >= 0; --k)
			{
				int j = bestIndex[k];
				if (addLinkIfNotThereBetween(i, j, false))
				{
					//std::cout << "ajout de lien " << i << " -> " << j << " distance("<< bestDistances[k]<<")"<< std::endl;
				}
			}
		}
	}

	/// enlever 1 lien sur n. si on a 30 link, et qu'on en enleve 1/3 alors il reste 10 link, relies 2 par 2
	public void removeOneLinkOutOf(int n)
	{
		if (n <= 0)
		{
			return;
		}
		for(int i = mEdges.Count - 1; i >=0; --i)
		{
			if (i % n == 0)
			{
				mEdges.RemoveAt(i);
			}
		}
	}

	/// chaque sommet du mesh devient un sommet du graph.
	/// ceci supprime cependant les doublons de sommets et d'edges.
	public bool initFromMesh( Mesh pMesh )
	{
		if( pMesh == null )
		{
			return false;
		}

		var lVertices = new List<Vector3>( pMesh.vertices );

		// supprimer les doublons de sommets.
		// on note les nouveaux indices dans une liste.
		// newVerticesIndex contient autant d'element que lVertices.Count
		List<int> newVerticesIndex = new List<int>();
		int newCurrentIndex = 0;
		for(int v = 0; v < lVertices.Count; ++v)
		{
			var lPoint3D = lVertices[v]; 
			int foundIndex = mPoints.FindIndex( (p3D)=>{ return p3D.isApproximately( lPoint3D ); } );
			if( foundIndex != -1 )
			{
				newVerticesIndex.Add( foundIndex );
			}else{
				newVerticesIndex.Add( newCurrentIndex );

				mPoints.Add( lPoint3D );
				++newCurrentIndex;
			}
		}

		// ajouter les liens entre ces elements
		for(int s = 0; s < pMesh.subMeshCount ; ++s)
		{
			var lSubMeshIndices = pMesh.GetIndices( s );
			for(int i = 0; i < lSubMeshIndices.Length; i += 3) // on suppose que ce sont des triangles
			{
				int originalV1 = lSubMeshIndices[i+0];
				int originalV2 = lSubMeshIndices[i+1];
				int originalV3 = lSubMeshIndices[i+2];

				int v1 = newVerticesIndex[ originalV1 ];
				int v2 = newVerticesIndex[ originalV2 ];
				int v3 = newVerticesIndex[ originalV3 ];
				addLinkIfNotThereBetween(v1, v2, false);
				addLinkIfNotThereBetween(v2, v3, false);
				addLinkIfNotThereBetween(v3, v1, false);
			}
		}

		return true;
	}

	// ------------------------------------------------------------
	// fonctions d'acces-------------------------------------------
	// ------------------------------------------------------------

	/// ordered : true => return true si il y a un link a=>b
	/// ordered : false=> return true si il y a un link a=>b OU un link b=>a
	public bool hasLinkBetween(int pA, int pB, bool ordered)
	{
		if (!ordered)
		{
			return hasLinkBetween(pA, pB, true) || hasLinkBetween(pB, pA, true);
		}
		foreach(var link in mEdges)
		{
			if( link.a == pA && link.b == pB )
			{
				return true;
			}
		}
		return false;
	}

	/// renvoie l'index du point du graph le plus proche de (x,y)
	/// renvoie -1 si graph vide
	public int getClosestPointFrom(Vector3 pPosition)
	{
		float bestDist = 1000000.0f;
		int found = -1;
		for (int i = 0; i < mPoints.Count; ++i)
		{
			float dist = (mPoints[i] - pPosition).magnitude;
			if (dist < bestDist)
			{
				bestDist = dist;
				found = i;
			}
		}
		return found;
	}

	/// renvoie la direction de ce lien en 3D
	public GVector3 getEdgeDirection3D(Edge pLink)
	{
		return (mPoints[pLink.b] - mPoints[pLink.a]).getNormalized();
	}

	// ------------------------------------------------------------
	// fonctions de modification-----------------------------------------
	// ------------------------------------------------------------

	/// deplacer tous les points de cette quantite
	public void translateAllPoints( float movementX, float movementY, float movementZ = 0.0f)
	{
		for(int i = 0; i < mPoints.Count; ++i)
		{
			mPoints[i] += new GVector3( movementX, movementY, movementZ);
		}
	}

	/// change la taille par rapport a 0.
	/// 1 : reste identique
	/// 2 : double la taille
	public void scale(float size01 )
	{
		for(int i = 0; i < mPoints.Count; ++i)
		{
			mPoints[i] *= size01;
		}
	}
		
	/// renvoie la transform correspondant a ce point
	public Transform getVisualPoint(int i)
	{
		if( mTransform.childCount == 0)
		{
			return null;
		}
		var lVisualPoints = mTransform.GetChild(0);
		if( lVisualPoints == null )
		{
			return null;
		}
		if( lVisualPoints.childCount == 0 )
		{
			return null;
		}
		return lVisualPoints.GetChild(i);
	}

	/// permet de changer la couleur d'un point visuel
	public void visual_changePointColor(int i, Color c)
	{
		var lVisualPoint = getVisualPoint(i);
		if( lVisualPoint == null )
		{
			return;
		}
		var lRenderer = lVisualPoint.GetComponent<MeshRenderer>();
		lRenderer.sharedMaterial.color = c;
	}

	/// permet de changer la couleur de tous les points visuels
	public void visual_changeAllPointsColor(Color c)
	{
		for(int i = 0; i < mPoints.Count; ++i)
		{
			var lVisualPoint = getVisualPoint(i);
			if( lVisualPoint == null )
			{
				return;
			}
			var lRenderer = lVisualPoint.GetComponent<MeshRenderer>();
			lRenderer.sharedMaterial.color = c;
		}
	}

	/// permet de changer la couleur d'un point visuel
	public void visual_changePointColor(Transform pVisualPoint, Color c)
	{
		var lRenderer = pVisualPoint.GetComponent<MeshRenderer>();
		lRenderer.sharedMaterial.color = c;
	}

	/// dessiner les links en transparence ou non
	public void visual_setEdgeTransparent( bool pTransparent)
	{
		if( pTransparent )
		{
			linkMaterial.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
		}else{
			linkMaterial.shader = Shader.Find("Legacy Shaders/Diffuse");
		}
	}

	/// enleve tous les texts (numeros dans le graph)
	public void visual_removeAllTexts()
	{
		forceSameSizeElementsUnderParent();
		var lTextMeshes = mTransform.GetComponentsInChildren<TextMesh>();
		for(int t = lTextMeshes.Length-1; t>=0; --t)
		{
			Object.Destroy(lTextMeshes[t]);
		}
	}

	/// changer l'echelle de chaque point, sans changer les dimensions du graph
	/// pQuantity01 : 1 : pas de changement
	/// pQuantity01 : 0.5 : divise par 2
	/// pQuantity01 : 3 : multiplie par 3.
	public void visual_scaleAllPoints( float pQuantity01 )
	{
		for(int i = 0; i < mPoints.Count; ++i)
		{
			var lTransform = getVisualPoint(i); 
			lTransform.localScale = new GVector3( pQuantity01, pQuantity01, pQuantity01 ) * lTransform.localScale; 
		}
	}


	// ------------------------------------------------------------
	// fonctions de dessin-----------------------------------------
	// ------------------------------------------------------------

	/// ajoute des gameobjects dans pParent la premiere fois.
	/// puis les reutilise lors de chaque dessin
	public void updateScene()
	{
		forceSameSizeElementsUnderParent();

		var lVisualPoints = mTransform.GetChild(0);
		for(int i = 0; i < mPoints.Count; ++i)
		{
			var lVisual = lVisualPoints.GetChild(i); 
			lVisual.position = mPoints[i];
		}

		var lVisualEdges = mTransform.GetChild(1);
		for(int i = 0; i < mEdges.Count; ++i)
		{
			var lEdge = mEdges[i]; 
			var lP1 = mPoints[lEdge.a];
			var lP2 = mPoints[lEdge.b];

			var lVisual = lVisualEdges.GetChild(i); 
			// on le place au milieu
			lVisual.position = (lP1 + lP2) * 0.5f;

			// on le tourne
			lVisual.LookAt( lP1 );

			// on calcule la taille du lien
			var lSize = (lP1 - lP2).magnitude;
			var lPreviousSize = lVisual.localScale;
			lVisual.localScale = new Vector3( lPreviousSize.x, lPreviousSize.y, lSize);
		}
	}

	/// ajoute des points et des links si besoin, et desactive le surplus
	private void forceSameSizeElementsUnderParent()
	{
		if( mTransform.childCount == 0 )
		{
			// creer les parents pour les points et les links
			GameObject lPointsGO = new GameObject("points");
			lPointsGO.transform.parent = mTransform;
			GameObject lEdgesGO = new GameObject("links");
			lEdgesGO.transform.parent = mTransform;
		}

		// maj des points (creations/activations/desactivations)
		{
			var lPoints = mTransform.GetChild(0);
			for(int i = 0; i < mPoints.Count; ++i)
			{
				if( i < lPoints.childCount )
				{
					lPoints.GetChild(i).gameObject.SetActive( true );
				}else{
					var lCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					lCube.transform.parent = lPoints;
					lCube.name = "p"+i.ToString();
					var lCubeRenderer = lCube.GetComponent<Renderer>();
					lCubeRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
					lCubeRenderer.sharedMaterial.color = Color.white;

					var lTxt = new GameObject("t"+i.ToString());
					lTxt.transform.parent = lCube.transform;
					lTxt.transform.localScale = Vector3.one * 0.3f;
					lTxt.AddComponent<MeshRenderer>();
					var lTxtMesh = lTxt.AddComponent<TextMesh>();
					lTxtMesh.alignment = TextAlignment.Center; 
					lTxtMesh.anchor = TextAnchor.MiddleCenter;
					lTxtMesh.fontSize = 60;
					lTxtMesh.text = i.ToString();
					lTxtMesh.color = Color.black;
				}
			}
			for(int i = mPoints.Count; i < lPoints.childCount; ++i)
			{
				lPoints.GetChild(i).gameObject.SetActive( false );
			}
		}

		// maj des links (creations/activations/desactivations)
		{
			var lEdges = mTransform.GetChild(1);
			for(int i = 0; i < mEdges.Count; ++i)
			{
				if( i < lEdges.childCount )
				{
					lEdges.GetChild(i).gameObject.SetActive( true );
				}else{
					var lCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					lCube.transform.parent = lEdges;
					lCube.transform.localScale = new Vector3(0.2f,0.2f,1.0f);
					lCube.name = "l"+i.ToString();
					var lRenderer = lCube.GetComponent<MeshRenderer>();
					lRenderer.sharedMaterial = this.linkMaterial;
				}
			}
			for(int i = mEdges.Count; i < lEdges.childCount; ++i)
			{
				lEdges.GetChild(i).gameObject.SetActive( false );
			}
		}
	}


	/// change les positions dans le graph grace a celles de la scene (mouvement)
	public void updateFromScene()
	{
		forceSameSizeElementsUnderParent();

		var lVisualPoints = mTransform.GetChild(0);
		for(int i = 0; i < mPoints.Count; ++i)
		{
			var lVisual = lVisualPoints.GetChild(i);
			mPoints[i] = lVisual.position;
		}
	}


}
