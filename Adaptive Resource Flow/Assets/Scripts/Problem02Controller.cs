using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem02Controller : MonoBehaviour
{
	public int nNodes = 12;
	public float totalProduction = 5f;
	public float distanceExponent = 1.25f;

	private float[] nodeX;
	private float[] nodeY;
	private float[,] distance;
	private float[,] minCost;
	private IDictionary<Vector2, int[]> minCostPath;
	private float[] rate;

	void Start()
	{
		// Initialise
		minCost = new float[nNodes, nNodes];
		minCostPath = new Dictionary<Vector2, int[]>();

		// Generate random nodes
		nodeX = RandomValues(nNodes, nNodes);
		nodeY = RandomValues(nNodes, nNodes);

		// Calculate distances
		distance = CalculateDistances(nodeX, nodeY);

		// Find shortest paths
		for (int a = 0; a < nNodes; a++) for (int b = 0; b < nNodes; b++)
		{
			DijkstraSolution solution = Dijkstra.Algorithm(a, b, distance, distanceExponent);
			minCost[a, b] = solution.cost;
			UpdateMinCostPath(a, b, solution.path);
		}

		// Generate random sources and sinks

	}

	private float[] RandomValues(int num, float scale)
	{
		float[] values = new float[num];
		for (int i = 0; i < num; i++) values[i] = Random.value * scale;
		return values;
	}

	private float[,] CalculateDistances(float[] x, float[] y)
	{
		int n = x.Length;
		float[,] d = new float[n, n];
		for (int a = 0; a < n; a++) for (int b = 0; b < n; b++)
			d[a, b] = Mathf.Sqrt(
				Mathf.Pow(x[a] - x[b], 2) + 
				Mathf.Pow(y[a] - y[b], 2));
		return d;
	}

	private void UpdateMinCostPath(int a, int b, int[] p)
	{
		Vector2 key = new Vector2(a, b);
		if (minCostPath.ContainsKey(key)) minCostPath[key] = p;
		else minCostPath.Add(key, p);
	}

	private float[] RandomRates(float t)
	{
		
	}
}
