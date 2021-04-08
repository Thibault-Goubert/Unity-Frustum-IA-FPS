using UnityEngine;
using System.Collections.Generic;

/// comme Vector3, mais c'est une classe.
/// cela permet de modifier plus facilement les elements dans les tableaux.
/// exemple : montableau[i].x *= 3.0f;    // ceci ne fonctionnerait pas avec des Vector3 de Unity3D
public class GVector3
{
	public float x;
	public float y;
	public float z;

	public GVector3()
	{
		x = 0; 
		y = 0;
		z = 0;
	}
	public GVector3(GVector3 p)
	{
		x = p.x;
		y = p.y;
		z = p.z;
	}
	public GVector3(Vector3 p)
	{
		x = p.x;
		y = p.y;
		z = p.z;
	}
	public GVector3(float px, float py, float pz)
	{
		x = px;
		y = py;
		z = pz;
	}
	public float length()
	{
		return Mathf.Sqrt(x*x+y*y+z*z);
	}
	public float longueur()
	{
		return Mathf.Sqrt(x*x+y*y+z*z);
	}
	public float magnitude
	{
		get{return Mathf.Sqrt(x*x+y*y+z*z);}
	}
	public GVector3 getNormalized()
	{
		float lLength = Mathf.Sqrt(x*x+y*y+z*z);
		if( lLength != 0.0f)
		{
			return new GVector3(x/lLength,y/lLength,z/lLength);
		}else{
			return new GVector3(1,0,0);
		}
	}
	public static GVector3 operator+(GVector3 a, Vector3 p)
	{
		return new GVector3(a.x + p.x, a.y + p.y, a.z + p.z);
	}
	public static GVector3 operator-(GVector3 a, Vector3 p)
	{
		return new GVector3(a.x - p.x, a.y - p.y, a.z - p.z);
	}
	public static GVector3 operator*(GVector3 a, Vector3 p)
	{
		return new GVector3(a.x * p.x, a.y * p.y, a.z * p.z);
	}
	public static GVector3 operator*(Vector3 p, GVector3 a)
	{
		return new GVector3(a.x * p.x, a.y * p.y, a.z * p.z);
	}
	public static GVector3 operator*(GVector3 a, float p)
	{
		return new GVector3(a.x * p, a.y * p, a.z * p);
	}
	public static GVector3 operator*(float p, GVector3 a)
	{
		return new GVector3(a.x * p, a.y * p, a.z * p);
	}
	public static GVector3 operator/(GVector3 a, float p)
	{
		return new GVector3(a.x / p, a.y / p, a.z / p);
	}
	public static GVector3 operator -(GVector3 a)
	{
		return new GVector3(-a.x, -a.y, -a.z);
	}
	// operateur de conversion
	public static implicit operator Vector3(GVector3 d)
	{
		return new Vector3(d.x,d.y,d.z);
	}
	// operateur de conversion
	public static implicit operator GVector3(Vector3 d)
	{
		return new GVector3(d.x,d.y,d.z);
	}

	public bool isApproximately(Vector3 d)
	{
		return Mathf.Approximately(d.x, x) && Mathf.Approximately(d.y, y) && Mathf.Approximately(d.z, z);
	}
	public bool isApproximately(GVector3 d)
	{
		return Mathf.Approximately(d.x, x) && Mathf.Approximately(d.y, y) && Mathf.Approximately(d.z, z);
	}

	/// faciliter la creation d'une liste de GVector3.
	/// ils sont tous a la valeur par defaut : 0,0,0
	public static List<GVector3> sCreateList(int nbElements)
	{
		List<GVector3> lNewList = new List<GVector3>(nbElements);
		for(int i = 0; i < nbElements; ++i)
		{
			lNewList.Add( new GVector3());
		}
		return lNewList;
	}

}