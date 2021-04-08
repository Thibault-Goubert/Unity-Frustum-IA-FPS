using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// un maillage plus simple que la classe Mesh d'unity, avec quelques fonctions en plus
/// on peut obtenir la liste des edges avec getEdges()
public class Mesh3D
{
	public List<Vector3> mPoints;
	public List<Triplet> mFaces;

	/// pour le debug visuel
	public Transform mTransform;
	public string mName;
	public static Material sLinkMaterial;


	//-------------------------------------------------------
	//----------------creation et init-----------------------
	//-------------------------------------------------------

	public Mesh3D()
	{
		mPoints = new List<Vector3>();
		mFaces = new List<Triplet>();
		mName ="defautMeshName";
		if( sLinkMaterial == null )
		{
			sLinkMaterial = new Material( Shader.Find("Standard"));
			sLinkMaterial.color = new Color( 0.1f, 0.76f, 0.3f, 0.6f);
		}
		//Debug.Log("creation d'un maillage");
	}

	public Mesh3D clone()
	{
		var lNew = new Mesh3D();
		foreach( var lPoint in mPoints )
		{
			lNew.mPoints.Add( lPoint );
		}
		foreach( var lTriplet in mFaces )
		{
			lNew.mFaces.Add( lTriplet.clone() );
		}
		return lNew;
	}

	/// recupere les informations qui proviennent d'un maillage de Unity3D
	public void initFrom( Mesh pMesh )
	{
		var lVerts = pMesh.vertices;
		for(int i = 0; i < lVerts.Length; ++i)
		{
			mPoints.Add( lVerts[i] );
		}

		for(int s = 0; s < pMesh.subMeshCount; ++s)
		{
			var lIndices = pMesh.GetIndices( s );
			for(int i = 0; i < lIndices.Length; i += 3)
			{
				int iA = lIndices[i];
				int iB = lIndices[i+1];
				int iC = lIndices[i+2];
				mFaces.Add( new Triplet( iA, iB, iC ));
			}
		}
	}

	//-------------------------------------------------------
	//----------------operations variees---------------------
	//-------------------------------------------------------

	/// changer la taille
	public void scale( float pSize01 )
	{
		for(int i = 0; i < mPoints.Count; ++i)
		{
			mPoints[i] = mPoints[i] * pSize01;
		}
	}

	// faire tourner notre objet
	public void rotate( float angleX_deg, float angleY_deg, float angleZ_deg )
	{
		var lMatrix = Matrix4x4.TRS( Vector3.zero, Quaternion.Euler( angleX_deg, angleY_deg, angleZ_deg), Vector3.one);
		for(int i = 0; i < mPoints.Count; ++i)
		{
			mPoints[i] = lMatrix.MultiplyPoint( mPoints[i] );
		}
	}

	/// calcule l'emplacement du centre de la face
	public Vector3 calcCenterFace(int pFaceIndex)
	{
		Triplet lFace = mFaces[pFaceIndex];
		return ( mPoints[lFace.a] + mPoints[lFace.b] + mPoints[lFace.c] ) / 3.0f;
	}

	/// renvoie l'aire du triangle concerne
	public float calcAreaFace( int pFaceIndex )
	{
		Triplet lFace = mFaces[pFaceIndex];
		var A = mPoints[lFace.a];
		var B = mPoints[lFace.b];
		var C = mPoints[lFace.c];

		// http://geomalgorithms.com/a01-_area.html
		return Vector3.Cross((B-A),(C-A)).magnitude *0.5f;
	}

	/// supprime le point et les faces qui lui sont reliees
	/// change les numeros impactes dans toutes les faces
	public bool removePoint( int pPointIndex )
	{
		if( pPointIndex < 0 || pPointIndex > mPoints.Count )
		{
			return false;
		}

		mPoints.RemoveAt( pPointIndex );

		// on a juste a diminuer de 1 les indices au dela de pPointIndex.
		for(int t = mFaces.Count -1; t >=0; --t)
		{
			if( mFaces[t].has( pPointIndex ) )
			{
				mFaces.RemoveAt( t );
				continue;
			}

			var lFace = mFaces[t];
			if( lFace.a > pPointIndex )
			{
				--lFace.a;
			}
			if( lFace.b > pPointIndex )
			{
				--lFace.b;
			}
			if( lFace.c > pPointIndex )
			{
				--lFace.c;
			}
		}
		return true;
	}

