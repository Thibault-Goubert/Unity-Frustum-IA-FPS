using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// sert a dessiner de nombreux triangles
/// faire dans l'ordre : 
///    beginAddingTri()
///    addTriangle( mon triangle )
///    updateMesh()
public class TriangleDrawer
{
	/// la liste des triangles a afficher
	private List<Triangle3D> mTris; 

	/// le necessaire pour faire l'affichage dans unity3D
	private GameObject mGo;
	private Mesh mMesh;

	public TriangleDrawer()
	{
		mTris = new List<Triangle3D>();

		mMesh = new Mesh();
		mMesh.MarkDynamic();

		mGo = new GameObject("TriangleDrawer");
		var lFilter = mGo.AddComponent<MeshFilter>();
		lFilter.mesh = mMesh;

		var lRenderer = mGo.AddComponent<MeshRenderer>();
		lRenderer.material = new Material( Shader.Find("CoursVertexColor") );
	}

	/// pour dessiner tous les triangles en double face ou non
	public void setDoubleFace(bool doubleFace)
	{
		string lShaderName = doubleFace ? "CoursVertexColorDoubleFace" : "CoursVertexColor";

		var lRenderer = mGo.GetComponent<MeshRenderer>();
		lRenderer.material.shader = Shader.Find( lShaderName );
	}

	public MeshRenderer getRenderer()
	{
		return mGo.GetComponent<MeshRenderer>();
	}

	public int getNbTriangles()
	{
		return mTris.Count;
	}

	/// reducedQuantity : reduit un petit peu la taille de chaque triangle independamment
	public void clearAndDrawThese(List<Triangle3D> pTriangles, int color_rand_seed = -1 , float reducedQuantity = 0.0f)
	{
		mTris.Clear();

		if( color_rand_seed != -1 )
		{
			System.Random rand = new System.Random(color_rand_seed);
			for(int i = 0; i < pTriangles.Count; ++i)
			{
				var lTri = pTriangles[i];

				var lColor = new Color( 
					((float)rand.Next(0,255))/255.0f, 
					((float)rand.Next(0,255))/255.0f, 
					((float)rand.Next(0,255))/255.0f, 
					1.0f) ;

				if( reducedQuantity != 0.0f)
				{
					mTris.Add( new Triangle3D(lTri.A, lTri.B, lTri.C, lColor ).getReduced(reducedQuantity) );
				}else{
					mTris.Add( new Triangle3D(lTri.A, lTri.B, lTri.C, lColor ) );
				}
			}
		}else{
			if( reducedQuantity == 0.0f )
			{
				mTris.AddRange( pTriangles );
			}else{
				for(int i = 0; i < pTriangles.Count; ++i)
				{
					mTris.Add( new Triangle3D( pTriangles[i] ).getReduced(reducedQuantity) );
				}
			}
		}
		updateMesh();
	}


	/// vide la liste interne de triangle.
	/// il faudra faire des addTriangle puis un updateMesh pour que le resultat devienne visible
	public void beginAddingTri()
	{
		mTris.Clear();
	}

	public void addTriangle(Triangle3D t)
	{
		mTris.Add( t );
	}

	public void addTriangle(Vector3 a, Vector3 b, Vector3 c)
	{
		mTris.Add( new Triangle3D(a,b,c) );
	}

	public void addTriangle(Vector3 a, Vector3 b, Vector3 c, Color pColour)
	{
		mTris.Add( new Triangle3D(a,b,c, pColour) );
	}

	public void updateMesh()
	{
		mMesh.Clear();

		int nbVertices =  mTris.Count * 3 ;

		if( nbVertices == 0)
		{
			return;
		}

		var newVertices = new Vector3[  nbVertices ];
		var newNormals = new Vector3[  nbVertices ];
		var newColors = new Color[  nbVertices ];
		var newTriangles = new int[ nbVertices ];

		for(int t = 0; t < mTris.Count; ++t)
		{
			int indiceVerticeA = (3*t)+0;
			int indiceVerticeB = (3*t)+1;
			int indiceVerticeC = (3*t)+2;

			var tri = mTris[t];
			newVertices[indiceVerticeA] = tri.A;
			newVertices[indiceVerticeB] = tri.B;
			newVertices[indiceVerticeC] = tri.C;

			var normal = tri.getNormal();
			newNormals[indiceVerticeA] = normal;
			newNormals[indiceVerticeB] = normal;
			newNormals[indiceVerticeC] = normal;

			var color = tri.color;
			newColors[indiceVerticeA] = color;
			newColors[indiceVerticeB] = color;
			newColors[indiceVerticeC] = color;

			// faces
			newTriangles[indiceVerticeA] = indiceVerticeA;
			newTriangles[indiceVerticeB] = indiceVerticeB;
			newTriangles[indiceVerticeC] = indiceVerticeC;
		}

		mMesh.vertices = newVertices;
		mMesh.normals = newNormals;
		mMesh.colors = newColors;
		mMesh.triangles = newTriangles;
	}
}



