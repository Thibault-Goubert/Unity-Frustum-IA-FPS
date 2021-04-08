using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// contient 3 points et une couleur
public class Triangle3D
{
	public Vector3 A;
	public Vector3 B;
	public Vector3 C;
	public Color color;

	public Triangle3D()
	{
		A = Vector3.zero;
		B = new Vector3(1,0,0);
		C = new Vector3(0,-1,0);
		color = Color.magenta;
	}

	public Triangle3D(Vector3 a, Vector3 b, Vector3 c)
	{
		// ceci fait bien une copie des valeurs
		A = a;
		B = b;
		C = c;
		color = Color.magenta;
	}

	public Triangle3D(Vector3 a, Vector3 b, Vector3 c, Color pColor)
	{
		// ceci fait bien une copie des valeurs
		A = a;
		B = b;
		C = c;
		color = pColor;
	}

	public Triangle3D( Triangle3D other)
	{
		// ceci fait bien une copie des valeurs
		A = other.A;
		B = other.B;
		C = other.C;
		color = other.color;
	}

	/// true si les 3 points sont presents dans this
	public bool isSameAs( Vector3 pA, Vector3 pB, Vector3 pC)
	{
		bool testA1 = MathsHlp.approximately( A, pA );
		bool testA2 = MathsHlp.approximately( A, pB );
		bool testA3 = MathsHlp.approximately( A, pC );

		bool testB1 = MathsHlp.approximately( B, pA );
		bool testB2 = MathsHlp.approximately( B, pB );
		bool testB3 = MathsHlp.approximately( B, pC );

		bool testC1 = MathsHlp.approximately( C, pA );
		bool testC2 = MathsHlp.approximately( C, pB );
		bool testC3 = MathsHlp.approximately( C, pC );

		return 
			(testA1 && testB2 && testC3) ||
			(testA1 && testB3 && testC2) ||

			(testA2 && testB3 && testC1) ||
			(testA2 && testB1 && testC3) ||

			(testA3 && testB2 && testC1) ||
			(testA3 && testB1 && testC2);
	}

	/// renvoie une normale au plan A-B-C (dans cet ordre de rotation)
	public Vector3 getNormal()
	{
		return Vector3.Cross((B-A),(C-A)).normalized;
	}

	/// return true si jamais le point P est l'un des 3 sommets : A ou B ou C
	public bool hasVertex(Vector3 P)
	{
		return MathsHlp.approximately( A,P ) || MathsHlp.approximately( B,P ) || MathsHlp.approximately( C,P );
	}

	/// -1 s'il n'est pas dedans
	/// 0 si c'est A
	/// 1 si c'est B
	/// 2 si c'est C
	public int getIndexOf(Vector3 P)
	{
		if( MathsHlp.approximately( A,P ))
		{
			return 0;
		}
		else if( MathsHlp.approximately( B,P ) )
		{
			return 1;
		}else if( MathsHlp.approximately( C,P ))
		{
			return 2;
		}else{
			return -1;
		}
	}

	/// pIndex02 : 0 => renvoie A
	/// pIndex02 : 1 => renvoie B
	/// pIndex02 : 2 => renvoie C
	/// pIndex02 : autre chose => renvoie C
	public Vector2 getVertex(int pIndex02)
	{
		if( pIndex02 == 0 )
		{
			return A;
		}else if( pIndex02 == 1)
		{
			return B;
		}else{
			return C;
		}
	}

	/// 0 : renvoie le segment AB
	/// 1 : renvoie le segment BC
	/// 2 : renvoie le segment CA
	/// pIndex02 : autre chose => renvoie le segment CA
	public Line3D getEdge(int pIndex02)
	{
		if( pIndex02 == 0 )
		{
			return new Line3D(A,B);
		}else if( pIndex02 == 1)
		{
			return new Line3D(B,C);
		}else{
			return new Line3D(C,A);
		}
	}

	/// le barycentre du triangle (centre de gravite, souvent note G)
	public Vector3 getCenter()
	{
		return (A+B+C) / 3.0f;
	}

	/// renvoie un nouveau triangle presque identique : on a change la taille par rapport a son centre, en pourcentage
	/// ex: 1 : la taille reste la meme
	/// ex: 0.5 : la taille est divisee par 2
	public Triangle3D getScaled(float f01)
	{
		var lNewTri = new Triangle3D(this);
		lNewTri.scale(f01);
		return lNewTri;
	}

