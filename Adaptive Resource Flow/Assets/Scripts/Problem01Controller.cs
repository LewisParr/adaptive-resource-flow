using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem01Controller : MonoBehaviour
{
	public int nNodes = 8;
	public int nEnds = 2;
	public float distanceExponent = 1.1f;

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

	void Start()
	{
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
		for (int m = 0; m < nNodes; m++) for (int n = 0; n < nNodes; n++)
											distance[m, n] = Mathf.Sqrt(Mathf.Pow(nodeX[m] - nodeX[n], 2) + Mathf.Pow(nodeY[m] - nodeY[n], 2));

		// Find shortest paths
		for (int m = 0; m < nNodes; m++)
		{
			for (int n = 0; n < nNodes; n++)
			{
				DijkstraSolution solution = Dijkstra.Algorithm(m, n, distance, distanceExponent);
				minCost[m, n] = solution.cost;
				UpdateMinCostPath(m, n, solution.path);
			}
		}

		// Generate random sources and sinks
		for (int i = 0; i < nEnds; i++)
		{
			int n = Mathf.FloorToInt(Random.value * nNodes);
			if (rate[n] == 0) rate[n] = 1;
			else i--;
		}
		for (int i = 0; i < nEnds; i++)
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
	}
	
	void Update()
	{
		frameCount++;
		if (frameCount % 100 == 0) AssignDelivery();
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

		// Draw flows
		Gizmos.color = Color.white;
		for (int a = 0; a < nNodes; a++)
		{
			for (int b = 0; b < nNodes; b++)
			{
				float amount = flow[a, b];
				if (amount > 0)
				{
					Vector3 pointA = new Vector3(nodeX[a], 0, nodeY[a]);
					Vector3 pointB = new Vector3(nodeX[b], 0, nodeY[b]);
					Gizmos.DrawLine(pointA, pointB);
				}
			}
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
			Vector2 key = new Vector2(_source, _sink);
			int[] deliveryPath = minCostPath[key];
			//Debug.Log(required + " is required of an available " + available);
			if (required <= available)
			{
				// Reduce the amount of source remaining
				remain[_source] += required;
				// Reduce the amount of sink remaining
				remain[_sink] -= required;
				// Assign flow along minimum cost path from source to sink
				for (int i = 0; i < deliveryPath.Length - 1; i++)
				{
					int a = deliveryPath[i];
					int b = deliveryPath[i + 1];
					flow[a, b] -= required;
				}

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
				// Reduce the amount of source remaining
				remain[_source] -= available;
				// Reduce the amount of sink remaining
				remain[_sink] += available;
				// Assign flow along minimum cost path from source to sink
				for (int i = 0; i < deliveryPath.Length - 1; i++)
				{
					int a = deliveryPath[i];
					int b = deliveryPath[i + 1];
					flow[a, b] += available;
				}

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