using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// dijkstra, source : 
/// https://www.geeksforgeeks.org/csharp-program-for-dijkstras-shortest-path-algorithm-greedy-algo-7/
/// je l'ai modifie un peu
public class GraphDistance
{
	// Function that implements Dijkstra's 
	// single source shortest path algorithm 
	// for a graph represented using adjacency 
	// matrix representation 
	//
	// src : numero du sommet concerne
	// graph : matrice d'adjacence
	// nbVertices : nombre de sommets dans le graph
	public static float[] dijkstra(int nbVertices, float[, ] graph, int src) 
	{ 
		float[] dist = new float[nbVertices]; // The output array. dist[i] 
		// will hold the shortest 
		// distance from src to i 

		// sptSet[i] will true if vertex 
		// i is included in shortest path 
		// tree or shortest distance from 
		// src to i is finalized 
		bool[] sptSet = new bool[nbVertices]; 

		// Initialize all distances as 
		// INFINITE and stpSet[] as false 
		for (int i = 0; i < nbVertices; i++) { 
			dist[i] = float.MaxValue; 
			sptSet[i] = false; 
		} 

		// Distance of source vertex 
		// from itself is always 0 
		dist[src] = 0; 

		// Find shortest path for all vertices 
		for (int count = 0; count < nbVertices - 1; count++) { 
			// Pick the minimum distance vertex 
			// from the set of vertices not yet 
			// processed. u is always equal to 
			// src in first iteration. 
			int u = minDistance( nbVertices, dist, sptSet); 

			// Mark the picked vertex as processed 
			sptSet[u] = true; 

			// Update dist value of the adjacent 
			// vertices of the picked vertex. 
			for (int v = 0; v < nbVertices; v++) 

				// Update dist[v] only if is not in 
				// sptSet, there is an edge from u 
				// to v, and total weight of path 
				// from src to v through u is smaller 
				// than current value of dist[v] 
				if (!sptSet[v] && graph[u, v] != 0 &&  
					//dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v]) 
					dist[u] != float.MaxValue && dist[u] + graph[u, v] < dist[v]) // correction par pierre
					dist[v] = dist[u] + graph[u, v]; 
		} 

		return dist;
	} 


	// A utility function to find the 
	// vertex with minimum distance 
	// value, from the set of vertices 
	// not yet included in shortest 
	// path tree
	static int minDistance(int nbVertex, float[] dist, bool[] sptSet) 
	{ 
		// Initialize min value 
		float min = float.MaxValue;
		int min_index = -1; 

		for (int v = 0; v < nbVertex; v++) 
			if (sptSet[v] == false && dist[v] <= min) { 
				min = dist[v]; 
				min_index = v; 
			} 

		return min_index; 
	} 

}
