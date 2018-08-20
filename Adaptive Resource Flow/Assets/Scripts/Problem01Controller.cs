using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem01Controller : MonoBehaviour
{
	public int nNodes = 8;

	private float[] nodeX;
	private float[] nodeY;
	private float[,] distance;
	private float[,] minCost;
	private IDictionary<Vector2, int[]> minCostPath;

	void Start () {
		// Initialise
		nodeX = new float[nNodes];
		nodeY = new float[nNodes];
		distance = new float[nNodes, nNodes];
		minCost = new float[nNodes, nNodes];
		minCostPath = new Dictionary<Vector2, int[]>();

		// Generate random nodes
		for (int n = 0; n < nNodes; n++)
		{
			nodeX[n] = Random.value * 10f;
			nodeY[n] = Random.value * 10f;
		}

		// Calculate distances
		for (int m = 0; m < nNodes; m++)
		{
			for (int n = 0; n < nNodes; n++)
			{
				distance[m, n] = Mathf.Sqrt(Mathf.Pow(nodeX[m] - nodeX[n], 2) + Mathf.Pow(nodeY[m] - nodeY[n], 2));
			}
		}

		// Find shortest paths
		for (int m = 0; m < nNodes; m++)
		{
			for (int n = 0; n < nNodes; n++)
			{
				DijkstraSolution solution = Dijkstra(m, n);
				minCost[m, n] = solution.Cost;
				UpdateMinCostPath(m, n, solution.Path);
			}
		}

		foreach (int i in minCostPath[new Vector2(0, 7)])
		{
			//Debug.Log(i);
		}
	}
	
	void Update () {
		
	}

	void OnDrawGizmos()
	{
		// Draw nodes
		Gizmos.color = Color.green;
		for (int n = 0; n < nNodes; n++)
		{
			Gizmos.DrawSphere(new Vector3(nodeX[n], 0, nodeY[n]), 0.2f);
		}
	}

	DijkstraSolution Dijkstra(int start, int end)
	{
		List<int> unvisited = new List<int>();
		float[] cost = new float[nNodes];
		int current;
		bool terminate = false;
		int[] prev = new int[nNodes];

		// Add all nodes to unvisited set
		for (int n = 0; n < nNodes; n++) unvisited.Add(n);

		// Assign tentative cost values
		for (int n = 0; n < nNodes; n++) cost[n] = float.MaxValue;

		// Set start cost to zero
		cost[start] = 0f;

		while (!terminate)
		{
			// Select the lowest cost unvisited node
			current = DijkstraSelect(unvisited, cost);

			// Calculate tentative costs
			for (int n = 0; n < nNodes; n++)
			{
				if (unvisited.Contains(n))
				{
					float tentative = cost[current] + Mathf.Pow(distance[current, n], 1.1f);
					if (tentative < cost[n])
					{
						cost[n] = tentative;
						prev[n] = current;
					}
				}
			}

			// Remove current from unvisited set
			unvisited.Remove(current);

			// Check for termination
			if (!unvisited.Contains(end)) terminate = true;
		}

		// Build path
		List<int> path = new List<int>();
		current = end;
		while (current != start)
		{
			path.Add(current);
			current = prev[current];
		}
		path.Add(current);
		int[] bestPath = path.ToArray();
		System.Array.Reverse(bestPath);

		// Buid solution
		DijkstraSolution solution = new DijkstraSolution(cost[end], bestPath);

		// Output solution
		return solution;
	}

	int DijkstraSelect(List<int> unvisited, float[] cost)
	{
		float min = float.MaxValue;
		int minIndex = -1;

		foreach (int n in unvisited)
		{
			if (cost[n] < min)
			{
				min = cost[n];
				minIndex = n;
			}
		}

		return minIndex;
	}

	void UpdateMinCostPath(int a, int b, int[] p)
	{
		Vector2 key = new Vector2(a, b);
		if (minCostPath.ContainsKey(key))
		{
			minCostPath[key] = p;
		}
		else
		{
			minCostPath.Add(key, p);
		}
	}
}

public class DijkstraSolution
{
	private float cost; public float Cost { get { return cost; } }
	private int[] path; public int[] Path { get { return path; } }

	public DijkstraSolution(float c, int[] p)
	{
		cost = c;
		path = p;
	}
}
