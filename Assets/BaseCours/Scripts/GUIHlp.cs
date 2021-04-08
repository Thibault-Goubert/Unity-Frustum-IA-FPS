using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// petites aides pour les interfaces 2D
public class GUIHlp
{
	/// dessine un bouton avec une fleche pour deployer/contracter ce qui vient en dessous
	public static bool drawDeployArrow(bool isDeploy, string pLabel = null)
	{
		string toShow = isDeploy? "\u25BC" : "\u25B6";
		if( !string.IsNullOrEmpty(pLabel))
		{
			toShow = toShow + pLabel; 
		}
		if( GUILayout.Button( toShow, GUILayout.ExpandWidth(false)))
		{
			isDeploy= !isDeploy;
		}
		return isDeploy;
	}

}
