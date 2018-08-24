﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem04Controller : MonoBehaviour
{
	public int numNodes = 20;
	public float productionProb = 0.25f;

	private List<Node> node;

	void OnEnable()
	{
		GenerateNodes();
	}

	void OnDrawGizmos()
	{
		// Draw nodes
		Gizmos.color = Color.green;
		foreach (Node n in node)
		{
			if (n.prod[0] > 0) Gizmos.color = Color.blue;
			else if (n.prod[0] < 0) Gizmos.color = Color.red;
			else Gizmos.color = Color.green;
			Gizmos.DrawSphere(n.pos, 0.2f);
		}
	}

	private void GenerateNodes()
	{
		if (node == null)
		{
			node = new List<Node>();
		}

		for (int i = 0; i < numNodes; i++)
		{
			// Position
			float x = Random.value * numNodes; // [0, numNodes]
			float y = Random.value * numNodes; // [0, numNodes]
			Vector3 pos = new Vector3(x, 0, y);

			// Production
			float[] prod = new float[1];
			if (Random.value < productionProb)
			{
				prod[0] = (Random.value - 0.5f) * 2f; // [-1, +1]
			}

			node.Add(new Node(pos, prod));
		}
	}
}
