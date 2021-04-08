using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// spring mass node
/// https://github.com/aquillen/boids_oval/blob/gh-pages/mass_spring.js
/// 
/// il serait aussi possible d'utiliser la formulation suivante :
/// https://www.gamedev.net/tutorials/programming/math-and-physics/towards-a-simpler-stiffer-and-more-stable-spring-r3227/
/// 
public class SM_node
{
	public SM_node()
	{
		pos = Vector3.zero;
		vitesse = Vector3.zero;
		accel = Vector3.zero;
		mass = 1.0f;
	}

	/// position, vitesse, acceleration
	public Vector3 pos;
	public Vector3 vitesse;
	public Vector3 accel;

	/// la masse (comme en sciences physiques)
	public float mass;
}

/// spring mass : sprinp
public class SM_spring
{
	public SM_spring()
	{
		i = 0;
		j = 1;
		stiffness_coeff = 0.01f;
		damping = 0.01f;
		resting_length = 0.0f;
	}

	/// index des points auxquel ce spring est relie
	public int i;
	public int j;

	/// constante de raideur
	///  0 : infiniment mou, donc pas de force du tout
	public float stiffness_coeff;

	/// c'est le damping, c-a-d l'amortissement ("ratio de decroissance des oscillations"),
	/// c-a-d une valeur qui va reduire progressivement la force
	public float damping;

	/// longueur a l'arret
	public float resting_length;
}


/// un graph de nodes et de springs
public class SM_Graph
{
	/// sommets
	public List<SM_node> mNodes;

	/// links
	public List<SM_spring> mSprings;


	public float mDamping_nodes = 0.1f;

	//------------------------------------------------------------
	//----------------initialisation------------------------------
	//------------------------------------------------------------

	public void initNodesFrom( List<GVector3> pList, float[] pMasses)
	{
		mNodes = new List<SM_node>();
		for(int v = 0; v < pList.Count; ++v )
		{
			SM_node lNode = new SM_node();
			lNode.pos = pList[v]; 
			if( pMasses != null )
			{
				lNode.mass = pMasses[v];
			}
			mNodes.Add( lNode );
		}
	}
	public void initNodesFrom( List<Vector3> pList, float[] pMasses)
	{
		mNodes = new List<SM_node>();
		for(int v = 0; v < pList.Count; ++v )
		{
			SM_node lNode = new SM_node();
			lNode.pos = pList[v]; 
			if( pMasses != null )
			{
				lNode.mass = pMasses[v];
			}
			mNodes.Add( lNode );
		}
	}

	public void initSpringsFrom( List<Edge> pList, float[] pLengthResting)
	{
		mSprings = new List<SM_spring>();
		for(int l = 0; l < pList.Count; ++l )
		{
			SM_spring lSpring = new SM_spring();
			lSpring.i = pList[l].a;
			lSpring.j = pList[l].b;
			if( pLengthResting != null )
			{
				lSpring.resting_length = pLengthResting[l];
			}

			mSprings.Add( lSpring );
		}
	}

	public void setCoeffStiffnessForAll(float pStiffness)
	{
		foreach( var s in mSprings)
		{
			s.stiffness_coeff = pStiffness;
		}
	}

	public void setDampingForAllSprings(float pDamping)
	{
		foreach( var s in mSprings )
		{
			s.damping = pDamping;
		}
	}

	public void setDampingForAllNodes(float pDamping)
	{
		mDamping_nodes = pDamping;
	}

	//------------------------------------------------------------
	//----------------mise a jour---------------------------------
	//------------------------------------------------------------

	/// dans unity Time.deltaTime doit contenir la quantite de temps ecoulee.
	/// cependant, pour la stabilite des calculs, il est preferable de mettre toujours la meme valeur
	/// ex: 0.01f
	public void update(float deltaTime_s)
	{
		computeAcceleration(deltaTime_s);


		float maxAcceleration = 200.0f;
		for(int n = 0; n < mNodes.Count; ++n)
		{
			var nodei = this.mNodes[n];

			// je ne veux pas que cela soit trop bouncy
			if( maxAcceleration < nodei.accel.magnitude)
			{
				nodei.accel = nodei.accel.normalized * maxAcceleration;
			}

			// on deduit la vitesse de l'acceleration en la multipliant par le temps
			Vector3 dv = nodei.accel * deltaTime_s; 
			nodei.vitesse += dv;

			// on deduit la position de la vitesse en la multipliant par le temps
			Vector3 dr = nodei.vitesse * deltaTime_s;
			nodei.pos += dr;
		}
	}

