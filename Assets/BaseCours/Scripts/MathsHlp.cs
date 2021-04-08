using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsHlp
{
	// extensions pour les vector3
	public static float longueur(this Vector3 pVector)
	{
		return pVector.magnitude;
	}

	// extensions pour les vector3
	public static float length(this Vector3 pVector)
	{
		return pVector.magnitude;
	}

	// source : https://answers.unity.com/questions/11363/converting-matrix4x4-to-quaternion-vector3.html
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{ 
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	/// multiplie un vecteur 3 par matrice 4, en passant temporairement par des coordonnees homogenes
	/// puis retourne en coordonnees cartesienne en divisant par la quatrieme composante
	public static Vector3 multiplication_homogene( Matrix4x4 pMatrix, Vector3 pVector)
	{
		// remarque :  pMatrix.MultiplyPoint( pVector); fait la meme chose, je l'ai verifie avec des multiplications de nombres premiers.


		// passage en coordonnees homogenes
		var lV4 = new Vector4( pVector.x, pVector.y, pVector.z, 1);

		// multiplication d'elements de la meme taille
		var lNewV4 = pMatrix * lV4;

		// passage en coordonnees cartesiennes
		if( lNewV4.w == 0)
		{
			return new Vector3(lNewV4.x, lNewV4.y, lNewV4.z);
		}else{
			float div = 1.0f / lNewV4.w;
			return new Vector3(lNewV4.x * div, lNewV4.y * div, lNewV4.z * div);
		}
	}

	/// return true si les deux sont quasiment identiques
	public static bool approximately(Vector3 a, Vector3 b)
	{
		return Mathf.Approximately(a.x, b.x)
			&& Mathf.Approximately(a.y, b.y)
			&& Mathf.Approximately(a.z, b.z);
	}


	/// pour une ligne A->B.
	/// dit si le point pPointToTest est du "meme cote de la ligne" que ne l'est le point pSideA 
	public static bool isSameSideAs(Vector3 pA_fromLine, Vector3 pB_fromLine, Vector3 pSideA, Vector3 pPointToTest)
	{
		var lLineDir = pB_fromLine - pA_fromLine;
		var lPerp1 = Vector3.Cross( lLineDir, pSideA-pA_fromLine );
		var lPerp2 = Vector3.Cross( lLineDir, pPointToTest-pA_fromLine );
		return Vector3.Dot( lPerp1, lPerp2 ) > 0;
	}

	/// renvoie true si P est dans le triangle ABC (en admettant qu'on soit en 2D)
	public static bool isInsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
	{
		var PA = A-P;
		var PB = B-P;
		var PC = C-P;

		bool sens1 = Vector3.Dot( PA, PB) > 0;
		bool sens2 = Vector3.Dot( PB, PC) > 0;
		bool sens3 = Vector3.Dot( PC, PA) > 0;

		return sens1 == sens2 || sens2 == sens3;
	}

	/// renvoie l'aire du triangle
	public static float calculateAreaTriangle(Vector3 A, Vector3 B, Vector3 C)
	{
		var BA = A-B;
		var CA = A-C;
		var BC = C-B;

		float a = BA.magnitude;
		float b = BC.magnitude;
		float c = CA.magnitude;

		float s = (a + b + c) * 0.5f;  
		return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
	}

	public static float calculateRayonCercleCircomscrit(Vector3 A, Vector3 B, Vector3 C)
	{
		// calcul de l'aire
		var BA = A-B;
		var CA = A-C;
		var BC = C-B;

		float a = BA.magnitude;
		float b = BC.magnitude;
		float c = CA.magnitude;

		float s = (a + b + c) * 0.5f;  
		float area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));

		// calcul du rayon : R = (abc) / (4 * A)
		float rayon = a*b*c / (4.0f * area);
		return rayon;
	}

	/// distance entre un point et un segment (pas une ligne infinie)
	/// https://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
	public static float distancePointToSegment2D(Vector3 p, Vector3 segment_p1,  Vector3 segment_p2)
	{
		float x = p.x;
		float y = p.y;

		float x1 = segment_p1.x;
		float y1 = segment_p1.y;
		float x2 = segment_p2.x;
		float y2 = segment_p2.y;

		var A = x - x1;
		var B = y - y1;
		var C = x2 - x1;
		var D = y2 - y1;

		var dot = A * C + B * D;
		var len_sq = C * C + D * D;
		float param = -1.0f;
		if (len_sq != 0.0f) //in case of 0 length line
			param = dot / len_sq;


		float xx = 0;
		float yy = 0;
		if (param < 0) {
			xx = x1;
			yy = y1;
		}
		else if (param > 1) {
			xx = x2;
			yy = y2;
		}
		else {
			xx = x1 + param * C;
			yy = y1 + param * D;
		}

		var dx = x - xx;
		var dy = y - yy;
		return Mathf.Sqrt(dx * dx + dy * dy);
	}

}


