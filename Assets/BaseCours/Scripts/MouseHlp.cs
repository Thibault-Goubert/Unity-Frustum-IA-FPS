using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// aide pour obtention de la souris
public class MouseHlp
{
	/// 00 est en bas a gauche.
	/// dt : down->top
	/// td : top->down
	/// px : pixels
	public static Vector3 getMousePosition_dt_px()
	{
		return Input.mousePosition;
	}

	/// 00 est en haut a gauche.
	/// dt : down->top
	/// td : top->down
	/// px : pixels
	public static Vector3 getMousePosition_td_px()
	{
		return new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Input.mousePosition.z);
	}

	/// renvoie la position 3D de la souris projetee sur le plan Z == pZ
	public static Vector3 getPositionOnZ(float pZ = 0)
	{
		var lCamera = Camera.main;
		var lRay = lCamera.ScreenPointToRay( getMousePosition_dt_px() );

		Plane p = new Plane(new Vector3(0,0,1), new Vector3(0,0,pZ));
		float lDistance;
		if( p.Raycast( lRay, out lDistance))
		{
			return lRay.GetPoint(lDistance);
		}
		return Vector3.zero;
	}

}
