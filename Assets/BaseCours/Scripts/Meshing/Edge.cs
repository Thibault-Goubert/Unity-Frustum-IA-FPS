using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// lien entre deux elements (arete d'un graph)
public class Edge : IEquatable<Edge>
{
	//------------------------------------------------
	//----------------attributs-----------------------
	//------------------------------------------------

	/// indice du debut du lien (dans tableau points du graph2d)
	public int a;

	/// indice de fin  (dans tableau points du graph2d)
	public int b;

	/// true : on prend le sens en compte.
	/// false : on ne prend pas le sens en compte.
	/// je le mets ici, afin de faciliter l'usage par les eleves de Contains(), IndexOf() etc.. sur les listes
	public bool isOrdered;
	//------------------------------------------------
	//----------------methodes------------------------
	//------------------------------------------------

	public Edge()
	{
		a = 0;
		b = 0;
		isOrdered = false;
	}

	public Edge(int pIndiceDebut, int pIndiceFin)
	{
		a = pIndiceDebut;
		b = pIndiceFin;
	}

	public Edge(Edge pOther)
	{
		a = pOther.a;
		b = pOther.b;
	}

	public bool Equals(Edge pOther)
	{
		if( isOrdered)
		{
			return a == pOther.a && b == pOther.b && pOther.isOrdered;
		}
		else if( !pOther.isOrdered )
		{
			return (a == pOther.a && b == pOther.b) || (a == pOther.b && b == pOther.a);
		}
		return false;
	}

	/// true : est identique (si on ne prend pas en compte le sens de l'edge)
	/// false : pas le meme edge
	public bool isSame_u(Edge other)
	{
		return (other.a == a && other.b == b) || (other.b == a && other.a == b);
	}

	public bool hasBothVertices( int v1, int v2)
	{
		return (v1== a && v2 == b) || (v2 == a && v1 == b);
	}

	public bool hasVertexIndex(int vertexIndex)
	{
		return vertexIndex == a || vertexIndex == b;
	}

	public int getByIndex( int pIndex01)
	{
		if( pIndex01 == 0 )
		{
			return a;
		}else{
			return b;
		}
	}

	/// si 'a' vaut original, alors met newOne a la place.
	/// si 'b' vaut original, alors met newOne a la place.
	public void replace( int original, int newOne)
	{
		int newA = a;
		int newB = b;
		if( a == original )
		{
			newA = newOne;
		}
		if( b == original )
		{
			newB = newOne;
		}
		a = newA;
		b = newB;
	}

	/// s'arrange pour que b >= a, change le sens si besoin
	public void makeAscendingOrder()
	{
		if( a > b )
		{
			int temp = b;
			b = a;
			a = temp;
		}
	}

	public override string ToString ()
	{
		return "("+a.ToString()+","+b.ToString()+")";
	}

	public Line3D getLine3D(List<Vector3> pPoints)
	{
		return new Line3D( pPoints[a], pPoints[b] ); 
	}

	public bool isInList_unordered(List<Edge> edges)
	{
		var lFound = edges.Find( (e)=>{ return e.hasBothVertices(a,b); } );
		return lFound != null;
	}

};