	/// quand un point 3D est le meme position qu'un autre, on le merge
	public void mergePointsAtSamePositions()
	{
		int nbMerged = 0;

		// pour chaque face, trouver si un point3D identique est present plus tot dans la liste, puis pointer dessus
		for( int f = 0; f < mFaces.Count; ++f)
		{
			var lFace = mFaces[f];
			for(int i = 0; i < 3; ++i)
			{
				int lIndex = lFace.getByIndex02(i);
				Vector3 lPoint3D = mPoints[lIndex];
				int found = mPoints.FindIndex( (pPoint)=>{ return MathsHlp.approximately(lPoint3D, pPoint); } );
				if( found != lIndex && found != -1)
				{
					lFace.setByIndex02( i, found );
					++nbMerged;
				}
			}
		}
		//Debug.Log("on vient de merge "+ nbMerged.ToString() +" sommets");

		// supprimer les points qui ne sont plus utilises du tout
		int nbDeleted = 0;
		for( int p = mPoints.Count -1 ; p >= 0; --p)
		{
			var lIndexFaceFound = mFaces.FindIndex( (face)=>{ return face.has(p); } );
			bool is_used = (-1 != lIndexFaceFound);
			if( !is_used )
			{
				removePoint( p );
				++nbDeleted;
			}

		}
		//Debug.Log("on vient de supprimer "+ nbDeleted.ToString() +" sommets");
	}


	/// trouver les sommets qui sont seuls
	public List<int> getUnusedVertices()
	{
		List<int> result= new List<int>();
		for(int v = 0; v < mPoints.Count; ++v)
		{
			bool used = false;
			for(int f = 0; f < mFaces.Count; ++f)
			{
				if( mFaces[f].has( v ))
				{
					used = true;
					break;
				}
			}
			if(!used)
			{
				result.Add( v );
			}
		}
		return result;
	}

	// appeler getNext() ensuite dessus pour obtenir les links
	public Mesh3DLinkIterator getEdgeIterator()
	{
		return new Mesh3DLinkIterator( this );
	}

	/// calcule les liens et les renvoie dans une liste sans veritable doublons d'indexes
	public List<Edge> getEdges()
	{
		List<Edge> result = new List<Edge>();
		Mesh3DLinkIterator lIter = new Mesh3DLinkIterator(this);
		while(true)
		{
			var lEdge = lIter.getNext();
			if( lEdge == null )
			{
				break;
			}
			result.Add( lEdge );
		}
		return result;
	}

	/// renvoie la representation matricielle du graph correspondant au mesh3D
	public float[,] getAdjacencyMatrix()
	{
		int nbVertices = mPoints.Count;
		float[,] result = new float[ nbVertices, nbVertices];

		var lEdges = getEdges();
		foreach( var lEdge in lEdges)
		{
			Line3D lLine3D = lEdge.getLine3D( mPoints);
			float length = lLine3D.getLength();
			result[ lEdge.a, lEdge.b ] = length;
			result[ lEdge.b, lEdge.a ] = length;
		}
		return result;
	}

	/// renvoie la representation matricielle du graph, mais avec 1 de distance pour chaque edge
	public float[,] getAdjacencyMatrix_1_1()
	{
		int nbVertices = mPoints.Count;
		float[,] result = new float[ nbVertices, nbVertices];

		var lEdges = getEdges();
		foreach( var lEdge in lEdges)
		{
			result[ lEdge.a, lEdge.b ] = 1;
			result[ lEdge.b, lEdge.a ] = 1;
		}
		return result;
	}

	//-------------------------------------------------------
	//----------------affichage visuel ----------------------
	//-------------------------------------------------------

	public void visual_destroy()
	{
		if( mTransform != null )
		{
			Object.Destroy( mTransform );
		}
	}

