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
	private float[] rate;
	private float[,] flow;

	List<int> source;
	List<int> sink;
	List<float> cost;
	float[] remain;

	private int frameCount;

	void Start () {
		// Initialise
		nodeX = new float[nNodes];
		nodeY = new float[nNodes];
		distance = new float[nNodes, nNodes];
		minCost = new float[nNodes, nNodes];
		minCostPath = new Dictionary<Vector2, int[]>();
		rate = new float[nNodes];
		flow = new float[nNodes, nNodes];
		frameCount = 0;

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

		// Generate random sources and sinks
		for (int i = 0; i < 2; i++)
		{
			int n = Mathf.FloorToInt(Random.value * nNodes);
			if (rate[n] == 0) rate[n] = 1;
			else i--;
		}
		for (int i = 0; i < 2; i++)
		{
			int n = Mathf.FloorToInt(Random.value * nNodes);
			if (rate[n] == 0) rate[n] = -1;
			else i--;
		}

		// Find each delivery option
		source = new List<int>();
		sink = new List<int>();
		cost = new List<float>();
		remain = new float[nNodes];
		for (int i = 0; i < nNodes; i++) remain[i] = rate[i];
		for (int m = 0; m < nNodes; m++)
		{
			if (rate[m] > 0)
			{
				for (int n = 0; n < nNodes; n++)
				{
					if (rate[n] < 0)
					{
						// Cost of delivering is minCost[m, n] along path minCostPath[(m, n)]
						source.Add(m);
						sink.Add(n);
						cost.Add(minCost[m, n]);
					}
				}	
			}
		}

		// Assign deliveries
	}
	
	void Update () {
		frameCount++;

		if (frameCount % 100 == 0)
		{
			AssignDelivery();
		}
	}

	void OnDrawGizmos()
	{
		// Draw nodes
		for (int n = 0; n < nNodes; n++)
		{
			if (rate[n] > 0) Gizmos.color = Color.blue;
			else if (rate[n] < 0) Gizmos.color = Color.red;
			else Gizmos.color = Color.green;
			Gizmos.DrawSphere(new Vector3(nodeX[n], 0, nodeY[n]), 0.2f);
		}
	}

	void AssignDelivery()
	{
		int nOptions = source.Count;
		Debug.Log(nOptions + " delivery options remaining.");
		if (nOptions > 0)
		{
			float minValue = float.MaxValue;
			int minIndex = -1;
			for (int i = 0; i < cost.Count; i++)
			{
				if (cost[i] < minValue)
				{
					minValue = cost[i];
					minIndex = i;
				}
			}
			Debug.Log("Minimum cost delivery is " + minValue + " at index " + minIndex);
			int _source = source[minIndex];
			int _sink = sink[minIndex];
			float available = rate[_source];
			float required = rate[_sink];
			//Debug.Log(required + " is required of an available " + available);
			if (required <= available)
			{
				remain[_source] += required;
				remain[_sink] -= required;

				// Remove sink
				List<int> toRemove = new List<int>();
				for (int i = 0; i < sink.Count; i++)
				{
					if (sink[i] == _sink) toRemove.Add(i);
				}
				for (int a = 0; a < toRemove.Count; a++)
				{
					int i = toRemove[a];
					for (int b = 0; b < a; b++) if (toRemove[b] < toRemove[a]) i--;
					source.RemoveAt(i);
					sink.RemoveAt(i);
					cost.RemoveAt(i);
				}

				if (remain[_source] == 0)
				{
					// Remove source
					toRemove.Clear();
					for (int i = 0; i < source.Count; i++)
					{
						if (source[i] == _source) toRemove.Add(i);
					}
					for (int a = 0; a < toRemove.Count; a++)
					{
						int i = toRemove[a];
						for (int b = 0; b < a; b++) if (toRemove[b] < toRemove[a]) i--;
						source.RemoveAt(i);
						sink.RemoveAt(i);
						cost.RemoveAt(i);
					}
				}
			}
			else
			{
				remain[_source] -= available;
				remain[_sink] += available;

				// Remove source
				List<int> toRemove = new List<int>();
				for (int i = 0; i < source.Count; i++)
				{
					if (source[i] == _source) toRemove.Add(i);
				}
				for (int a = 0; a < toRemove.Count; a++)
				{
					int i = toRemove[a];
					for (int b = 0; b < a; b++) if (toRemove[b] < toRemove[a]) i--;
					source.RemoveAt(i);
					sink.RemoveAt(i);
					cost.RemoveAt(i);
				}
			}
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
