using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem04Controller : MonoBehaviour
{
	[Range(8, 24)]
	public int numNodes = 20;
	[Range(0.1f, 0.5f)]
	public float productionProb = 0.25f;

	private List<Node> node;

	private int frameCounter;
	private bool allNodesGenerated;

	void OnEnable()
	{
		frameCounter = 0;
		allNodesGenerated = false;
	}

	void Update()
	{
		frameCounter++;

		if (!allNodesGenerated)
		{
			if (frameCounter % 10 == 0)
			{
				GenerateNode();

				if (node.Count == numNodes)
				{
					allNodesGenerated = true;
					Debug.Log("All nodes generated.");
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		// Draw nodes
		if (node != null)
		{
			foreach (Node n in node)
			{
				if (n.prod[0] > 0) Gizmos.color = Color.blue;
				else if (n.prod[0] < 0) Gizmos.color = Color.red;
				else Gizmos.color = Color.green;
				Gizmos.DrawSphere(n.pos, n.maxOut / 5f);
			}
		}
	}

	private void GenerateNode()
	{
		if (node == null) node = new List<Node>();

		int n = node.Count;

		// Position
		float x = Random.value * numNodes; // [0, numNodes]
		float y = Random.value * numNodes; // [0, numNodes]
		Vector3 pos = new Vector3(x, 0, y);

		// Production
		float[] prod = new float[1];
		if (Random.value < productionProb) prod[0] = (Random.value - 0.5f) * 2f; // [-1, +1]
		else prod[0] = 0f;

		// Infrastructure
		float maxOut = (Random.value * 0.5f) + 0.5f; // [+0.5, +1.0]

		// Create node
		node.Add(new Node(pos, prod, maxOut));

		// Add new distance edges
		for (int i = 0; i < n; i++)
		{
			node[i].AddDistanceEdge(new DistanceEdge(node[n], (node[n].pos - node[i].pos).magnitude));
			node[n].AddDistanceEdge(new DistanceEdge(node[i], (node[i].pos - node[n].pos).magnitude));
		}
	}
}