	/// met a jour la position des elements 3D
	public void visual_update()
	{
		// verifie qu'on a bien le bon nombre d'elements
		visual_forceSameSizeElementsUnderParent();

		// mets a jour les positions et orientations des elements
		var lVisualPoints = mTransform.GetChild(0);
		for(int i = 0; i < mPoints.Count; ++i)
		{
			var lVisual = lVisualPoints.GetChild(i); 
			lVisual.position = mPoints[i];
		}

		var lVisualEdges = mTransform.GetChild(1);
		for(int t = 0; t < mFaces.Count; ++t)
		{
			var lFace = mFaces[t]; 
			for(int edge = 0; edge < 3; ++edge)
			{
				var lEdge = lFace.getLine3D( mPoints, edge);
				var lP1 = lEdge.p1;
				var lP2 = lEdge.p2;

				int fullIndexEdge = t * 3 + edge;
				var lVisual = lVisualEdges.GetChild( fullIndexEdge ); 
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

		var lVisualFaces = mTransform.GetChild(2);
		for(int f = 0; f < mFaces.Count; ++f)
		{
			var lVisual = lVisualFaces.GetChild(f);
			lVisual.position = calcCenterFace(f);
		}
	}

	/// changer la couleur d'un sommet a l'ecran
	public void visual_setPointColor(int vertexIndex, Color c)
	{
		if( mTransform == null )
		{
			return;
		}
		var lPoints = mTransform.GetChild(0);
		if( lPoints.childCount > vertexIndex && vertexIndex >=0)
		{
			Transform lVertexIndex = lPoints.GetChild( vertexIndex );
			var lRenderer = lVertexIndex.GetComponent<MeshRenderer>();
			lRenderer.sharedMaterial.color = c;
		}
	}

	/// acces a l'element visuel
	public Transform visual_getPoint(int vertexIndex )
	{
		if( mTransform == null )
		{
			return null;
		}
		var lPoints = mTransform.GetChild(0);
		if( lPoints.childCount > vertexIndex && vertexIndex >=0)
		{
			return lPoints.GetChild( vertexIndex );
		}else{
			return null;
		}
	}

	/// ajoute des points et des links si besoin, et desactive le surplus.
	/// ceci ne repositionne pas la 3D par contre.
	private void visual_forceSameSizeElementsUnderParent()
	{
		if( mTransform == null )
		{
			GameObject lNewGO = new GameObject(mName);
			mTransform = lNewGO.transform; 
		}

		if( mTransform.childCount == 0 )
		{
			// creer les parents pour les points et les links
			GameObject lPointsGO = new GameObject("points");
			lPointsGO.transform.parent = mTransform;
			GameObject lEdgesGO = new GameObject("links");
			lEdgesGO.transform.parent = mTransform;
			GameObject lFacesGO = new GameObject("faces");
			lFacesGO.transform.parent = mTransform;
		}

		// maj des sommets (creations/activations/desactivations)
		{
			var lPoints = mTransform.GetChild(0);
			for(int i = 0; i < mPoints.Count; ++i)
			{
				if( i < lPoints.childCount )
				{
					lPoints.GetChild(i).gameObject.SetActive( true );
				}else{
					var lCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					//var lCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					lCube.transform.parent = lPoints;
					//lCube.transform.localScale = Vector3.one * 0.2f;
					lCube.name = "p"+i.ToString();
					var lCubeRenderer = lCube.GetComponent<Renderer>();
					lCubeRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
					//lCubeRenderer.sharedMaterial.color = Color.black;
					lCubeRenderer.sharedMaterial.color = Color.white;

					var lTxt = new GameObject("t"+i.ToString());
					lTxt.transform.parent = lCube.transform;
					lTxt.transform.localScale = Vector3.one * 0.05f;
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
			int nbLinks = mFaces.Count * 3;
			var lEdges = mTransform.GetChild(1);
			for(int i = 0; i < nbLinks; ++i)
			{
				if( i < lEdges.childCount )
				{
					lEdges.GetChild(i).gameObject.SetActive( true );
				}else{
					var lCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					lCube.transform.parent = lEdges;
					lCube.transform.localScale = new Vector3(0.2f,0.2f,1.0f);
					lCube.name = "l"+i.ToString();
					lCube.GetComponent<MeshRenderer>().sharedMaterial = sLinkMaterial;
				}
			}
			for(int i = nbLinks; i < lEdges.childCount; ++i)
			{
				lEdges.GetChild(i).gameObject.SetActive( false );
			}
		}

		// maj des faces
		{
			int nbFaces = mFaces.Count;
			var lFaces = mTransform.GetChild(2);
			for(int i = 0; i < nbFaces; ++i)
			{
				if( i < lFaces.childCount )
				{
					lFaces.GetChild(i).gameObject.SetActive( true );
				}else{
					var lCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					lCube.transform.parent = lFaces;
					lCube.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
					lCube.name = "f"+i.ToString();
				}
			}
			for(int i = nbFaces; i < lFaces.childCount; ++i)
			{
				lFaces.GetChild(i).gameObject.SetActive( false );
			}
		}
	}

	/// change les positions dans le graph grace a celles de la scene (mouvement)
	public void visual_updateFromScene()
	{
		visual_forceSameSizeElementsUnderParent();

		var lVisualPoints = mTransform.GetChild(0);
		for(int i = 0; i < mPoints.Count; ++i)
		{
			var lVisual = lVisualPoints.GetChild(i);
			mPoints[i] = lVisual.position;
		}
	}

}


/// sert juste a parcourir les links
public class Mesh3DLinkIterator
{
	List<Edge> alreadySeen;
	Mesh3D mMesh;
	int currentFaceIndex;
	int currentFaceLinkIndex02;

	public Mesh3DLinkIterator(Mesh3D pMesh)
	{
		alreadySeen = new List<Edge>();
		mMesh = pMesh;
		currentFaceIndex = 0;
		currentFaceLinkIndex02 = 0;
	}

	// avance au link suivant.
	// renvoie null s'il n'y en a plus
	public Edge getNext()
	{
		if( mMesh.mFaces.Count <= currentFaceIndex)
		{
			return null;
		}
		var lFace = mMesh.mFaces[currentFaceIndex];
		Edge lEdge = lFace.getEdge(currentFaceLinkIndex02); 
		if( alreadySeen.Find( (already) =>{ return already.isSame_u( lEdge); } ) == null )
		{
			// pas deja vu
			alreadySeen.Add( lEdge );
			return lEdge;
		}

		// deja vu, il faut aller au suivant
		if( currentFaceLinkIndex02 == 0 || currentFaceLinkIndex02 == 1)
		{
			++currentFaceLinkIndex02;
			return getNext();
		}

		currentFaceLinkIndex02 = 0;
		++currentFaceIndex;
		return getNext();
	}

	public void reset()
	{
		alreadySeen.Clear();
		currentFaceIndex = 0;
		currentFaceLinkIndex02 = 0;
	}

}
