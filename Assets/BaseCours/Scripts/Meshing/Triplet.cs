using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// un triplet d'entier
public class Triplet
{
	public int a;
	public int b;
	public int c;

	public Triplet()
	{
		a= 0;
		b= 0;
		c= 0;
	}

	public Triplet(int pA, int pB, int pC)
	{
		a= pA;
		b= pB;
		c= pC;
	}

	public Triplet clone()
	{
		return new Triplet(a,b,c);
	}

	/// 0 => renvoie a
	/// 1 => renvoie b
	/// 2 => renvoie c
	public int getByIndex02( int pIndex02)
	{
		if( pIndex02 == 0 )
		{
			return a;
		}else if( pIndex02 == 1 )
		{
			return b;
		}else{
			return c;
		}
	}

	/// pour affecter a,b ou c.
	/// on choisit lequel grace a pIndex02.
	/// la nouvelle valeur sera pValue.
	public void setByIndex02( int pIndex02, int pValue)
	{
		if( pIndex02 == 0)
		{
			a = pValue;
		}else if( pIndex02 == 1 )
		{
			b = pValue;
		}else if( pIndex02 == 2)
		{
			c = pValue;
		}else{
			Debug.LogError("erreur dans le calcul, on a recu un setByIndex02 : "+ pIndex02 + " " + pValue);
		}
	}

	/// vrai uniquement si 'i' est l'un des 3 numeros
	public bool has(int i)
	{
		return a == i || b == i || c == i;
	}

	/// vrai uniquement si les 2 extremites de l sont dans this
	/// ne teste pas le sens (u pour unordered)
	public bool has_u(Edge l)
	{
		return 
			(a == l.a || b == l.a || c == l.a) &&
			(a == l.b || b == l.b || c == l.b);
	}

	/// vrai si on trouve I et J parmi A,B,C
	public bool hasBoth(int i, int j)
	{
		return (a == i || b == i || c == i)
			&& (a == j || b == j || c == j);
	}

	/// true si jamais il y a les memes 3 elements, meme s'ils sont dans le desordre
	/// (le '_u' signifie 'unordered')
	public bool isSame_u( Triplet other)
	{
		return (a == other.a || b == other.a || c == other.a)
			&& (a == other.b || b == other.b || c == other.b)
			&& (a == other.c || b == other.c || c == other.c);
	}

	/// renvoie l'un des trois link, selon qu'on demande 0,1,2.
	public Edge getEdge(int pIndexLink02)
	{
		if( pIndexLink02 == 0 )
		{
			return new Edge( a,b);
		}else if( pIndexLink02 == 1)
		{
			return new Edge( b, c);
		}else if( pIndexLink02 == 2)
		{
			return new Edge( c, a);
		}else{
			Debug.LogError("recu une erreur dans les parametres : "+ pIndexLink02);
			return new Edge( c, a);
		}
	}

	public Edge getEdge0()
	{
		return new Edge(a,b);
	}

	public Edge getEdge1()
	{
		return new Edge(b,c);
	}

	public Edge getEdge2()
	{
		return new Edge(c,a);
	}

	/// renvoie la ligne 3D correspondant a cet indice
	/// pPoints : tableau de sommets
	/// pIndexLineInFace_02 : 0 => renvoie a-b
	/// pIndexLineInFace_02 : 1 => renvoie b-c
	/// pIndexLineInFace_02 : 2 => renvoie c-a
	public Line3D getLine3D( List<Vector3> pPoints, int pIndexLineInFace_02 )
	{
		if( pIndexLineInFace_02 == 0 )
		{
			return new Line3D( pPoints[a], pPoints[b]);
		}else if( pIndexLineInFace_02 == 1 )
		{
			return new Line3D( pPoints[b], pPoints[c]);
		}else{
			return new Line3D( pPoints[c], pPoints[a]);
		}
	}

	/// renvoie le sommet 3D correspondant a cet indice
	/// pIndexLineInFace_02 : 0 => renvoie a
	/// pIndexLineInFace_02 : 1 => renvoie b
	/// pIndexLineInFace_02 : 2 => renvoie c
	public Vector3 getPoint3D( List<Vector3> pPoints, int pIndexInFace_02 )
	{
		if( pIndexInFace_02 == 0 )
		{
			return pPoints[a];
		}else if( pIndexInFace_02 == 1 )
		{
			return pPoints[b];
		}else{
			return pPoints[c];
		}
	}

	/// remplace previous par newOne
	/// exemple : si ('b' vaut 3) et (previous vaut 3) et (newOne vaut 7),  alors b = 7 apres cette operation
	public void replace(int previous, int newOne)
	{
		int newA = a;
		int newB = b;
		int newC = c;

		if( a == previous )
		{
			newA = newOne;
		}
		if( b == previous )
		{
			newB = newOne;
		}
		if( c == previous )
		{
			newC = newOne;
		}

		a = newA;
		b = newB;
		c = newC;
	}

	public override string ToString()
	{
		return "("+a.ToString()+","+b.ToString()+","+c.ToString()+")";
	}

	/// converti en string une liste de triplets
	public static string sToString(List<Triplet> tris)
	{
		string message = "{ ";
		foreach( var t in tris)
		{
			message +="("+t.a+","+t.b+","+t.c+") "; 
		}
		return message + "}";
	}
}