	/// renvoie un nouveau triangle plus petit que l'original
	/// on change sa taille par rapport a son centre
	public Triangle3D getReduced(float quantity)
	{
		var tri = new Triangle3D(this);
		var G = (tri.A + tri.B + tri.C) / 3.0f;

		var GA = (tri.A - G);
		var GB = (tri.B - G);
		var GC = (tri.C - G);

		float GAsize = GA.magnitude;
		float GBsize = GB.magnitude;
		float GCsize = GC.magnitude;

		var lNewGA = (GAsize - quantity) * GA / GAsize ;
		var lNewGB = (GBsize - quantity) * GB / GBsize;
		var lNewGC = (GCsize - quantity) * GC / GCsize;

		tri.A = G + lNewGA;
		tri.B = G + lNewGB;
		tri.C = G + lNewGC;

		return tri;
	}


	/// true si P1 et P2 sont 2 sommets de notre triangle
	/// si notre triangle est (P1, P2, C) => true
	/// si notre triangle est (P2, P1, C) => true (car non oriente)
	public bool hasEdge_nonOriented( Vector3 p1, Vector3 p2)
	{
		bool testA1 = MathsHlp.approximately( A, p1 );
		bool testA2 = MathsHlp.approximately( A, p2 );

		bool testB1 = MathsHlp.approximately( B, p1 );
		bool testB2 = MathsHlp.approximately( B, p2 );

		bool testC1 = MathsHlp.approximately( C, p1 );
		bool testC2 = MathsHlp.approximately( C, p2 );

		bool result = 
			(testA1 && testB2)
			|| (testA2 && testB1)
			|| (testB1 && testC2)
			|| (testB2 && testC1)
			|| (testC1 && testA2)
			|| (testC2 && testA1);
		return result;
	}

	/// true si edge[0] et edge[1] sont 2 sommets de notre triangle
	public bool hasEdge_nonOriented( Vector3[] edge)
	{
		return hasEdge_nonOriented(edge[0], edge[1]);
	}

	/// true si on a 1 edge en commun (non oriente)
	public bool hasAtLeastOneEdgeInCommon_nonOriented( Triangle3D other)
	{
		bool aOK = hasVertex( other.A);
		bool bOK = hasVertex( other.B);
		bool cOK = hasVertex( other.C);
		return (aOK && bOK) || (aOK && cOK) || (bOK && cOK);
	}


	/// le dessine sous forme de lignes dans la 'vue scene' de unity
	public void drawLines()
	{
		Debug.DrawLine( A, B );
		Debug.DrawLine( B, C );
		Debug.DrawLine( C, A );
	}

	// ------------------------------------------------------------------
	// ----------modification -------------------------------------------
	// ------------------------------------------------------------------

	/// reduit ou augmente la taille du triangle par rapport a son centre
	/// ex: 1 : la taille reste la meme
	/// ex: 0.5 : la taille est divisee par 2
	public void scale(float f01)
	{
		var center = (A+B+C)/ 3.0f;
		A = center + (A-center)*0.95f;
		B = center + (B-center)*0.95f;
		C = center + (C-center)*0.95f;
	}

	// ------------------------------------------------------------------
	// ----------operations sur des Listes de triangles------------------
	// ------------------------------------------------------------------

	/// changer la couleur de tous les triangles
	public static void sList_setColor(List<Triangle3D> pTriangles, Color pColor)
	{
		foreach( var tri in pTriangles)
		{
			tri.color = pColor;
		}
	}

	/// creer un nouvel ensemble a partir d'un mesh
	public static List<Triangle3D> sList_initFromMesh( Mesh pMesh, int rand_color = -1 )
	{
		var result = new List<Triangle3D>();
		if( pMesh == null )
		{
			return result;
		}

		System.Random rand = new System.Random( rand_color );

		var lVertices = pMesh.vertices;
		for(int s = 0; s < pMesh.subMeshCount ; ++s)
		{
			var lSubMeshIndices = pMesh.GetIndices( s );
			for(int i = 0; i < lSubMeshIndices.Length; i += 3) // on suppose que ce sont des triangles
			{
				int index0 = lSubMeshIndices[i];
				int index1 = lSubMeshIndices[i+1];
				int index2 = lSubMeshIndices[i+2];
				var lTri = new Triangle3D( lVertices[index0], lVertices[index1], lVertices[index2]);
				if( rand_color != -1 )
				{
					var lColor = new Color( 
						((float)rand.Next(0,255))/255.0f, 
						((float)rand.Next(0,255))/255.0f, 
						((float)rand.Next(0,255))/255.0f, 
						1.0f) ;
					lTri.color = lColor;
				}
				result.Add( lTri );
			}
		}

		return result;
	}
}
