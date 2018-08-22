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

	void Start()
	{
		// Initialise
		// ...

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
	}
	
	void Update()
	{
		
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
		Debug.Log(remaining+" source remaining.");

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
		Debug.Log(remaining+" sink remaining.");

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
}
