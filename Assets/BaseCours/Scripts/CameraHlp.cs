using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// prevu pour avoir des fonctions d'aide sur la camera
public class CameraHlp
{
	/// pour le cours, on vient mettre la camera a un emplacement predefini avec une vue X horizontal, Y vertical
	public static void setup2DCamera()
	{
		var lCam = Camera.main;
		Camera.main.transform.localPosition = new Vector3(0,0,-10);
		lCam.transform.localRotation = Quaternion.identity; 
		lCam.orthographic = true;
		lCam.orthographicSize = 20;
		lCam.backgroundColor = Color.gray;
		lCam.clearFlags = CameraClearFlags.SolidColor;
	}

	public static Camera setup3DCamera()
	{
		var lCam = Camera.main;
		Camera.main.transform.localPosition = new Vector3(0,0,-10);
		lCam.transform.localRotation = Quaternion.identity; 
		lCam.orthographic = false;
		lCam.nearClipPlane = 1.0f;
		lCam.farClipPlane = 2000.0f;
		lCam.backgroundColor = Color.gray;
		lCam.clearFlags = CameraClearFlags.SolidColor;
		return lCam;
	}

	/// trouver le coin en haut a gauche visible, sur le plan Z==0
	public Vector3 getTopLeftCornerZ()
	{
		Ray lRay = Camera.main.ScreenPointToRay(new Vector3(0,Screen.height-1,0));
		Plane p = new Plane(new Vector3(0,0,1), Vector3.zero);
		float distance;
		if( p.Raycast( lRay, out distance) )
		{
			return lRay.GetPoint( distance );
		}else{
			// souci
			return new Vector3(-1,1,0);
		}
	}

	/// trouver le coin en bas a gauche visible, sur le plan Z==0
	public Vector3 getBottomLeftCornerZ()
	{
		Ray lRay = Camera.main.ScreenPointToRay(new Vector3(0,0,0));
		Plane p = new Plane(new Vector3(0,0,1), Vector3.zero);
		float distance;
		if( p.Raycast( lRay, out distance) )
		{
			return lRay.GetPoint( distance );
		}else{
			// souci
			return new Vector3(-1,-1,0);
		}
	}

	/// trouver le coin en haut a gauche visible, sur le plan Z==0
	public Vector3 getTopRightCornerZ()
	{
		Ray lRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width-1,Screen.height-1,0));
		Plane p = new Plane(new Vector3(0,0,1), Vector3.zero);
		float distance;
		if( p.Raycast( lRay, out distance) )
		{
			return lRay.GetPoint( distance );
		}else{
			// souci
			return new Vector3(1,1,0);
		}
	}

	/// renvoie le vecteur du "bottom left"(BL) au "top right"(TR)
	public Vector3 getDiagonal_BL_to_TR()
	{
		return getTopRightCornerZ() - getBottomLeftCornerZ();
	}
}
