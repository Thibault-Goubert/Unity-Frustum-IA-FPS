using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMeshUnity
{
	/// change l'ordre des sommets si besoin pour cela.
	public static Mesh sCreateMeshFromTriangles(string pMeshName, List<Triangle3D> pTriangles)
	{
		if( pTriangles.Count == 0)
		{
			return null;
		}

		Mesh lMesh = new Mesh();
		lMesh.name = pMeshName;

		int nbVertices =  pTriangles.Count * 3;

		var newVertices = new Vector3[  nbVertices ];
		var newNormals = new Vector3[  nbVertices ];
		var newColors = new Color[  nbVertices ];
		var newTriangles = new int[ nbVertices ];

		for(int t = 0; t < pTriangles.Count; ++t)
		{
			int indiceVerticeA = (3*t)+0;
			int indiceVerticeB = (3*t)+1;
			int indiceVerticeC = (3*t)+2;

			var lTriangle = pTriangles[t];
			newVertices[indiceVerticeA] = lTriangle.A;
			newVertices[indiceVerticeB] = lTriangle.B;
			newVertices[indiceVerticeC] = lTriangle.C;

			Vector3 normal = Vector3.Cross( lTriangle.B - lTriangle.A, lTriangle.C - lTriangle.A).normalized;
			newNormals[indiceVerticeA] = normal;
			newNormals[indiceVerticeB] = normal;
			newNormals[indiceVerticeC] = normal;

			var color = lTriangle.color;
			newColors[indiceVerticeA] = color;
			newColors[indiceVerticeB] = color;
			newColors[indiceVerticeC] = color;

			// faces
			newTriangles[indiceVerticeA] = indiceVerticeA;
			newTriangles[indiceVerticeB] = indiceVerticeB;
			newTriangles[indiceVerticeC] = indiceVerticeC;
		}

		lMesh.vertices = newVertices;
		lMesh.normals = newNormals;
		lMesh.colors = newColors;
		lMesh.triangles = newTriangles;

		return lMesh;
	}




}
