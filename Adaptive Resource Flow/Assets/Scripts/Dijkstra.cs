using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dijkstra
{
	public static DijkstraSolution Algorithm(
		int startNode, 
		int endNode, 
		float[,] distances, 
		float distanceExponent)
	{
		// startNode: Index of start node
		// endNode: Index of end node
		// distances: Distance between nodes [a, b]
		// distanceExponent: Exponent for cost of traversing a distance
		int nNodes = distances.GetLength(0); // Number of nodes in network
		List<int> unvisitedNodes = new List<int>(); // List of unvisited nodes
		float[] minNodeCosts = new float[nNodes]; // Array of lowest cost of reaching nodes
		int currentNode; // Index of currently selected node
		bool terminate = false; // Signals that minimum end node cost has been found
		int[] previousNode = new int[nNodes]; // Previous node in the cheapest path for node with index a

		// Add all nodes to the unvisited set
		for (int n = 0; n < nNodes; n++) unvisitedNodes.Add(n);

		// Assign tentative cost values
		for (int n = 0; n < nNodes; n++) minNodeCosts[n] = float.MaxValue;
		minNodeCosts[startNode] = 0f;

		// Until solution is found
		while (!terminate)
		{
			// Select the unvisited node with lowest cost
			currentNode = DijkstraSelect(unvisitedNodes, minNodeCosts);
			
			// Calculate tentative costs
			for (int n = 0; n < nNodes; n++)
			{
				if (unvisitedNodes.Contains(n))
				{
					float tentativeCost = minNodeCosts[currentNode] + Mathf.Pow(distances[currentNode, n], distanceExponent);
					if (tentativeCost < minNodeCosts[n])
					{
						minNodeCosts[n] = tentativeCost;
						previousNode[n] = currentNode;
					}
				}
			}

			// Remove current node from the unvisited set
			unvisitedNodes.Remove(currentNode);

			// Check for termination
			if (!unvisitedNodes.Contains(endNode)) terminate = true;
			else if (DijkstraSelect(unvisitedNodes, minNodeCosts) == -1) terminate = true;
		}

		// Build path
		List<int> path = new List<int>();
		if (minNodeCosts[endNode] < float.MaxValue)
		{
			currentNode = endNode;
			while (currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = previousNode[currentNode];
			}
			path.Add(currentNode);
		}
		int[] bestPath = path.ToArray();
		System.Array.Reverse(bestPath);

		// Build solution
		DijkstraSolution solution = new DijkstraSolution(minNodeCosts[endNode], bestPath);

		// Output solution
		return solution;
	}

	public static int DijkstraSelect(List<int> unvisitedNodes, float[] minNodeCosts)
	{
		// Initialise
		float min = float.MaxValue;
		int minIndex = -1;

		// Iterate through nodes
		foreach (int n in unvisitedNodes) if (minNodeCosts[n] < min)
		{
			min = minNodeCosts[n];
			minIndex = n;
		}

		// Return index of lowest cost
		return minIndex;
	}
}

public class DijkstraSolution
{
	public float cost;
	public int[] path;

	public DijkstraSolution(float c, int[] p)
	{
		cost = c;
		path = p;
	}
}