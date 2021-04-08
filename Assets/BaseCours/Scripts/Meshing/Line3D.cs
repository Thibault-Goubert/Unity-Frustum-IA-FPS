using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// un segment en 3D
public class Line3D
{
	/// une extremite du segment
	public Vector3 p1;
	/// une extremite du segment
	public Vector3 p2;

	public Line3D(Vector3 pp1, Vector3 pp2)
	{
		p1 = pp1;
		p2 = pp2;
	}

	/// pIndex01 vaut 0 => renvoie p1
	/// pIndex01 vaut 1 => renvoie p2
	/// pIndex01 vaut autre chose => renvoie p2
	public Vector3 getPoint(int pIndex01)
	{
		return pIndex01 == 0 ? p1 : p2;
	}

	public Vector3 getAsVector()
	{
		return p2-p1;
	}

	public float getLength()
	{
		return (p2-p1).magnitude;
	}

	/// vrai si l'une des deux extremites est 'p'
	public bool hasExtremity( Vector3 p )
	{
		return MathsHlp.approximately( p, p1) || MathsHlp.approximately( p, p2);
	}

	/// dit s'il sont pareils (meme si l'ordre n'est pas le meme)
	/// 'u' pour 'unordered' => sans ordre
	public bool isSame_u( Vector3 pA, Vector3 pB)
	{
		return (MathsHlp.approximately( pA, p1) && MathsHlp.approximately( pB, p2))||
			(MathsHlp.approximately( pB, p1) && MathsHlp.approximately( pA, p2));
	}

	/// dit s'il sont pareils (meme si l'ordre n'est pas le meme)
	/// 'u' pour 'unordered' => sans ordre
	public bool isSame_u( Line3D pLine)
	{
		return (MathsHlp.approximately( pLine.p1, p1) && MathsHlp.approximately( pLine.p2, p2))||
			(MathsHlp.approximately( pLine.p2, p1) && MathsHlp.approximately( pLine.p1, p2));
	}

	/// dessine mais visible uniquement sur la vue scene
	public void drawInScene()
	{
		Debug.DrawLine( p1, p2 );
	}

	public Vector3 getCenter()
	{
		return ( p1 + p2 ) *0.5f;
	}

	//-----------------------------------------------------------------------------------------
	//-------------------------------operations sur des Listes --------------------------------
	//-----------------------------------------------------------------------------------------

	/// renvoie true si other est present dans pLines (non ordonne)
	public static bool sContainsLine_u(List<Line3D> pLines, Line3D other)
	{
		var found = pLines.Find( (x)=>{ return x.isSame_u( other ); } );
		return found != null;
	}

	/// renvoie l'indice de other dans pLines (non ordonne)
	public static int sIndexOfLine_u(List<Line3D> pLines, Line3D other)
	{
		int found = pLines.FindIndex( (x)=>{ return x.isSame_u( other ); } );
		return found;
	}
}
