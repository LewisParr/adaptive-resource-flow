using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem03Controller : MonoBehaviour
{
	public int nNodes = 12;
	public float totalProduction = 5f;
	public float minProduction = 1f;
	public float maxProduction = 2.5f;
	public float distanceExponent = 1.25f;
	public float minFlowLimit = 2.5f;
	public float maxFlowLimit = 3.5f;

	private float[] nodeX;
	private float[] nodeY;
	private float[,] distance;
	private float[] rate;
	private float maxRate;
	private float[] nodeMaxFlow;
	private float[,] flow;

	List<int> source;
	List<int> sink;
	List<float> cost;
	List<int[]> path;
	float[] remainingRate;
	float[] remainingFlow;

	private int frameCount;

	void Start()
	{
		// Initialise
		flow = new float[nNodes, nNodes];
		frameCount = 0;

		// Generate random nodes
		nodeX = RandomValues(nNodes, 0, nNodes);
		nodeY = RandomValues(nNodes, 0, nNodes);

		// Calculate distances
		distance = CalculateDistances(nodeX, nodeY);

		// Generate random sources and sinks
		rate = RandomRates(totalProduction, minProduction, maxProduction);
		maxRate = 0f;
		foreach (float r in rate) if (Mathf.Abs(r) > maxRate) maxRate = Mathf.Abs(r);

		// Generate maximum node flows
		nodeMaxFlow = RandomValues(nNodes, minFlowLimit, maxFlowLimit);

		// Find maximum flow (all sources and sinks)
		//float maximumFlow = EdmondsKarp.NodeLimit(); WIP

		// Find each delivery option
		source = new List<int>();
		sink = new List<int>();
		cost = new List<float>();
		path = new List<int[]>();
		remainingRate = new float[nNodes];
		for (int i = 0; i < nNodes; i++) remainingRate[i] = rate[i];
		remainingFlow = new float[nNodes];
		for (int i = 0; i < nNodes; i++) remainingFlow[i] = nodeMaxFlow[i];
		for (int a = 0; a < nNodes; a++) if (rate[a] > 0)
		{
			// For each source, ...
			for (int b = 0; b < nNodes; b++) if (rate[b] < 0)
			{
				// ... and each sink
				source.Add(a); // Record source index
				sink.Add(b); // Record sink index
				DijkstraSolution solution = Dijkstra.Algorithm(a, b, distance, distanceExponent);
				float minCost = solution.cost;
				cost.Add(minCost); // Record cost of delivery
				path.Add(solution.path); // Record path of delivery
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
			Gizmos.DrawSphere(new Vector3(nodeX[n], 0, nodeY[n]), nodeMaxFlow[n] / 10f);
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

	private float[] RandomValues(int num, float min, float max)
	{
		float[] values = new float[num];
		for (int i = 0; i < num; i++) values[i] = min + (Random.value * (max - min));
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

	private float[] RandomRates(float total, float min, float max)
	{
		float[] r = new float[nNodes];

		// Sources
		float remaining = total;
		while (remaining > (2f * min))
		{
			float amount = Random.value * max;
			int n = RandomIndexWithValue(r, 0);
			r[n] = amount;
			remaining -= amount;
		}
		int i = RandomIndexWithValue(r, 0);
		r[i] = remaining;
		remaining -= remaining;
		//Debug.Log(remaining+" source remaining.");

		// Sinks
		remaining = -total;
		while (remaining < (-2f * min))
		{
			float amount = Random.value * -max;
			int n = RandomIndexWithValue(r, 0);
			r[n] = amount;
			remaining -= amount;
		}
		i = RandomIndexWithValue(r, 0);
		r[i] = remaining;
		remaining -= remaining;
		//Debug.Log(remaining+" sink remaining.");

		// Return rates
		return r;
	}

	private int RandomIndexWithValue(float[] array, float value)
	{
		int N = array.Length;
		int n = Mathf.FloorToInt(Random.value * N);
		while (array[n] != value) n = Mathf.FloorToInt(Random.value * N);
		return n;
	}

	private void AssignDelivery()
	{
		int nOptions = source.Count;
		Debug.Log(nOptions+" delivery options remaining.");
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
			Debug.Log("Minimum cost delivery is "+minValue+" at index "+minIndex);
			int _source = source[minIndex]; // Node index of source of cheapest delivery option
			int _sink = sink[minIndex]; // Node index of sink of cheapest delivery option
			float available = remainingRate[_source];
			float required = remainingRate[_sink];
			Vector2 key = new Vector2(_source, _sink);
			int[] deliveryPath = path[minIndex]; // Indexes of path of cheapest delivery option
			Debug.Log(required+" is required; "+available+" is available.");

			// Find the flow limit through nodes on path
			float nodeLimit = float.MaxValue;
			int nodeLimitIndex = -1;
			foreach (int n in deliveryPath) if (remainingFlow[n] < nodeLimit)
			{
				nodeLimit = remainingFlow[n];
				nodeLimitIndex = n;
			}
			Debug.Log(nodeLimit+" is the flow limit.");

			if (Mathf.Abs(required) <= available)
			{
				if (nodeLimit < Mathf.Abs(required))
				{
					// nodeLimit < required
					ApplyDelivery(_source, _sink, deliveryPath, nodeLimit);
				}
				else
				{
					// required <= nodeLimit
					ApplyDelivery(_source, _sink, deliveryPath, Mathf.Abs(required));
				}
			}
			else
			{
				if (nodeLimit < available)
				{
					// nodeLimit < available
					ApplyDelivery(_source, _sink, deliveryPath, nodeLimit);
				}
				else
				{
					// available <= nodeLimit
					ApplyDelivery(_source, _sink, deliveryPath, available);
				}
			}
		}
		else
		{
			// Remaining source
			float remainingSource = 0f;
			foreach (float r in remainingRate) if (r > 0) remainingSource += r;
			Debug.Log("Remaining source: "+remainingSource);

			// Remaining sink
			float remainingSink = 0f;
			foreach (float r in remainingRate) if (r < 0) remainingSink -= r;
			Debug.Log("Remaining sink: "+remainingSink);
		}
	}

	private void ApplyDelivery(int _source, int _sink, int[] _path, float amount)
	{
		// Reduce the amount of source remaining
		remainingRate[_source] -= amount;

		// Reduce the amount of sink remaining
		remainingRate[_sink] += amount;

		// Reduce the amount of flow remaining along path
		foreach (int n in _path) remainingFlow[n] -= amount;

		// Assign flow along minimum cost path from source to sink
		for (int i = 0; i < _path.Length - 1; i++)
		{
			int a = _path[i];
			int b = _path[i + 1];
			flow[a, b] += amount;
		}

		// Remove any delivery options that are now invalid
		CheckExhaustedSource(_source);
		CheckExhaustedSink(_sink);
		CheckExhaustedNodes(_path);
	}

	private void CheckExhaustedSource(int _source)
	{
		if (remainingRate[_source] == 0)
		{
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
				path.RemoveAt(i);
			}
		}
	}

	private void CheckExhaustedSink(int _sink)
	{
		if (remainingRate[_sink] == 0)
		{
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
				path.RemoveAt(i);
			}
		}
	}

	private void CheckExhaustedNodes(int[] _path)
	{
		foreach (int n in _path) if (remainingFlow[n] == 0)
		{
			List<int> toRemove = new List<int>();
			for (int i = 0; i < path.Count; i++)
			{
				bool inPath = false;
				foreach (int j in path[i]) if (j == n) inPath = true;
				if (inPath) toRemove.Add(i);
			}
			for (int a = 0; a < toRemove.Count; a++)
			{
				int i = toRemove[a];
				for (int b = 0; b < a; b++) if (toRemove[b] < toRemove[a]) i--;
				source.RemoveAt(i);
				sink.RemoveAt(i);
				cost.RemoveAt(i);
				path.RemoveAt(i);
			}
		}
	}
}