	/// affecte ces changements a notre maillage 3D
	public void applyTo(Mesh3D pMesh)
	{
		for(int i = 0; i < pMesh.mPoints.Count; ++i)
		{
			pMesh.mPoints[i] = mNodes[i].pos;
		}
	}

	/// affecte ces changements a notre graph
	public void applyTo(Graph pGraph)
	{
		for(int i = 0; i < pGraph.mPoints.Count; ++i)
		{
			pGraph.mPoints[i] = mNodes[i].pos;
		}
	}

	#region version originale

	private void computeAcceleration(float deltaTime_s)
	{
		// apply springs
		for( int s = 0; s < mSprings.Count; ++s)
		{
			calc_spring_force(s);
		}

		// faire le damping
		calc_damping_forAll();
	}

	/// met a jours les accelerations, en calculant l'amortissement (damping)
	/// et en l'appliquant aux accelerations en cours
	private void calc_damping_forAll()
	{
		for(int i = 0; i< mNodes.Count ; i++)
		{
			var lNode = mNodes[i];

			// mettre le damping sur la vitesse
			// "l'amortissement est proportionnel au carre de la vitesse", et influence la vitesse. Pas l'acceleration.
			// source : http://maron.perso.univ-pau.fr/meca_old/ch8.htm#ch8-2
			if( mDamping_nodes > 0 )
			{
				float dampingNode = (Vector3.Dot(lNode.vitesse, lNode.vitesse) * mDamping_nodes);
				dampingNode = Mathf.Clamp01( dampingNode );

				lNode.vitesse -=  lNode.vitesse.normalized * dampingNode;
			}

			/* dans le code original, berk :
			var dForce = lNode.vitesse * mDamping_nodes ;
			lNode.accel -= dForce;
			*/
		}
	}

	/// pSpringIndex est le numero de la spring
	private void calc_spring_force(int pSpringIndex)
	{
		int i = mSprings[pSpringIndex].i;
		int j = mSprings[pSpringIndex].j;

		var nodei = mNodes[i];
		var nodej = mNodes[j];
		float stiffness = mSprings[pSpringIndex].stiffness_coeff;
		//float damping = mSprings[pSpringIndex].damping;
		float resting_length = mSprings[pSpringIndex].resting_length;

		Vector3 JI = nodei.pos - nodej.pos; // difference dx vector
		float currentLength = JI.magnitude; // length

		// direction JI
		var JI_dir = new Vector3( JI.x, JI.y, JI.z ).normalized;

		// kForce is spring force
		float length_extension = currentLength-resting_length;
		Vector3 kForce_JI = JI_dir * (stiffness *  length_extension / resting_length );  // is a vector in the direction between nodes
		Vector3 kJI_accel_i = kForce_JI / nodei.mass;  // acceleration on node i
		Vector3 kJI_accel_j = kForce_JI / nodej.mass;  // acceleration on node j

		// sum spring accelerations on nodes
		nodei.accel -= (kJI_accel_i); // store by adding to node acceleration
		nodej.accel += (kJI_accel_j);

		/*
		// mettre le damping sur la vitesse
		// "l'amortissement est proportionnel au carre de la vitesse", et influence la vitesse.
		// source : http://maron.perso.univ-pau.fr/meca_old/ch8.htm#ch8-2
		if( damping > 0 )
		{
			float dampingI = (Vector3.Dot(nodei.vitesse, nodei.vitesse) * damping);
			float dampingJ = (Vector3.Dot(nodej.vitesse, nodej.vitesse) * damping);

			nodei.vitesse -=  nodei.vitesse.normalized *  ;
			nodej.vitesse -=  nodej.vitesse.normalized * (Vector3.Dot(nodej.vitesse, nodej.vitesse) * damping) ;
		}
		*/
		/*
		// dForce is damping force
		if( damping > 0 )
		{
			Vector3 JI_dV = nodei.vitesse - nodej.vitesse;  // dV vector
			float dxdotdv = Vector3.Dot(JI, JI_dV); // dx dot dv
			float mu_mass = nodei.mass * nodej.mass / (nodei.mass + nodej.mass); // reduced mass

			float epsilon = 0.00001f;

			Vector3 dForce = JI_dir *(damping * dxdotdv * mu_mass / (currentLength + epsilon ));
			Vector3 dai = dForce * nodei.mass;  // acceleration on node i
			Vector3 daj = dForce * nodej.mass;  // acceleration on node j
			// sum damping accelerations on nodes
			nodei.accel -= (dai); // store by adding to node acceleration
			nodej.accel += (daj);
		}*/
	}

	#endregion

}


