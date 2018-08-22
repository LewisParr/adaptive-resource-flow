using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem02Controller : MonoBehaviour
{
	public int nNodes = 12;
	public float totalProduction = 5f;
	public float minProduction = 1f;
	public float maxProduction = 2f;
	public float distanceExponent = 1.25f;

	private float[] nodeX;
	private float[] nodeY;
	private float[,] distance;
	private float[,] minCost;
	private IDictionary<Vector2, int[]> minCostPath;
	private float[] rate;
	private float maxRate;
	private float[,] flow;

	List<int> source;
	List<int> sink;
	List<float> cost;
	float[] remain;

	private int frameCount;

	void Start()
	{
		// Initialise
		minCost = new float[nNodes, nNodes];
		minCostPath = new Dictionary<Vector2, int[]>();
		flow = new float[nNodes, nNodes];
		frameCount = 0;

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
		rate = RandomRates(totalProduction, minProduction, maxProduction);
		maxRate = 0f;
		foreach (float r in rate) if (Mathf.Abs(r) > maxRate) maxRate = Mathf.Abs(r);

		// Find each delivery option
		source = new List<int>();
		sink = new List<int>();
		cost = new List<float>();
		remain = new float[nNodes];
		for (int i = 0; i < nNodes; i++) remain[i] = rate[i];
		for (int a = 0; a < nNodes; a++) if (rate[a] > 0)
		{
			// For each source, ...
			for (int b = 0; b < nNodes; b++) if (rate[b] < 0)
			{
				// ... and each sink
				source.Add(a); // Record source index
				sink.Add(b); // Record sink index
				cost.Add(minCost[a, b]); // Record cost of delivery
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
			if (rate[n] > 0) Gizmos.color = Color.Lerp(Color.white, Color.blue, rate[n] / maxRate);
			else if (rate[n] < 0) Gizmos.color = Color.Lerp(Color.white, Color.red, -rate[n] / maxRate);
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

	private float[] RandomRates(float total, float min, float max)
	{
		float[] r = new float[nNodes];

		// Sources
		float remaining = total;
		while (remaining > (2f * min))
		{
			float amount = Random.value * max;
			int n = Mathf.FloorToInt(Random.value * nNodes);
			if (r[n] == 0)
			{
				//Debug.Log("Assigning " + amount + " of " + remaining);
				r[n] = amount;
				remaining -= amount;
			}
		}
		int i = Mathf.FloorToInt(Random.value * nNodes);
		while (r[i] != 0) i = Mathf.FloorToInt(Random.value * nNodes);
		//Debug.Log("Assigning " + remaining + " of " + remaining);
		r[i] = remaining;
		remaining -= remaining;
		//Debug.Log("Remaining: " + remaining);

		// Sinks
		remaining = -total;
		while (remaining < (-2f * min))
		{
			float amount = Random.value * -max;
			int n = Mathf.FloorToInt(Random.value * nNodes);
			if (r[n] == 0)
			{
				//Debug.Log("Assigning " + amount + " of " + remaining);
				r[n] = amount;
				remaining -= amount;
			}
		}
		i = Mathf.FloorToInt(Random.value * nNodes);
		while (r[i] != 0) i = Mathf.FloorToInt(Random.value * nNodes);
		//Debug.Log("Assigning " + remaining + " of " + remaining);
		r[i] = remaining;
		remaining -= remaining;
		//Debug.Log("Remaining: " + remaining);

		// Return rates
		return r;
	}

	private void AssignDelivery()
	{
		int nOptions = source.Count;
		Debug.Log(nOptions + " delivery options remaining.");
		if (nOptions > 0)
		{
			// Find the cheapest delivery option
			float minValue = float.MaxValue;
			int minIndex = -1;
			for (int i = 0; i < nOptions; i++) if (cost[i] < minValue)
			{
				minValue = cost[i];
				minIndex = i;
			}
			Debug.Log("Minimum cost delivery is " + minValue + " at index " + minIndex);
			int _source = source[minIndex]; // Node index of source of cheapest delivery option
			int _sink = sink[minIndex]; // Node index of sink of cheapest delivery option
			float available = remain[_source];
			float required = remain[_sink];
			Vector2 key = new Vector2(_source, _sink);
			int[] deliveryPath = minCostPath[key];
			Debug.Log(required+" is required; "+available+" is available.");
			if (Mathf.Abs(required) <= Mathf.Abs(available))
			{
				remain[_source] += required;
				remain[_sink] -= required;
				for (int i = 0; i < deliveryPath.Length - 1; i++)
				{
					int a = deliveryPath[i];
					int b = deliveryPath[i + 1];
					flow[a, b] -= required;
				}

				//Debug.Log(required+" of "+rate[_source]+" was sourced, "+remain[_source]+" remains.");
				//Debug.Log(required+" of "+rate[_sink]+" was sunk, "+remain[_sink]+" remains.");

				// Remove sink
				List<int> toRemove = new List<int>();
				for (int i = 0; i < nOptions; i++) if (sink[i] == _sink) toRemove.Add(i);
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
					// Recount remaining options
					nOptions = source.Count;

					// Remove source
					toRemove.Clear();
					for (int i = 0; i < nOptions; i++) if (source[i] == _source) toRemove.Add(i);
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
				for (int i = 0; i < deliveryPath.Length - 1; i++)
				{
					int a = deliveryPath[i];
					int b = deliveryPath[i + 1];
					flow[a, b] += available;
				}

				//Debug.Log(available+" of "+rate[_source]+" was sourced, "+remain[_source]+" remains.");
				//Debug.Log(available+" of "+rate[_sink]+" was sunk, "+remain[_sink]+" remains.");

				// Remove source
				List<int> toRemove = new List<int>();
				for (int i = 0; i < nOptions; i++) if (source[i] == _source) toRemove.Add(i);
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
}
